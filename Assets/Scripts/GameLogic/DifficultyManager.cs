using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner; // Reference to the EnemySpawner script

    [Header("Spawn Timing and Difficulty")]
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.2f;
    [SerializeField] private float spawnIntervalStep = 0.3f;
    [SerializeField] private float spawnIntervalReductionRate = 30;
    [SerializeField] private float gameDuration = 7f;

    private float elapsedTime = 0f;
    private float nextReductionTime = 0f;

    void Start()
    {
        enemySpawner.SetSpawnInterval(initialSpawnInterval);
        nextReductionTime = spawnIntervalReductionRate;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= nextReductionTime)
        {
            float currentInterval = enemySpawner.GetSpawnInterval();
            float newSpawnInterval = Mathf.Max(minSpawnInterval, currentInterval - spawnIntervalStep);
            
            enemySpawner.SetSpawnInterval(newSpawnInterval);
            nextReductionTime += spawnIntervalReductionRate;
        }


        if (elapsedTime*60 >= gameDuration)
        {
            // boss fight transition
        }

    }
}
