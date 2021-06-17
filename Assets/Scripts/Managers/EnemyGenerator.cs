using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using General;
using Level;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EnemyGenerator : Generator
    {
        private IEnumerator InstantiateEnemiesOverTime()
        {
            while (true)
            {
                // BaseEnemy enemy = ObjectPooler.DynamicInstantiate(numberOfEnemies[PickARandomEnemy()], _spawnPos,
                //     Quaternion.identity);
                // enemy.GetComponent<HealthModifier>().onDeath.AddListener(() =>
                //     GameMaster.singletonAccess.possessor.AdditionToCurrentKillCount = 1);
                yield return new WaitForSeconds(0);
            }
        }

        private HealthModifier _target;

        public void SetTarget(HealthModifier target)
        {
            _target = target;
        }


        // private void OnDrawGizmos()
        // {
        //     if (GameMaster.singletonAccess.playerObject is { } player)
        //     {
        //         Gizmos.color = Color.red;
        //         Gizmos.DrawWireSphere(player.transform.position, spawnDistanceFromPlayer);
        //
        //         Gizmos.color = Color.magenta;
        //         Gizmos.DrawSphere(_spawnPos, 1);
        //     }
        // }
        //
        //
        // private int PickARandomEnemy()
        // {
        //     return Random.Range(0, numberOfEnemies.Count);
        // }


        protected override IEnumerator StartGenerating<T>(PlayerController playerController, Scene currentScene,
            List<T> uniqueElements,
            float minSpawnRate, float maxSpawnRate, float spawnDistFromPlayer)
        {
            List<BaseEnemy> uniqueEnemyList = uniqueElements.OfType<BaseEnemy>().ToList();
            while (true)
            {
                BaseEnemy chosenEnemy =
                    ObjectPooler.DynamicInstantiate(uniqueEnemyList[Random.Range(0, uniqueEnemyList.Count)],
                        SpawnAroundPlayer(currentScene, playerController, spawnDistFromPlayer), Quaternion.identity);
                chosenEnemy.target = _target.gameObject;
                chosenEnemy.ONOverridingDeathEvent +=
                    () =>
                    {
                        if (_target.GetComponent<PlayerController>() is { } player)
                            if (chosenEnemy.WeaponManager.CurTarget == typeof(PlayerController))
                                player.PossessionManager.AdditionToCurrentKillCount = 1;
                    };


                yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
            }
        }

        public new void StopGenerating()
        {
            if (generator != null)
                StopCoroutine(generator);
            ClearEntities<BaseEnemy>();
        }

        public void Generate(PlayerController playerController, Scene currentScene, List<BaseEnemy> numberOfEnemies,
            float minAmm, float maxAmm,
            float spawnDistFromPlayer)
        {
            SetTarget(playerController.HealthManager);
            base.Generate(playerController, currentScene, numberOfEnemies, minAmm, maxAmm, spawnDistFromPlayer);
        }
    }
}