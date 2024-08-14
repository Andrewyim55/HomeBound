﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Pathfinding pathFinding;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<GameObject> enemyPrefab;

    [Header("Attributes")]
    [SerializeField] float spawnInterval = 2f;
    [SerializeField] private float spawnDistanceMin = 5f;
    [SerializeField] private float spawnDistanceMax = 10f;

    private Transform playerTransform;
    private int totalWeight;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        totalWeight = 0;
        // Get the total weightage for the enemy spawning
        for (int i = 0; i < enemyPrefab.Count; i++)
        {
            totalWeight += enemyPrefab[i].GetComponent<Enemy>().GetSpawnWeight();
        }
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
        Node spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
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
        int spawnNum = Random.Range(0, totalWeight);
        Debug.Log(spawnNum);
        int currentWeight = 0;
        for (int i = 0; i < enemyPrefab.Count; i++)
        {
            currentWeight += enemyPrefab[i].GetComponent<Enemy>().GetSpawnWeight();

            if (spawnNum < currentWeight)
            {
                return enemyPrefab[i];
            }
        }
        return enemyPrefab[enemyPrefab.Count - 1];
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
