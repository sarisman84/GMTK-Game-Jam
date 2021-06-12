using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using Level;
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
            Vector3 onEdgeOfSphere = Vector3.zero;

            while (PositionIsInvalid(onEdgeOfSphere))
            {
                if (GameMaster.SingletonAccess.PlayerObject is { } player)
                {
                    Vector3 foundResult = Random.onUnitSphere;
                    onEdgeOfSphere = player.transform.position +
                                     Vector3.ClampMagnitude(
                                         new Vector3(foundResult.x, 0, foundResult.z),
                                         spawnDistanceFromPlayer);
                }
                else
                    break;
            }

            return onEdgeOfSphere;
        }

        private bool PositionIsInvalid(Vector3 onEdgeOfSphere)
        {
            Transform outOfBoundsBox = LevelManager.GetGameObjectFromSceneOfTag("Level/OutOfBounds");


            Collider[] foundColliders = outOfBoundsBox.GetComponents<Collider>();

            foreach (var foundCollider in foundColliders)
            {
                if (foundCollider.bounds.Contains(onEdgeOfSphere))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (GameMaster.SingletonAccess.PlayerObject is { } player)
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