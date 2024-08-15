using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Pathfinding pathfinding;

    [Header("Attributes")]
    [SerializeField] protected float health;
    // dmg is the damage done to player if player collides with the enemy
    [SerializeField] protected float dmg;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int spawnWeight;

    // As each node in the path is at the bottom left of each tile, we can add it by half the cellsize in x and y to get the middle point
    protected List<Node> path;
    protected int pathIndex;

    protected float pathUpdateTimer;
    protected float pathUpdateInterval = 0.5f;

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        pathfinding = GameObject.FindGameObjectWithTag("Map").GetComponent<Pathfinding>();
        pathfinding = FindObjectOfType<Pathfinding>();
        UpdatePath();
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    protected virtual void Update()
    {
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            UpdatePath();
            pathUpdateTimer = 0f;
        }

        RotateTowardsTarget();

        // check if in range of player
        // if not in range follow path to player
        if (!CheckInRange())
        {
            FollowPath();
        }
        else
        {
            // attack the player when in range
            path = null;
            rb.velocity = Vector2.zero;
            Attack();
        }
    }

    protected void FollowPath()
    {
        if (path == null || pathIndex >= path.Count)
        {
            return;
        }

        Vector3 pathPosition = path[pathIndex].GetPosition();
        Vector3 direction = (path[pathIndex].GetPosition() + new Vector3(path[pathIndex].grid.GetCellSize() / 2, path[pathIndex].grid.GetCellSize() / 2, 0) - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (Vector3.Distance(transform.position, path[pathIndex].GetPosition()) < 0.1f)
        {
            pathIndex++;
        }
    }

    protected virtual void UpdatePath()
    {
        // update the path to the player
        if (pathfinding != null && target != null)
        {
            Node startNode = pathfinding.grid.GetGridObject(transform.position);
            Node endNode = pathfinding.grid.GetGridObject(target.position);
            path = pathfinding.FindPath(startNode.x, startNode.y, endNode.x, endNode.y);
            pathIndex = 1;
            // update the target so that the enemy will look at where it is going to next
        }
    }

    protected virtual void RotateTowardsTarget()
    {
        if (target == null)
            return;

        // If not in range of target look at where it is going 
        if (!CheckInRange())
        {
            if (path == null)
                return;
            Vector3 nextNodePosition = path[pathIndex].GetPosition();
            Vector3 direction = (nextNodePosition + new Vector3(path[pathIndex].grid.GetCellSize() / 2, path[pathIndex].grid.GetCellSize() / 2, 0) - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200 * Time.deltaTime);
        }
        else
        {
            // If in range of the target look at the target
            float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg + -90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200 * Time.deltaTime);
        }
    }

    protected abstract void Attack();

    protected bool CheckInRange()
    {
        if (target == null)
            return false;
        return Vector2.Distance(target.position, transform.position) <= attackRange;
    }

    public void TakeDmg(float _dmg)
    {
        health -= _dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public int GetSpawnWeight()
    {
        return spawnWeight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().TakeDmg(dmg);
        }
    }

    protected virtual void DrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, attackRange);

        // Draw the path
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(
                    path[i].GetPosition() + new Vector3(path[pathIndex].grid.GetCellSize() / 2, path[pathIndex].grid.GetCellSize() / 2, 0), 
                    path[i + 1].GetPosition() + new Vector3(path[pathIndex].grid.GetCellSize() / 2, path[pathIndex].grid.GetCellSize() / 2, 0));
            }
        }
    }
}
