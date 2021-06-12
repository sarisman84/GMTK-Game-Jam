using System;
using System.Collections.Generic;
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

        public string levelName;
        public List<Level> subLevels;
        public float timeRemainingBeforeGameOver;


        public int SelectRandom()
        {
            return Random.Range(0, subLevels.Count);
        }

        public string SpawnPos(int subIndex)
        {
            switch (subLevels[subIndex].endDirection)
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

        [Serializable]
        public struct Level
        {
            public string levelScene;
            public EndDirection endDirection;
        }
    }
}