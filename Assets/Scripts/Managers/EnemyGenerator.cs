using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using General;
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

        private Vector3 _spawnPos;

        public void Generate()
        {
            StartCoroutine(InstantiateEnemiesOverTime());
        }

        private void Update()
        {
            _spawnPos = SpawnAroundPlayer();
        }


        private IEnumerator InstantiateEnemiesOverTime()
        {
            while (true)
            {
                BaseEnemy enemy = ObjectPooler.DynamicInstantiate(numberOfEnemies[PickARandomEnemy()], _spawnPos,
                    Quaternion.identity);
                enemy.GetComponent<HealthModifier>().onDeath.AddListener(() =>
                    GameMaster.singletonAccess.possessor.AdditionToCurrentKillCount = 1);
                yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
            }
        }

        private Vector3 SpawnAroundPlayer()
        {
            Vector3 onEdgeOfSphere = Vector3.zero;

            bool runOnce = true;
            while (PositionIsInvalid(onEdgeOfSphere) || runOnce)
            {
                if (GameMaster.singletonAccess.playerObject is { } player)
                {
                    Vector2 foundResult = Random.insideUnitCircle.normalized * spawnDistanceFromPlayer;
                    onEdgeOfSphere = player.transform.position + new Vector3(foundResult.x, 0, foundResult.y);
                    runOnce = false;
                }
                else
                    break;
            }

            return onEdgeOfSphere;
        }

        private bool PositionIsInvalid(Vector3 onEdgeOfSphere)
        {
            Transform outOfBoundsBox = LevelManager.GetGameObjectFromActiveSceneWithTag("Level/OutOfBounds");

            if (!outOfBoundsBox) return false;
            Collider[] foundColliders = outOfBoundsBox.GetComponents<Collider>();

            if (foundColliders == null) return false;
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
            if (GameMaster.singletonAccess.playerObject is { } player)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.transform.position, spawnDistanceFromPlayer);

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(_spawnPos, 1);
            }
        }


        private int PickARandomEnemy()
        {
            return Random.Range(0, numberOfEnemies.Count);
        }
    }
}