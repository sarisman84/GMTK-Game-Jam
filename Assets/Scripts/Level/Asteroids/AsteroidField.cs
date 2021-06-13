using System;
using System.Collections;
using System.Collections.Generic;
using Level.Asteroids;
using Managers;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

public class AsteroidField : MonoBehaviour
{
    public List<Asteroid> asteroidPrefabs;
    public Vector2 fieldSize;

    public float minSpawnRate, maxSpawnRate;
    // Start is called before the first frame update

    private void Awake()
    {
        StartCoroutine(SpawnAsteroids());
    }

    private IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            Asteroid asteroid = ObjectPooler.DynamicInstantiate(
                asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)], transform.position, transform.rotation);


            asteroid.Rigidbody.AddForce(
                (GameMaster.singletonAccess.playerObject.transform.position - asteroid.transform.position).normalized *
                Random.Range(15, 100), ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(fieldSize.x, 4, fieldSize.y));
    }
}