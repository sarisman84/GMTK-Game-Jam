using System.Collections;
using System.Collections.Generic;
using Level;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public abstract class Generator : MonoBehaviour
    {
        protected Coroutine generator;

        public virtual void Generate<T>(PlayerController player, Scene currentScene, List<T> uniqueElements, float minSpawnRate,
            float maxSpawnRate, float spawnDistFromPlayer) where T : MonoBehaviour
        {
            StopGenerating();
            generator = StartCoroutine(StartGenerating(player,currentScene, uniqueElements, minSpawnRate, maxSpawnRate,
                spawnDistFromPlayer));
        }

        protected abstract IEnumerator StartGenerating<T>(PlayerController playerController, Scene currentScene,
            List<T> uniqueElements,
            float minSpawnRate, float maxSpawnRate, float spawnDistFromPlayer) where T : MonoBehaviour;

        public void StopGenerating()
        {
            if (generator != null)
                StopCoroutine(generator);
        }

        protected bool PositionIsInvalid(Vector3 onEdgeOfSphere, Scene currentScene)
        {
            Transform outOfBoundsBox = LevelManager.FetchGameObjectWithTagFromScene(currentScene, "Level/OutOfBounds")
                ?.transform;

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

        public Vector3 SpawnAroundPlayer(Scene currentScene, PlayerController player, float spawnDistanceFromPlayer)
        {
            Vector2 foundResult = Random.insideUnitCircle.normalized * spawnDistanceFromPlayer;
            Vector3 onEdgeOfSphere = player.transform.position + new Vector3(foundResult.x, 0, foundResult.y);

            if (PositionIsInvalid(onEdgeOfSphere, currentScene))
            {
                onEdgeOfSphere = SpawnAroundPlayer(currentScene, player, spawnDistanceFromPlayer);
            }


            return onEdgeOfSphere;
        }

        public void ClearEntities<T>() where T : MonoBehaviour
        {
            foreach (var foundType in FindObjectsOfType<T>())
            {
                foundType.gameObject.SetActive(false);
            }
        }
    }
}