using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] private float timeUntilSpawn;
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
