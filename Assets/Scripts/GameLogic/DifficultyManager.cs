using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner; // Reference to the EnemySpawner script

    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.2f;
    [SerializeField] private float spawnIntervalStep = 0.2f;
    [SerializeField] private float spawnIntervalReductionRate = 30;

    [Header("Elite spawning")]
    [SerializeField] private float initialEliteChance = 5f;
    [SerializeField] private float maxEliteChance = 20f;

    [Header("Game length")]
    [SerializeField] private float gameDuration = 5f;

    private float elapsedTime = 0f;
    private float nextReductionTime = 0f;

    void Start()
    {
        enemySpawner.SetSpawnInterval(initialSpawnInterval);
        enemySpawner.SetEliteChance(initialEliteChance);
        nextReductionTime = spawnIntervalReductionRate;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float eliteIncreaseDuration = gameDuration*60;
        float eliteChance = Mathf.Lerp(initialEliteChance, maxEliteChance, Mathf.Clamp01(elapsedTime / eliteIncreaseDuration));

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
