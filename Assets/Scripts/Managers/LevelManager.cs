using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using General;
using Managers;
using Player;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Utility;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility.Attributes;


namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public TMP_Text _timerDisplayer;
        [Expose] public List<LevelSettings> currentLevels;
        public UnityEvent ONGameOver;

        private int _currentLevel;
        private int _currentSublevel;
        private bool _isTransitioning;

        private Coroutine currentCo;

        private bool _hasTimeRanOut = false;
        private bool _showTimer = false;
        private float _currentCountdown = float.MaxValue;
        private bool hasStarted = false;


        public void StartGame()
        {
            if (!hasStarted)
            {
                if (!_timerDisplayer)
                    throw new NullReferenceException(
                        $"Missing Time Displayer. Check the Inspector of {gameObject.name}");
                _currentLevel = 0;
                if (!GameMaster.singletonAccess.playerObject)
                {
                    GameMaster.singletonAccess.RegisterPlayerScene(SceneManager.CreateScene("Player Scene"));
                    SceneManager.SetActiveScene(GameMaster.singletonAccess.playerScene);
                    GameMaster.singletonAccess.InitializePlayer(playerPrefab,
                        Vector3.zero);
                    GameMaster.singletonAccess.ONPlayerGameOver += () => StartCoroutine(OnGameOver());
                }

                currentCo = StartCoroutine(SetCurrentLevelTo(_currentLevel, 0.5f));
                hasStarted = true;
            }
        }

        private void Update()
        {
            _hasTimeRanOut = CountdownTime();

            _showTimer = (_currentCountdown <= 61 ||
                          currentLevels[_currentLevel].subLevels[_currentSublevel].countdownType ==
                          LevelSettings.CountdownType.ToNextStage) &&
                         GameMaster.singletonAccess.playerObject.activeSelf && !_hasTimeRanOut;

            if (_showTimer)
                DisplayCountdown();
            else
                _timerDisplayer.text = "";
        }

        private bool CountdownTime()
        {
            _currentCountdown -= Time.deltaTime;
            return _currentCountdown <= 0;
        }


        private bool HasTimeRanOut(int newLevel, int newSubLevel, LevelSettings.CountdownType type)
        {
            LevelSettings settings = currentLevels[newLevel];

            LevelSettings.Level level = settings.subLevels[newSubLevel];
            return level.countdownType == type && _hasTimeRanOut &&
                   GameMaster.singletonAccess.playerObject.activeSelf;
        }

        public Vector2Int DisplayCountdown()
        {
            int minutes = Mathf.FloorToInt(_currentCountdown / 60f);
            int seconds = Mathf.FloorToInt(_currentCountdown % 60f);
            _timerDisplayer.text = $"{minutes:00}:{seconds:00}";
            return new Vector2Int(minutes, seconds);
        }


        private IEnumerator SetCurrentLevelTo(int newLevel, float delay)
        {
            yield return ResetPreviousLevel();
            GameMaster.singletonAccess.playerCompass.DisableTracker = true;
            GameMaster.singletonAccess.possessor.SetPossessionsActive(false);
            GameMaster.singletonAccess.playerObject.SetActive(false);
         
            int newSubLevel = 0;
            yield return new WaitForSeconds(delay);
            if (newLevel < currentLevels.Count)
            {
                newSubLevel = currentLevels[newLevel].SelectRandom();
                _currentCountdown = currentLevels[newLevel].timeRemaining;
            }

            if (newLevel >= currentLevels.Count)
            {
                yield return OnGameOver();
                _isTransitioning = false;
                yield break;
            }

            AsyncOperation op = SceneManager.LoadSceneAsync(
                $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}",
                LoadSceneMode.Additive);

            op.allowSceneActivation = true;
            yield return new WaitUntil(() => op.isDone && SceneManager.GetSceneByName(
                $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}").isLoaded);
            SceneManager.SetActiveScene(
                SceneManager.GetSceneByName(
                    $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}"));

            Transform potentialPos =
                GetGameObjectFromActiveSceneWithTag(currentLevels[_currentLevel].SpawnPos(_currentSublevel), true);

            Vector3 foundPosition = potentialPos ? potentialPos.position : Vector3.zero;

            GameObject exit = GetGameObjectFromActiveSceneWithTag("Level/Exit", true)?.gameObject;
            if (exit)
            {
                Detector detector = exit.GetComponent<Detector>();
                detector.ONTriggerEnter.RemoveAllListeners();
                detector.additionalPossessionsRequired += GameMaster.singletonAccess.possessor.possessedEntities.Count;
                detector.ONTriggerEnter.AddListener((col) =>
                {
                    if (col.gameObject.GetInstanceID() == GameMaster.singletonAccess.playerObject.GetInstanceID())
                        TransitionToNextLevel(0.5f);
                });
                GameMaster.singletonAccess.ClearUpdateEvents();
                GameMaster.singletonAccess.ONUpdate += () =>
                {
                    if (detector)
                        detector.gameObject.SetActive(GameMaster.singletonAccess.possessor.possessedEntities.Count >=
                                                      detector.additionalPossessionsRequired);
                };
            }

            GetComponentFromScene<EnemyGenerator>()?.Generate();
            
            GameMaster.singletonAccess.possessor.SetPossessionsActive(true);
            GameMaster.singletonAccess.playerObject.gameObject.SetActive(true);
            GameMaster.singletonAccess.playerObject.transform.position = foundPosition;
            GameMaster.singletonAccess.possessor.TeleportPossessionsToPosition(foundPosition);
            GameMaster.singletonAccess.abilityManager.AddAbility(currentLevels[newLevel].GetRandomAbility());
            GameMaster.singletonAccess.playerCompass.DisableTracker =
                currentLevels[newLevel].subLevels[newSubLevel].countdownType == LevelSettings.CountdownType.ToNextStage;
            _currentLevel = newLevel;
            _isTransitioning = false;
            currentCo = null;
            while (true)
            {
                if (GameMaster.singletonAccess.playerObject &&
                    GameMaster.singletonAccess.playerObject.GetComponent<HealthModifier>() is { } healthModifier &&
                    healthModifier.IsFlaggedForDeath && !GameMaster.singletonAccess.playerObject.activeSelf ||
                    HasTimeRanOut(_currentLevel, _currentSublevel, LevelSettings.CountdownType.ToGameOver))
                {
                    yield return OnGameOver();
                    _hasTimeRanOut = true;
                    _currentCountdown = 0;
                    GameMaster.singletonAccess.playerCompass.DisableTracker = true;
                    yield break;
                }

                if (HasTimeRanOut(_currentLevel, _currentSublevel, LevelSettings.CountdownType.ToNextStage))
                {
                    TransitionToNextLevel(0.5f);
                    yield break;
                }


                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator OnGameOver()
        {
            yield return ResetPreviousLevel();
            GameMaster.singletonAccess.possessor.ResetPossessions();
            GameMaster.singletonAccess.playerHealth.DestroyThis();
            GameMaster.singletonAccess.playerWeaponManager.ResetWeaponLibrary();
            GameMaster.singletonAccess.abilityManager.ManualReset();
      
            GameMaster.singletonAccess.playerCompass.DisableTracker = true;
            ONGameOver?.Invoke();
            hasStarted = false;
            _isTransitioning = false;
            if (currentCo != null)
                StopCoroutine(currentCo);
        }

        public static Transform GetGameObjectFromActiveSceneWithTag(string inputTag, bool debug = false)
        {
            if (debug)
            {
                Debug.Log($"Searching for GameObjects in {SceneManager.GetActiveScene().name}");
            }

            foreach (var rootObj in SceneManager.GetActiveScene().GetRootGameObjects().ToList())
            {
                if (rootObj)
                {
                    Transform child = rootObj.transform.tag.Contains(inputTag) ? rootObj.transform : null;
                    if (!child)
                        child = rootObj.transform.FindChildOfTag(inputTag);
                    if (child)
                    {
                        if (debug)
                            Debug.Log($"Found a gameObject with the Inputed Tag {inputTag}. Returning!");
                        return child;
                    }
                }
            }

            return null;
        }

        private T GetComponentFromScene<T>()
        {
            foreach (var rootObj in SceneManager.GetActiveScene().GetRootGameObjects().ToList())
            {
                if (rootObj)
                {
                    T child = rootObj.GetComponentInChildren<T>();

                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return default;
        }

        private IEnumerator ResetPreviousLevel()
        {
            if (SceneManager.GetSceneByName(currentLevels[_currentLevel].subLevels[_currentSublevel]
                .levelScene).isLoaded)
                yield return SceneManager.UnloadSceneAsync(currentLevels[_currentLevel].subLevels[_currentSublevel]
                    .levelScene);
            else
                yield return null;
        }

        public void TransitionToNextLevel(float delay)
        {
            if (!_isTransitioning && currentCo == null)
            {
                currentCo = StartCoroutine(SetCurrentLevelTo(_currentLevel + 1, delay));
                _isTransitioning = true;
            }
        }
    }
}