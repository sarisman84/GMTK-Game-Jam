using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Player;
using UnityEngine;
using Utility;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        public PlayerController playerPrefab;
        public List<LevelSettings> currentLevels;
        private readonly Dictionary<LevelSettings, List<GameObject>> _spawnedLevels = new();
        private int currentLevel = 0;
        private bool _isTransitioning;


        public void InitializeLevels()
        {
            foreach (var current in currentLevels)
            {
                _spawnedLevels.Add(current, current.InitializePrefabs());
            }
        }

        public void StartGame()
        {
            InitializeLevels();
            StartCoroutine(SetCurrentLevelTo(currentLevel, 2f));
        }

        private IEnumerator SetCurrentLevelTo(int newLevel, float delay)
        {
            GameMaster.SingletonAccess.GetPlayer()?.gameObject.SetActive(false);
            ResetPreviousLevels();

            yield return new WaitForSeconds(delay);

            if (newLevel >= currentLevels.Count) yield break;

            int subLevelIndex = currentLevels[newLevel].SelectRandom();
            GameObject level = _spawnedLevels[currentLevels[newLevel]][subLevelIndex];
            level.SetActive(true);
            Vector3 spawnPos = level.transform.FindChildOfTag(currentLevels[currentLevel].SpawnPos()).position;

            if (!GameMaster.SingletonAccess.GetPlayer())
            {
                GameMaster.SingletonAccess.InitializePlayer(playerPrefab, spawnPos);
            }
            else
            {
                GameMaster.SingletonAccess.GetPlayer().gameObject.SetActive(true);
                GameMaster.SingletonAccess.GetPlayer().transform.position = spawnPos;
            }


            currentLevel = newLevel;
            _isTransitioning = false;
        }

        private void ResetPreviousLevels()
        {
            foreach (var listOfSublevels in _spawnedLevels)
            {
                foreach (var level in listOfSublevels.Value)
                {
                    level.SetActive(false);
                }
            }
        }

        public void TransitionToNextLevel(float delay)
        {
            if (!_isTransitioning)
            {
                StartCoroutine(SetCurrentLevelTo(currentLevel + 1, delay));
                _isTransitioning = true;
            }
        }
    }
}