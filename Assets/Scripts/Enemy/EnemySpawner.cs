using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private List<GameObject> enemyPrefab;
    [SerializeField] private List<GameObject> elitePrefab;

    [Header("Attributes")]
    [SerializeField] private float spawnDistanceMin = 5f;
    [SerializeField] private float spawnDistanceMax = 10f;

    private float spawnInterval;
    private int totalWeight;
    private float eliteChance;
    private int eliteTotalWeight;

    // Start is called before the first frame update
    void Start()
    {
        totalWeight = 0;
        // Get the total weightage for the enemy spawning
        for (int i = 0; i < enemyPrefab.Count; i++)
        {
            totalWeight += enemyPrefab[i].GetComponent<Enemy>().GetSpawnWeight();
        }

        for (int i = 0; i < elitePrefab.Count; i++)
        {
            eliteTotalWeight += elitePrefab[i].GetComponent<Enemy>().GetSpawnWeight();
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
        if(Pathfinding.instance != null)
        {
            GameObject enemyToSpawn = ChooseEnemyType();
            Vector3 spawnPosition = GetSpawnPosition();
            Node spawnNode = Pathfinding.instance.grid.GetGridObject(spawnPosition);
            while (spawnNode == null || spawnNode.nodeType != Node.NodeType.FLOOR)
            {
                spawnPosition = GetSpawnPosition();
                spawnNode = Pathfinding.instance.grid.GetGridObject(spawnPosition);
            }
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    // Method to choose the enemy type based on spawn factors
    private GameObject ChooseEnemyType()
    {
        int spawnRoll = Random.Range(0, 100);
        if (spawnRoll <= eliteChance)
        {
            return ChooseEliteEnemy(); 
        }
        else
        {
            return ChooseNormalEnemy(); 
        }
    }


    private GameObject ChooseNormalEnemy()
    {
        int spawnNum = Random.Range(0, totalWeight);
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

    private GameObject ChooseEliteEnemy()
    {
        int spawnNum = Random.Range(0, eliteTotalWeight);
        int currentWeight = 0;
        for (int i = 0; i < elitePrefab.Count; i++)
        {
            currentWeight += elitePrefab[i].GetComponent<Enemy>().GetSpawnWeight();

            if (spawnNum < currentWeight)
            {
                return elitePrefab[i];
            }
        }
        return elitePrefab[elitePrefab.Count - 1];
    }

    // Get a random spawn position around player
    private Vector3 GetSpawnPosition()
    {
        float spawnDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        float spawnAngle = Random.Range(0f, 360f);
        Vector3 spawnDirection = new Vector3(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle), 0).normalized;
        Vector3 spawnPosition = Player.instance.transform.position + spawnDirection * spawnDistance;
        return spawnPosition;
    }

    public void SetSpawnInterval(float newInterval)
    {
        spawnInterval = newInterval;
    }

    public float GetSpawnInterval()
    {
        return spawnInterval;
    }

    public void SetEliteChance(float newChance)
    {
        eliteChance = newChance;
    }
}
