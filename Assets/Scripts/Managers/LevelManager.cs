using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using General;
using Managers;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Utility;
using UnityEngine.SceneManagement;
using Utility.Attributes;


namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        [Expose] public List<LevelSettings> currentLevels;
        public UnityEvent ONGameOver;

        private int _currentLevel;
        private int _currentSublevel;
        private bool _isTransitioning;

        private Coroutine currentCo;


        public void StartGame()
        {
            _currentLevel = 0;
            if (!GameMaster.SingletonAccess.PlayerObject)
            {
                GameMaster.SingletonAccess.RegisterPlayerScene(SceneManager.CreateScene("Player Scene"));
                SceneManager.SetActiveScene(GameMaster.SingletonAccess.PlayerScene);
                GameMaster.SingletonAccess.InitializePlayer(playerPrefab,
                    Vector3.zero);
                GameMaster.SingletonAccess.ONPlayerGameOver += () => StartCoroutine(OnGameOver());
            }

            currentCo = StartCoroutine(SetCurrentLevelTo(_currentLevel, 2f));
        }


        private IEnumerator SetCurrentLevelTo(int newLevel, float delay)
        {
            yield return ResetPreviousLevel();
            GameMaster.SingletonAccess.Possessor.SetPossessionsActive(false);
            GameMaster.SingletonAccess.PlayerObject.SetActive(false);


            yield return new WaitForSeconds(delay);
            if (newLevel >= currentLevels.Count)
            {
                yield return OnGameOver();
                _isTransitioning = false;
                yield break;
            }


            int newSubLevel = currentLevels[newLevel].SelectRandom();
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
                GetGameObjectFromSceneOfTag(currentLevels[_currentLevel].SpawnPos(_currentSublevel), true);
            Vector3 foundPosition = potentialPos ? potentialPos.position : Vector3.zero;

            GameObject exit = GetGameObjectFromSceneOfTag("Level/Exit", true)?.gameObject;
            if (exit)
            {
                Detector detector = exit.GetComponent<Detector>();
                detector.ONTriggerEnter.RemoveAllListeners();
                detector.ONTriggerEnter.AddListener((col) =>
                {
                    if (col.gameObject.GetInstanceID() == GameMaster.SingletonAccess.PlayerObject.GetInstanceID())
                        TransitionToNextLevel(2f);
                });
                GameMaster.SingletonAccess.ClearUpdateEvents();
                GameMaster.SingletonAccess.ONUpdate += () =>
                {
                    if (detector)
                        detector.gameObject.SetActive(GameMaster.SingletonAccess.Possessor.possessedEntities.Count >=
                                                      detector.requiredAmmToEnableDetector);
                };
            }

            GetComponentFromScene<EnemyGenerator>()?.Generate();

            GameMaster.SingletonAccess.Possessor.SetPossessionsActive(true);
            GameMaster.SingletonAccess.PlayerObject.gameObject.SetActive(true);
            GameMaster.SingletonAccess.PlayerObject.transform.position = foundPosition;
            GameMaster.SingletonAccess.Possessor.TeleportPossessionsToPosition(foundPosition);

            _currentLevel = newLevel;
            _isTransitioning = false;
            currentCo = null;
            
            while (true)
            {
                if (GameMaster.SingletonAccess.PlayerObject &&
                    GameMaster.SingletonAccess.PlayerObject.GetComponent<HealthModifier>() is { } healthModifier &&
                    healthModifier.IsFlaggedForDeath && !GameMaster.SingletonAccess.PlayerObject.activeSelf)
                {
                    yield return OnGameOver();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator OnGameOver()
        {
            yield return ResetPreviousLevel();
            GameMaster.SingletonAccess.Possessor.ResetPossessions();
            ONGameOver?.Invoke();
            _isTransitioning = false;
            if (currentCo != null)
                StopCoroutine(currentCo);
        }

        private Transform GetGameObjectFromSceneOfTag(string inputTag, bool debug = false)
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