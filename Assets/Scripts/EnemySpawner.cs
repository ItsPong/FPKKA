using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject goblinPrefab;
    public GameObject skeletonPrefab;
    public GameObject darkBanditPrefab;
    public GameObject lightBanditPrefab;
    public Transform player;
    public float spawnInterval = 5f;
    public float spawnRadius = 10f;
    public float minSpawnDistance = 3f;
    public int initialMaxEnemies = 5;
    public float increaseMaxEnemiesInterval = 30f;

    private float spawnTimer;
    private float maxEnemiesIncreaseTimer;
    private float lightBanditSpawnTimer = 240f;
    private float darkBanditSpawnTimer = 300f;
    private int maxEnemies;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public float minX = -24f;
    public float maxX = 24f;
    public float minY = -15f;
    public float maxY = 14f;

    void Start()
    {
        spawnTimer = spawnInterval;
        maxEnemiesIncreaseTimer = increaseMaxEnemiesInterval;
        maxEnemies = initialMaxEnemies;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        maxEnemiesIncreaseTimer -= Time.deltaTime;
        lightBanditSpawnTimer -= Time.deltaTime;
        darkBanditSpawnTimer -= Time.deltaTime;

        if (maxEnemiesIncreaseTimer <= 0f)
        {
            maxEnemies += 2;
            spawnInterval = Mathf.Max(1f, spawnInterval - 2f);
            maxEnemiesIncreaseTimer = increaseMaxEnemiesInterval;
        }

        if (spawnTimer <= 0f && activeEnemies.Count < maxEnemies)
        {
            SpawnRandomEnemy();
            spawnTimer = spawnInterval;
        }

        if (lightBanditSpawnTimer <= 0f)
        {
            SpawnSpecificEnemy(lightBanditPrefab);
            lightBanditSpawnTimer = 240f;
        }

        if (darkBanditSpawnTimer <= 0f)
        {
            SpawnSpecificEnemy(darkBanditPrefab);
            darkBanditSpawnTimer = 300f;
        }

        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    private void SpawnRandomEnemy()
    {
        GameObject enemyPrefab = Random.value < 0.65f ? goblinPrefab : skeletonPrefab;
        Vector2 spawnPosition = GetRandomSpawnPosition();

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    private void SpawnSpecificEnemy(GameObject enemyPrefab)
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnPosition;

        do
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            spawnPosition = new Vector2(randomX, randomY);

        } while (Vector2.Distance(spawnPosition, player.position) < minSpawnDistance);

        return spawnPosition;
    }

}
