using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Level.Asteroids;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Random = UnityEngine.Random;

public class AsteroidField : Generator
{
    // Start is called before the first frame update
    private Coroutine _generator;

    private PlayerController _playerController;

    public void SetTarget(PlayerController player)
    {
        _playerController = player;
    }
    
    public new void StopGenerating()
    {
        if (generator != null)
            StopCoroutine(generator);
        ClearEntities<Asteroid>();
    }

    protected override IEnumerator StartGenerating<T>(Scene currentScene, List<T> uniqueElements, float minSpawnRate,
        float maxSpawnRate,
        float spawnDistFromPlayer)
    {
        List<Asteroid> uniqueAsteroids = uniqueElements.OfType<Asteroid>().ToList();
        while (true)
        {
            Asteroid asteroid = ObjectPooler.DynamicInstantiate(
                uniqueAsteroids[Random.Range(0, uniqueAsteroids.Count)],
                SpawnAroundPlayer(currentScene, spawnDistFromPlayer), transform.rotation);


            asteroid.Rigidbody.AddForce(
                (_playerController.transform.position - asteroid.transform.position).normalized *
                Random.Range(15, 100), ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnRate, maxSpawnRate));
        }
    }

    public void Generate(Scene currentScene, List<Asteroid> uniqueAsteroids, float minAmm, float maxAmm,
        float spawnDistFromPlayer, PlayerController target)
    {
        SetTarget(target);
        Generate(currentScene, uniqueAsteroids, minAmm, maxAmm, spawnDistFromPlayer);
    }
}