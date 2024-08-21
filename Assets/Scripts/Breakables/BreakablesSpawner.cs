using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablesSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Pathfinding pathFinding;

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
    

    private Transform playerTransform;
    private List<GameObject> breakablesInScene = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        StartCoroutine(SpawnBreakableRoutine());
    }

    private IEnumerator SpawnBreakableRoutine()
    {
        while (true)
        {
            // only spawn breakables if not at max amount
            if (breakablesInScene.Count < maxBreakables)
            {
                SpawnBreakable();
            }
            // wait for random interval before spawning next breakable
            float randomInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(randomInterval);
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
            Node spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
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
        Vector3 spawnPosition = playerTransform.position + spawnDirection * spawnDistance;
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
