using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] public GameObject bossEnemyPrefab;
    [SerializeField] private float timeUntilSpawn;
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    // private List<EnemyController> _childEnemies = new List<EnemyController>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        TimeManager.Instance.MinuteElapsed += DifficultyRamp;
    }

    // Basically just decrease the spawning delay every 5 minutes
    void DifficultyRamp(int minutes)
    {
        if (minutes % 2 != 0) return;
        maxSpawnDelay = maxSpawnDelay > 10 ? maxSpawnDelay - 5 : maxSpawnDelay;
        if (minutes % 5 != 0) return;
        // Have a 10% Chance of Spawning a boss enemy
        if (Random.Range(1, 100) <= 10) Instantiate(bossEnemyPrefab, transform.position, Quaternion.identity);
    }

    void Start()
    {
        timeUntilSpawn = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilSpawn -= Time.deltaTime;

        if (timeUntilSpawn <= 0)
        {
            timeUntilSpawn = Random.Range(0, maxSpawnDelay);
            var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            // _childEnemies.Add(enemy.GetComponent<EnemyController>());
        }
    }
}
