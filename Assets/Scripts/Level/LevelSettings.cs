using System;
using System.Collections.Generic;
using Enemies;
using Player.HUD.Abilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Level
{
    [CreateAssetMenu(fileName = "New Level Settings", menuName = "GMTK/Levels/Create Level", order = 0)]
    public class LevelSettings : ScriptableObject
    {
        public enum EndDirection
        {
            West,
            East,
            North,
            South,
            Center
        }

        public List<Level> levelsToUse;
        public List<Ability> abilitiesToGiveToPlayerOnLevelEntry;


        public Level SelectRandom()
        {
            return levelsToUse[Random.Range(0, levelsToUse.Count)];
        }

        

        public Ability GetRandomAbility()
        {
            if (abilitiesToGiveToPlayerOnLevelEntry.Count == 0) return null;
            return abilitiesToGiveToPlayerOnLevelEntry[Random.Range(0, abilitiesToGiveToPlayerOnLevelEntry.Count)];
        }

        [Serializable]
        public struct Level
        {
            [Header("Music Settings")] public AudioClip musicClip;

            [Header("Enemy Spawn Settings")] public List<BaseEnemy> numberOfEnemies;
            public float spawnDistanceFromPlayer;
            public float minSpawnRate, maxSpawnRate;

            [Header("Settings")] public float timeRemaining;
            public string levelScene;
            public EndDirection levelExitDirection;
            public CountdownType timerType;

            public Scene FetchScene()
            {
                return SceneManager.GetSceneByName(levelScene);
            }
            
            public string SpawnPos()
            {
                string baseTag = "Level/Spawn/";
                switch (levelExitDirection)
                {
                    case EndDirection.West:
                        return baseTag + "West";
                    case EndDirection.East:
                        return baseTag + "East";
                    case EndDirection.North:
                        return baseTag + "North";
                    case EndDirection.South:
                        return baseTag + "South";
                    case EndDirection.Center:
                        return baseTag + "Center";
                }

                return "";
            }
        }

        public enum CountdownType
        {
            GameOverOnZero,
            ProgressOnZero
        }
    }
}