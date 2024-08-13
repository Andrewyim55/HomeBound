using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Pathfinding pathFinding;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;

    [Header("Attributes")]
    [SerializeField] float spawnInterval = 5f;
    [SerializeField] private float spawnDistanceMin = 5f;
    [SerializeField] private float spawnDistanceMax = 10f;

    [Header("Spawn Chance Weights")]
    [SerializeField, Range(0, 10)] private float meleeSpawnFactor = 10f;
    [SerializeField, Range(0, 10)] private float rangedSpawnFactor = 10f;

    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        StartCoroutine(SpawnEnemyRoutine());
    }

    // Coroutine for spawning enemies at intervals
    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Spawn enemy at a random position around the player
    private void SpawnEnemy()
    {
        GameObject enemyToSpawn = ChooseEnemyType();
        Vector3 spawnPosition = GetSpawnPosition();
        Debug.Log(enemyToSpawn);
        Debug.Log(spawnPosition);
        Node spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
        Debug.Log("x: " + spawnNode.x + "y: " + spawnNode.y);
        while (spawnNode == null || spawnNode.nodeType != Node.NodeType.FLOOR)
        {
            spawnPosition = GetSpawnPosition();
            spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
        }
        Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
    }

    // Method to choose the enemy type based on spawn factors
    private GameObject ChooseEnemyType()
    {
        float totalWeight = meleeSpawnFactor + rangedSpawnFactor;
        float randomValue = Random.Range(0, totalWeight);

        if (randomValue < meleeSpawnFactor)
        {
            return meleeEnemyPrefab;
        }
        else
        {
            return rangedEnemyPrefab;
        }
    }

    // Get a random spawn position around player
    private Vector3 GetSpawnPosition()
    {
        float spawnDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        float spawnAngle = Random.Range(0f, 360f);
        Vector3 spawnDirection = new Vector3(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle), 0).normalized;
        Vector3 spawnPosition = playerTransform.position + spawnDirection * spawnDistance;
        return spawnPosition;
    }
}
