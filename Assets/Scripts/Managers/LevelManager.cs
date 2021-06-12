using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Player;
using UnityEngine;
using Utility;
using UnityEngine.SceneManagement;


namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public List<LevelSettings> currentLevels;
        private int _currentLevel;
        private int _currentSublevel;
        private bool _isTransitioning;

        private void Awake()
        {
            StartGame();
        }

        public void StartGame()
        {
            if (!GameMaster.SingletonAccess.GetPlayer())
            {
                SceneManager.SetActiveScene(SceneManager.CreateScene("Player Scene"));
                GameMaster.SingletonAccess.InitializePlayer(playerPrefab,
                    Vector3.zero);
            }

            StartCoroutine(SetCurrentLevelTo(_currentLevel, 2f));
        }

        private IEnumerator SetCurrentLevelTo(int newLevel, float delay)
        {
            yield return ResetPreviousLevel();
            GameMaster.SingletonAccess.GetPlayer()?.gameObject.SetActive(false);


            yield return new WaitForSeconds(delay);

            if (newLevel >= currentLevels.Count) yield break;

            int newSubLevel = currentLevels[newLevel].SelectRandom();
            AsyncOperation op = SceneManager.LoadSceneAsync(
                $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}",
                LoadSceneMode.Additive);
            op.allowSceneActivation = true;

            yield return new WaitUntil(() => op.isDone);

            SceneManager.SetActiveScene(
                SceneManager.GetSceneByName(
                    $"Scenes/Levels/{currentLevels[newLevel].subLevels[newSubLevel].levelScene}"));
            Transform potentialPos =
                GetGameObjectFromSceneOfTag(currentLevels[_currentLevel].SpawnPos(_currentSublevel), true);
            Vector3 foundPosition = potentialPos ? potentialPos.position : Vector3.zero;

            GameObject exit = GetGameObjectFromSceneOfTag("Level/Exit", true)?.gameObject;
            if (exit)
            {
                Detector dectector = exit.GetComponent<Detector>();
                dectector.ONTriggerEnter.RemoveAllListeners();
                dectector.ONTriggerEnter.AddListener((col) =>
                {
                    if (col.gameObject.GetInstanceID() == GameMaster.SingletonAccess.GetPlayer().GetInstanceID())
                        TransitionToNextLevel(2f);
                });
            }

            GetComponentFromScene<EnemyGenerator>()?.Generate();

            GameMaster.SingletonAccess.GetPlayer().gameObject.SetActive(true);
            GameMaster.SingletonAccess.GetPlayer().transform.position = foundPosition;

            _currentLevel = newLevel;
            _isTransitioning = false;
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
            if (!_isTransitioning)
            {
                StartCoroutine(SetCurrentLevelTo(_currentLevel + 1, delay));
                _isTransitioning = true;
            }
        }
    }
}