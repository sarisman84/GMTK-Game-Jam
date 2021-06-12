using System;
using System.Collections.Generic;
using UnityEngine;
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

        public string levelName;
        public List<GameObject> levelPrefabs;
        public float timeRemainingBeforeGameOver;
        public EndDirection endDirection;

        public List<GameObject> InitializePrefabs()
        {
            List<GameObject> initializedLevels = new List<GameObject>();
            foreach (var level in levelPrefabs)
            {
                GameObject clone = Instantiate(level, Vector3.zero, Quaternion.identity);
                clone.SetActive(false);
                initializedLevels.Add(clone);
            }

            return initializedLevels;
        }

        public int SelectRandom()
        {
            return Random.Range(0, levelPrefabs.Count);
        }

        public string SpawnPos()
        {
            switch (endDirection)
            {
                case EndDirection.West:
                    return "WestSpawn";
                case EndDirection.East:
                    return "EastSpawn";
                case EndDirection.North:
                    return "NorthSpawn";
                case EndDirection.South:
                    return "SouthSpawn";
                case EndDirection.Center:
                    return "CenterSpawn";
            }

            return "";
        }
    }
}