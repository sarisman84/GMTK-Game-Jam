using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using UnityEngine.SceneManagement;
using Utility.Attributes;


namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        #region Old Code

        // public GameObject playerPrefab;
        // public TMP_Text _timerDisplayer;
        // [Expose] public List<LevelSettings> currentLevels;
        // public UnityEvent ONGameOver;
        //
        // private int _currentLevel;
        // private int _currentSublevel;
        // private bool _isTransitioning;
        //
        // private Coroutine currentCo;
        //
        // private bool _hasTimeRanOut = false;
        // private bool _showTimer = false;
        // private float _currentCountdown = float.MaxValue;
        // private bool hasStarted = false;
        //
        //
        // public void StartGame()
        // {
        //     if (!hasStarted)
        //     {
        //         if (!_timerDisplayer)
        //             throw new NullReferenceException(
        //                 $"Missing Time Displayer. Check the Inspector of {gameObject.name}");
        //         _currentLevel = 0;
        //         if (!GameMaster.singletonAccess.playerObject)
        //         {
        //             GameMaster.singletonAccess.RegisterPlayerScene(SceneManager.CreateScene("Player Scene"));
        //             SceneManager.SetActiveScene(GameMaster.singletonAccess.playerScene);
        //             GameMaster.singletonAccess.InitializePlayer(playerPrefab,
        //                 Vector3.zero);
        //             GameMaster.singletonAccess.ONPlayerGameOver += () => StartCoroutine(OnGameOver());
        //         }
        //
        //         currentCo = StartCoroutine(SetCurrentLevelTo(_currentLevel, 0.5f));
        //         hasStarted = true;
        //     }
        // }
        //
        // private void Update()
        // {
        //     _hasTimeRanOut = CountdownTime();
        //
        //     _showTimer = (_currentCountdown <= 61 ||
        //                   currentLevels[_currentLevel].subLevels[_currentSublevel].countdownType ==
        //                   LevelSettings.CountdownType.ToNextStage) &&
        //                  GameMaster.singletonAccess.playerObject.activeSelf && !_hasTimeRanOut;
        //
        //     if (_showTimer)
        //         DisplayCountdown();
        //     else
        //         _timerDisplayer.text = "";
        // }
        //
        // private bool CountdownTime()
        // {
        //     _currentCountdown -= Time.deltaTime;
        //     return _currentCountdown <= 0;
        // }
        //
        //
        // private bool HasTimeRanOut(int newLevel, int newSubLevel, LevelSettings.CountdownType type)
        // {
        //     LevelSettings settings = currentLevels[newLevel];
        //
        //     LevelSettings.Level level = settings.subLevels[newSubLevel];
        //     return level.countdownType == type && _hasTimeRanOut &&
        //            GameMaster.singletonAccess.playerObject.activeSelf;
        // }
        //
        // public Vector2Int DisplayCountdown()
        // {
        //     int minutes = Mathf.FloorToInt(_currentCountdown / 60f);
        //     int seconds = Mathf.FloorToInt(_currentCountdown % 60f);
        //     _timerDisplayer.text = $"{minutes:00}:{seconds:00}";
        //     return new Vector2Int(minutes, seconds);
        // }
        //
        //
        // private IEnumerator SetCurrentLevelTo(int newLevel, float delay)
        // {
        //     yield return ResetPreviousLevel();
        //     GameMaster.singletonAccess.playerCompass.DisableTracker = true;
        //     GameMaster.singletonAccess.possessor.SetPossessionsActive(false);
        //     GameMaster.singletonAccess.playerObject.SetActive(false);
        //
        //     int newSubLevel = 0;
        //     yield return new WaitForSeconds(delay);
        //     if (newLevel < currentLevels.Count)
        //     {
        //         newSubLevel = currentLevels[newLevel].SelectRandom();
        //         _currentCountdown = currentLevels[newLevel].timeRemaining;
        //     }
        //
        //     if (newLevel >= currentLevels.Count)
        //     {
        //         yield return OnGameOver();
        //         _isTransitioning = false;
        //         yield break;
        //     }
        //
        //     AsyncOperation op = SceneManager.LoadSceneAsync(
        //         $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}",
        //         LoadSceneMode.Additive);
        //
        //     op.allowSceneActivation = true;
        //     yield return new WaitUntil(() => op.isDone && SceneManager.GetSceneByName(
        //         $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}").isLoaded);
        //     SceneManager.SetActiveScene(
        //         SceneManager.GetSceneByName(
        //             $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}"));
        //
        //     Transform potentialPos =
        //         GetGameObjectFromActiveSceneWithTag(currentLevels[_currentLevel].SpawnPos(_currentSublevel), true);
        //
        //     Vector3 foundPosition = potentialPos ? potentialPos.position : Vector3.zero;
        //
        //     GameObject exit = GetGameObjectFromActiveSceneWithTag("Level/Exit", true)?.gameObject;
        //     GameObject instruction = GetGameObjectFromActiveSceneWithTag("Level/Instruction")?.gameObject;
        //     if (exit)
        //     {
        //         Detector detector = exit.GetComponent<Detector>();
        //         detector.ONTriggerEnter.RemoveAllListeners();
        //         detector.currentPossessionRequired = detector.additionalPossessionsRequired;
        //         detector.currentPossessionRequired += GameMaster.singletonAccess.possessor.possessedEntities.Count;
        //         detector.ONTriggerEnter.AddListener((col) =>
        //         {
        //             if (col.gameObject.GetInstanceID() == GameMaster.singletonAccess.playerObject.GetInstanceID())
        //                 TransitionToNextLevel(0.5f);
        //         });
        //         GameMaster.singletonAccess.ClearUpdateEvents();
        //         GameMaster.singletonAccess.ONUpdate += () =>
        //         {
        //             if (detector)
        //                 detector.gameObject.SetActive(GameMaster.singletonAccess.possessor.possessedEntities.Count >=
        //                                               detector.currentPossessionRequired);
        //             if (instruction)
        //             {
        //                 instruction.GetComponent<TMP_Text>().text =
        //                     $"Required possessions: {detector.currentPossessionRequired}";
        //             }
        //         };
        //     }
        //
        //     GetComponentFromScene<EnemyGenerator>()?.Generate();
        //
        //     GameMaster.singletonAccess.possessor.SetPossessionsActive(true);
        //     GameMaster.singletonAccess.playerObject.gameObject.SetActive(true);
        //     GameMaster.singletonAccess.playerObject.transform.position = foundPosition;
        //     GameMaster.singletonAccess.possessor.TeleportPossessionsToPosition(foundPosition);
        //     GameMaster.singletonAccess.abilityManager.AddAbility(currentLevels[newLevel].GetRandomAbility());
        //     GameMaster.singletonAccess.playerCompass.DisableTracker =
        //         currentLevels[newLevel].subLevels[newSubLevel].countdownType == LevelSettings.CountdownType.ToNextStage;
        //     _currentLevel = newLevel;
        //     _isTransitioning = false;
        //     currentCo = null;
        //     while (true)
        //     {
        //         if (GameMaster.singletonAccess.playerObject &&
        //             GameMaster.singletonAccess.playerObject.GetComponent<HealthModifier>() is { } healthModifier &&
        //             healthModifier.IsFlaggedForDeath && !GameMaster.singletonAccess.playerObject.activeSelf ||
        //             HasTimeRanOut(_currentLevel, _currentSublevel, LevelSettings.CountdownType.ToGameOver))
        //         {
        //             yield return OnGameOver();
        //             _hasTimeRanOut = true;
        //             _currentCountdown = 0;
        //             GameMaster.singletonAccess.playerCompass.DisableTracker = true;
        //             yield break;
        //         }
        //
        //         if (HasTimeRanOut(_currentLevel, _currentSublevel, LevelSettings.CountdownType.ToNextStage))
        //         {
        //             TransitionToNextLevel(0.5f);
        //             yield break;
        //         }
        //
        //
        //         yield return new WaitForEndOfFrame();
        //     }
        // }
        //
        // private IEnumerator OnGameOver()
        // {
        //     yield return ResetPreviousLevel();
        //     GameMaster.singletonAccess.possessor.ResetPossessions();
        //     GameMaster.singletonAccess.playerHealth.DestroyThis();
        //     GameMaster.singletonAccess.playerWeaponManager.ResetWeaponLibrary();
        //     GameMaster.singletonAccess.abilityManager.ManualReset();
        //
        //     GameMaster.singletonAccess.playerCompass.DisableTracker = true;
        //     ONGameOver?.Invoke();
        //     hasStarted = false;
        //     _isTransitioning = false;
        //     if (currentCo != null)
        //         StopCoroutine(currentCo);
        // }
        //
        // public static Transform GetGameObjectFromActiveSceneWithTag(string inputTag, bool debug = false)
        // {
        //     if (debug)
        //     {
        //         Debug.Log($"Searching for GameObjects in {SceneManager.GetActiveScene().name}");
        //     }
        //
        //     foreach (var rootObj in SceneManager.GetActiveScene().GetRootGameObjects().ToList())
        //     {
        //         if (rootObj)
        //         {
        //             Transform child = rootObj.transform.tag.Contains(inputTag) ? rootObj.transform : null;
        //             if (!child)
        //                 child = rootObj.transform.FindChildOfTag(inputTag);
        //             if (child)
        //             {
        //                 if (debug)
        //                     Debug.Log($"Found a gameObject with the Inputed Tag {inputTag}. Returning!");
        //                 return child;
        //             }
        //         }
        //     }
        //
        //     return null;
        // }
        //
        // private T GetComponentFromScene<T>()
        // {
        //     foreach (var rootObj in SceneManager.GetActiveScene().GetRootGameObjects().ToList())
        //     {
        //         if (rootObj)
        //         {
        //             T child = rootObj.GetComponentInChildren<T>();
        //
        //             if (child != null)
        //             {
        //                 return child;
        //             }
        //         }
        //     }
        //
        //     return default;
        // }
        //
        // private IEnumerator ResetPreviousLevel()
        // {
        //     if (SceneManager.GetSceneByName(currentLevels[_currentLevel].subLevels[_currentSublevel]
        //         .levelScene).isLoaded)
        //         yield return SceneManager.UnloadSceneAsync(currentLevels[_currentLevel].subLevels[_currentSublevel]
        //             .levelScene);
        //     else
        //         yield return null;
        // }
        //
        // public void TransitionToNextLevel(float delay)
        // {
        //     if (!_isTransitioning && currentCo == null)
        //     {
        //         currentCo = StartCoroutine(SetCurrentLevelTo(_currentLevel + 1, delay));
        //         _isTransitioning = true;
        //     }
        // }

        #endregion

        #region New Code

        public GameObject playerPrefab;
        [Header("Menu Settings")] public AudioClip menuMusic;

        [Header("Level Settings")] [Space] [Expose]
        public List<LevelSettings> currentLevelSettings;

        [Space] public UnityEvent onGameOver;
        public UnityEvent onGameCompletion;
        public UnityEvent onLevelTransitionEnter;
        public UnityEvent onLevelTransitionExit;

        private MusicPlayer _musicPlayer;
        private TimeDisplayer _timeDisplayer;
        private EnemyGenerator _enemyGenerator;

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

            _loadLevelCoroutine = StartCoroutine(LoadLevel(_currentLevel));
        }

        private void Update()
        {
            if (_timeDisplayer)
            {
                if (_timeDisplayer.TimeCounter.HasRanOutOfTime() ||
                    (!_player.gameObject.activeSelf && _player.HealthManager.IsFlaggedForDeath))
                {
                    OnGameOver();
                }
            }
        }

        private IEnumerator LoadLevel(int newLevelIndex)
        {
            onLevelTransitionEnter?.Invoke();
            _enemyGenerator.StopGenerating();


            yield return UnloadPreviousLevel(_currentLevel);
            _player.AbilityManager.DisruptAbilities();
            _player.AbilityManager.ResetCd();
            _player.AbilityManager.display.SetActive(false);
            _player.DisableController(false);

            if (newLevelIndex >= _selectedLevels.Count)
            {
                yield return OnGameComplete();
            }


            var selectedLevel = _selectedLevels[newLevelIndex];
            selectedLevel.FetchScene().SetSceneActive(true);
            _enemyGenerator.Generate(selectedLevel.numberOfEnemies);
            _musicPlayer.Play(selectedLevel.musicClip);
            _timeDisplayer.BeginCounting(selectedLevel.timeRemaining, selectedLevel.timerType);

            Vector3 pos = FetchGameObjectWithTagFromScene(selectedLevel.FetchScene(), selectedLevel.SpawnPos())
                .transform.position;
            _player.AbilityManager.AddAbility(currentLevelSettings[newLevelIndex].GetRandomAbility());
            yield return _player.TeleportPlayerToPos(pos);
            _player.AbilityManager.display.SetActive(true);


            onLevelTransitionExit?.Invoke();
            yield return FindAndSetup(newLevelIndex, "Level/Exit", "Level/Instructions");
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
                            foundDetector.ONTriggerEnter.AddListener((col) =>
                            {
                                if (col.gameObject.GetInstanceID() ==
                                    _player.gameObject.GetInstanceID())
                                    TransitionToNextLevel();
                            });
                            GameMaster.singletonAccess.ClearUpdateEvents();
                            _player.UpdateExitTracker(foundDetector);
                            break;

                        case "Level/Instruction":
                            if (foundDetector)
                                foundElement.GetComponent<TMP_Text>().text =
                                    $"Required possessions: {foundDetector.currentPossessionRequired}";

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

        private IEnumerator OnGameComplete()
        {
            onGameCompletion?.Invoke();
            yield break;
        }

        private void OnGameOver()
        {
            onGameOver?.Invoke();
        }

        private IEnumerator UnloadPreviousLevel(int currentLevel)
        {
            _selectedLevels[currentLevel].FetchScene().SetSceneActive(false);
            yield return new WaitForSeconds(1f);
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

            _musicPlayer = new GameObject("Music Player").AddComponent<MusicPlayer>();
            _timeDisplayer = new GameObject("Time Displayer").AddComponent<TimeDisplayer>();
            _enemyGenerator = new GameObject("Enemy Spawner").AddComponent<EnemyGenerator>();

            _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                .GetComponentInChildren<PlayerController>();
            _player.DisableController(false);
            yield return null;
        }

        private IEnumerator TransitionToMainMenu()
        {
            _mainMenu.SetSceneActive(true);
            _musicPlayer.Play(menuMusic);
            yield return new WaitForSeconds(1f);
        }


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
    }
}