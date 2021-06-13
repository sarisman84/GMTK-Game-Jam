using System;
using System.Collections.Generic;
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

        public string levelName;
        public List<Level> subLevels;
        public float timeRemaining;
        public List<Ability> abilitiesToGiveToPlayerOnLevelEntry;


        public int SelectRandom()
        {
            return Random.Range(0, subLevels.Count);
        }

        public string SpawnPos(int subIndex)
        {
            string baseTag = "Level/Spawn/";
            switch (subLevels[subIndex].endDirection)
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

        public Ability GetRandomAbility()
        {
            return abilitiesToGiveToPlayerOnLevelEntry[Random.Range(0, abilitiesToGiveToPlayerOnLevelEntry.Count)];
        }

        [Serializable]
        public struct Level
        {
            public string levelScene;
            public EndDirection endDirection;
            public CountdownType countdownType;
        }

        public enum CountdownType
        {
            ToGameOver,
            ToNextStage
        }
    }
}