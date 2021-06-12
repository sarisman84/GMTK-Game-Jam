using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EnemyGenerator : MonoBehaviour
    {
        public List<BaseEnemy> numberOfEnemies = new();
        public float spawnDistanceFromPlayer = 20f;
        public float minSpawnRate = 0.5f, maxSpawnRate = 2f;

        public void Generate()
        {
            StartCoroutine(InstantiateEnemiesOverTime());
        }


        private IEnumerator InstantiateEnemiesOverTime()
        {
            while (true)
            {
                ObjectPooler.DynamicInstantiate(numberOfEnemies[PickARandomEnemy()], SpawnAroundPlayer(),
                    Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
            }
        }

        private Vector3 SpawnAroundPlayer()
        {
            Vector3 onEdgeOfSphere = Random.onUnitSphere;
            onEdgeOfSphere = new Vector3(onEdgeOfSphere.x, 0, onEdgeOfSphere.z);
            if (GameMaster.SingletonAccess.GetPlayer() is { } player)
            {
                return player.transform.position + onEdgeOfSphere * spawnDistanceFromPlayer;
            }

            return onEdgeOfSphere * spawnDistanceFromPlayer;
        }

        private void OnDrawGizmos()
        {
            if (GameMaster.SingletonAccess.GetPlayer() is { } player)
            {
                Gizmos.DrawWireSphere(player.transform.position, spawnDistanceFromPlayer);
            }
        }


        private int PickARandomEnemy()
        {
            return Random.Range(0, numberOfEnemies.Count);
        }
    }
}