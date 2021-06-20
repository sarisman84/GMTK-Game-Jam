using System;
using System.Collections;
using System.Collections.Generic;
using Enemies.AI;
using Level.UI;
using Managers;
using Player;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using UnityEngine.SceneManagement;
using Utility.Attributes;


namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        #region New Code

        public GameObject playerPrefab;

        [Header("Menu Settings")] public AudioClip menuMusic;
        public Canvas timeDisplayPrefab;

        public Canvas transitionPrefab;
        public CanvasGroup menuCanvas;
        public DoTweenAnimationClip onGameOverTransition, onVictoryTransition, toMainMenuTransition;


        [Header("Level Settings")] [Expose] public List<LevelSettings> currentLevelSettings;

        public Vector2 aStarGridSize;
        public LayerMask aStarObstacleMask;
        public float aStarNodeRadius;
        public bool displayGrid;

        [Space] public UnityEvent onGameOver;
        public UnityEvent onGameCompletion;
        public UnityEvent onLevelTransitionEnter;
        public UnityEvent onLevelTransitionExit;

        private MusicPlayer _musicPlayer;
        private TimeDisplayer _timeDisplayer;
        private EnemyGenerator _enemyGenerator;
        private AsteroidField _asteroidGenerator;
        private PathfindingManager _pathfindingManager;
        private DoTweenAnimator _transition;

        private PlayerController _player;
        private int _currentLevel;

        private readonly List<LevelSettings.Level> _selectedLevels = new();
        private Scene _gameManagers;
        private Scene _mainMenu;
        private Coroutine _loadLevelCoroutine;


        private void Awake()
        {
            StartCoroutine(Setup());
        }

        public void StartGame()
        {
            _currentLevel = 0;
            if (_loadLevelCoroutine != null)
            {
                StopCoroutine(_loadLevelCoroutine);
            }

            _player.FullReset();
            _loadLevelCoroutine = StartCoroutine(LoadLevel(_currentLevel));
        }

        private bool isTransitioning = false;

        private void Update()
        {
            if (_timeDisplayer && _player && _timeDisplayer.TimeCounter != null)
            {
                if (_timeDisplayer.TimeCounter.HasRanOutOfTime(_selectedLevels[_currentLevel].timerType ==
                                                               LevelSettings.CountdownType.GameOverOnZero) ||
                    (!_player.gameObject.activeSelf && _player.HealthManager.IsFlaggedForDeath) &&
                    !isTransitioning)
                {
                    if (_loadLevelCoroutine != null)
                    {
                        StopCoroutine(_loadLevelCoroutine);
                    }
                    
                    isTransitioning = true;
                    StartCoroutine(OnGameOver());
                    
                }
                else if (_timeDisplayer.TimeCounter.HasRanOutOfTime(_selectedLevels[_currentLevel].timerType ==
                                                                    LevelSettings.CountdownType.ProgressOnZero) &&
                         !isTransitioning)
                {
                    TransitionToNextLevel();
                    isTransitioning = true;
                }
            }
        }

        private IEnumerator Setup()
        {
            _mainMenu = SceneManager.GetActiveScene();

            foreach (var currentLevelSetting in currentLevelSettings)
            {
                var chosenLevel = currentLevelSetting.SelectRandom();
                yield return SceneManager.LoadSceneAsync(chosenLevel.levelScene, LoadSceneMode.Additive);
                _selectedLevels.Add(chosenLevel);
                chosenLevel.FetchScene().SetSceneActive(false);
            }

            yield return CreateSetupScene();
        }


        private IEnumerator CreateSetupScene()
        {
            _gameManagers = SceneManager.CreateScene("Game Managers");
            SceneManager.SetActiveScene(_gameManagers);

            _transition = new GameObject("Transition Manager").AddComponent<DoTweenAnimator>();
            Canvas clone = Instantiate(transitionPrefab, _transition.transform);
            _transition.AddObjectsToTween(clone.gameObject);

            _musicPlayer = new GameObject("Music Player").AddComponent<MusicPlayer>();
            _timeDisplayer = new GameObject("Time Displayer").AddComponent<TimeDisplayer>();
            _enemyGenerator = new GameObject("Enemy Spawner").AddComponent<EnemyGenerator>();
            _asteroidGenerator = new GameObject("Asteroid Field").AddComponent<AsteroidField>();

            _timeDisplayer.Setup(timeDisplayPrefab);

            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                .GetComponentInChildren<PlayerController>();
            _player.SetActive(false, false);
            _musicPlayer.Play(menuMusic);

            _pathfindingManager =
                new PathfindingManager(transform.position, aStarObstacleMask, aStarGridSize, aStarNodeRadius,
                    _enemyGenerator);
            _pathfindingManager.logistics.area.displayGrid = displayGrid;
            _transition.Play(toMainMenuTransition);


            yield return null;
        }

        private IEnumerator LoadLevel(int newLevelIndex)
        {
            onLevelTransitionExit?.Invoke();

            var currentLevel = _selectedLevels[_currentLevel];
            _transition.Play(currentLevel.onLevelExitTransition);
            yield return new WaitUntil(() => !_transition.isPlaying);

            if (menuCanvas && menuCanvas.alpha != 0)
            {
                menuCanvas.alpha = 0;
                menuCanvas.interactable = false;
            }


            yield return ResetGame();


            if (newLevelIndex >= _selectedLevels.Count)
            {
                yield return OnGameComplete();
            }


            var selectedLevel = _selectedLevels[newLevelIndex];


            selectedLevel.FetchScene().SetSceneActive(true);
            _pathfindingManager.logistics.area.UpdateGrid();
            _musicPlayer.Play(selectedLevel.musicClip);


            _enemyGenerator.Generate(_player, selectedLevel.FetchScene(), selectedLevel.uniqueEnemies,
                selectedLevel.minEnemySpawnRate, selectedLevel.maxEnemySpawnRate,
                selectedLevel.enemySpawnDistanceFromPlayer);

            if (selectedLevel.spawnAsteroids)
                _asteroidGenerator.Generate(_player, selectedLevel.FetchScene(), selectedLevel.uniqueAsteroids,
                    selectedLevel.minAsteroidSpawnRate, selectedLevel.maxAsteroidSpawnRate,
                    selectedLevel.asteroidSpawnDistanceFromPlayer, _player);


            _timeDisplayer.BeginCounting(selectedLevel.timeRemaining);

            Vector3 pos = Vector3.zero;
            if (FetchGameObjectWithTagFromScene(selectedLevel.FetchScene(), selectedLevel.SpawnPos()) is
                { } foundSpawnPos)
                pos = foundSpawnPos.transform.position;
            _player.SetActive(true, false);
            _player.AbilityManager.AddAbility(currentLevelSettings[newLevelIndex].GetRandomAbility());
            yield return _player.TeleportPlayerToPos(pos);


            yield return FindAndSetup(newLevelIndex, "Level/Exit", "Level/Instructions");
            _currentLevel = newLevelIndex;

            onLevelTransitionEnter?.Invoke();
            _transition.Play(selectedLevel.onLevelEnterTransition);
            yield return new WaitUntil(() => !_transition.isPlaying);
            _player.SetInputActive(true);
            _player.AbilityManager.display.SetActive(true);
            isTransitioning = false;
        }

        private IEnumerator ResetGame()
        {
            _enemyGenerator.StopGenerating();
            _asteroidGenerator.StopGenerating();
            _timeDisplayer.Reset();
            ClearBullets();
            _player.AbilityManager.DisruptAbilities();
            _player.AbilityManager.ResetCd();
            _player.AbilityManager.display.SetActive(false);
            _player.ManualUpdateExitTracker(null);
            _player.SetActive(false, false);


            yield return UnloadPreviousLevel(_currentLevel);
        }

        private void ClearBullets()
        {
            foreach (var bullet in FindObjectsOfType<Bullet>())
            {
                bullet.gameObject.SetActive(false);
            }
        }

        private IEnumerator OnGameComplete()
        {
            _transition.Play(onVictoryTransition);
            yield return new WaitUntil(() => !_transition.isPlaying);
            isTransitioning = false;
            yield return ResetGame();
            yield return TransitionToMainMenu();
            onGameCompletion?.Invoke();
        }

        private IEnumerator OnGameOver()
        {
            _transition.Play(onGameOverTransition);
            yield return new WaitUntil(() => !_transition.isPlaying);
            yield return ResetGame();
            yield return TransitionToMainMenu();
            onGameOver?.Invoke();
            isTransitioning = false;
        }


        private IEnumerator FindAndSetup(int newLevelIndex, params string[] input)
        {
            Detector foundDetector = null;
            foreach (var variable in input)
            {
                GameObject foundElement =
                    FetchGameObjectWithTagFromScene(_selectedLevels[newLevelIndex].FetchScene(), variable);

                if (foundElement)
                    switch (variable)
                    {
                        case "Level/Exit":
                            foundDetector = foundElement.GetComponent<Detector>();
                            foundDetector.ONTriggerEnter.RemoveAllListeners();
                            foundDetector.currentPossessionRequired = foundDetector.additionalPossessionsRequired;
                            foundDetector.currentPossessionRequired +=
                                _player.PossessionManager.possessedEntities.Count;
                            foundDetector.ResetEvents();
                            foundDetector.ONTriggerEnter.AddListener((col) =>
                            {
                                if (col.gameObject.GetInstanceID() ==
                                    _player.gameObject.GetInstanceID() &&
                                    _player.PossessionManager.possessedEntities.Count >=
                                    foundDetector.currentPossessionRequired)
                                    TransitionToNextLevel();
                            });


                            _player.UpdateExitTracker(foundDetector,
                                () => _selectedLevels[newLevelIndex].timerType ==
                                      LevelSettings.CountdownType.GameOverOnZero);
                            break;

                        case "Level/Instructions":
                            if (foundDetector)
                                foundElement.GetComponent<TMP_Text>().text =
                                    $"Required possessions to progress to the next level: {foundDetector.currentPossessionRequired}";

                            break;
                    }

                yield return new WaitForSeconds(0.5f);
            }


            yield return null;
        }

        private void TransitionToNextLevel()
        {
            StartCoroutine(LoadLevel(_currentLevel + 1));
        }


        private IEnumerator UnloadPreviousLevel(int currentLevel)
        {
            _selectedLevels[currentLevel].FetchScene().SetSceneActive(false);
            yield return null;
        }


        private IEnumerator TransitionToMainMenu()
        {
            _transition.Play(toMainMenuTransition);
            _mainMenu.SetSceneActive(true);
            if (menuCanvas && menuCanvas.alpha == 0)
            {
                menuCanvas.alpha = 1;
                menuCanvas.interactable = true;
            }

            _musicPlayer.Play(menuMusic);
            yield return new WaitUntil(() => !_transition.isPlaying);
        }

        private void OnDrawGizmos()
        {
            if (_pathfindingManager != null)
                _pathfindingManager.logistics.area.DrawGrid();
        }


        #region Helper Methods

        public static GameObject FetchGameObjectWithTagFromScene(Scene scene, string tag, bool debug = false)
        {
            if (debug)
            {
                Debug.Log($"Searching for GameObjects in {scene.name}");
            }

            foreach (var rootObj in scene.GetRootGameObjects())
            {
                if (rootObj)
                {
                    Transform child = rootObj.transform.CompareTag(tag) ? rootObj.transform : null;
                    if (!child)
                        child = rootObj.transform.FindChildOfTag(tag);
                    if (child)
                    {
                        if (debug)
                            Debug.Log($"Found a gameObject with the Inputed Tag {tag}. Returning!");
                        return child.gameObject;
                    }
                }
            }

            return null;
        }

        #endregion

        #endregion
    }
}