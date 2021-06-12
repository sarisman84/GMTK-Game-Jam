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
                GameMaster.SingletonAccess.InitializePlayer(playerPrefab.GetComponentInChildren<PlayerController>(),
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

            _currentSublevel = currentLevels[newLevel].SelectRandom();
            AsyncOperation op = SceneManager.LoadSceneAsync(
                $"Scenes/Levels/{currentLevels[_currentLevel].subLevels[_currentSublevel].levelScene}",
                LoadSceneMode.Additive);
            op.allowSceneActivation = true;
            yield return new WaitUntil(() => op.isDone);


            GameObject potentialPos = SceneManager.GetActiveScene().GetRootGameObjects().ToList()
                .Find(g => g.CompareTag(currentLevels[_currentLevel].SpawnPos(_currentSublevel)));
            Vector3 foundPosition = potentialPos ? potentialPos.transform.position : Vector3.zero;


            GameMaster.SingletonAccess.GetPlayer().gameObject.SetActive(true);
            GameMaster.SingletonAccess.GetPlayer().transform.position = foundPosition;


            _currentLevel = newLevel;
            _isTransitioning = false;
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