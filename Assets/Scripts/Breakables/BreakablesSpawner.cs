using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablesSpawner : MonoBehaviour
{
    [Header("Breakable prefab")]
    [SerializeField] private GameObject breakablePrefab;

    [Header("Attributes")]
    [SerializeField] private float spawnIntervalMin = 5f; 
    [SerializeField] private float spawnIntervalMax = 30f; 
    [SerializeField] private float spawnDistanceMin = 5f;
    [SerializeField] private float spawnDistanceMax = 10f;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private int spawnAttempts = 10;
    [SerializeField] private int maxBreakables = 5;
    

    public List<GameObject> breakablesInScene = new List<GameObject>();
    private float randomInterval;
    private float spawningTime;

    // Start is called before the first frame update
    void Start()
    {
        randomInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    private void Update()
    {
        if (Player.instance == null)
            return;
        if (GameLogic.instance.isSceneChanging)
            return;

        spawningTime += Time.deltaTime;
        if(spawningTime >= randomInterval)
        {
            if (breakablesInScene.Count < maxBreakables)
            {
                SpawnBreakable();
                spawningTime = 0;
                randomInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }

    }

    // spawn breakable at random position around the player
    private void SpawnBreakable()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        // try to find a valid spawn position
        for (int i = 0; i < spawnAttempts; i++) 
        {
            spawnPosition = GetSpawnPosition();
            Node spawnNode = Pathfinding.instance.grid.GetGridObject(spawnPosition);
            if (spawnNode != null && spawnNode.nodeType == Node.NodeType.FLOOR)
            {
                if (IsValidSpawnPosition(spawnPosition))
                {
                    validPositionFound = true;
                    break;
                }
            }
        }

        if (!validPositionFound)
        {
            Debug.Log("Unable to find a valid spawn position.");
            return;
        }
        
        // instantiate breakable and add to list
        GameObject breakable = Instantiate(breakablePrefab, spawnPosition, Quaternion.identity);
        breakablesInScene.Add(breakable);
        
        // remove from list when destroyed
        var breakableComponent = breakable.GetComponent<Breakables>();
        if (breakableComponent != null)
        {
            breakableComponent.OnBreak += () => breakablesInScene.Remove(breakable);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        float spawnDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
        float spawnAngle = Random.Range(0f, 360f);
        Vector3 spawnDirection = new Vector3(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle), 0).normalized;
        Vector3 spawnPosition = Player.instance.transform.position + spawnDirection * spawnDistance;
        return spawnPosition;
    }

    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Check distance to existing breakables
        foreach (GameObject breakable in breakablesInScene)
        {
            if (Vector3.Distance(position, breakable.transform.position) < minSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }
}
