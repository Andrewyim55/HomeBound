using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Pathfinding pathfinding;

    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float dmg;
    [SerializeField] private float attackRange;
    [SerializeField] private float firingRate;
    [SerializeField] private float timeTillFire;
    [SerializeField] private float bulletDmg;
    [SerializeField] private float fireForce;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float packUp;

    private float fireTime;
    private bool isShooting;
    private float packingUp;
    private List<Node> path;
    private int pathIndex;

    private float pathUpdateTimer;
    private float pathUpdateInterval = 0.5f;

    private void Start()
    {
        fireTime = timeTillFire;
        pathfinding = FindObjectOfType<Pathfinding>();
        UpdatePath();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, attackRange);

        // Draw the path
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].GetPosition() , path[i + 1].GetPosition());
            }
        }
    }

    private void Update()
    {
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            UpdatePath();
            pathUpdateTimer = 0f;
        }

        RotateTowardsTarget();

        if (!CheckInRange())
        {
            if (isShooting)
            {
                packingUp += Time.deltaTime;
                if (packingUp >= packUp)
                {
                    isShooting = false;
                    fireTime = timeTillFire;
                    packingUp = 0;
                }
            }
            else
            {
                FollowPath();
            }
        }
        else
        {
            path = null;
            isShooting = true;
            rb.velocity = Vector2.zero;
            // If target in range, shoot instead
            fireTime += Time.deltaTime;
            if (fireTime >= 1f / firingRate)
            {
                Fire();
                fireTime = 0f;
            }
        }
    }

    private void FollowPath()
    {
        if (path == null || pathIndex >= path.Count)
        {
            return;
        }

        Vector3 pathPosition = path[pathIndex].GetPosition();
        Vector3 direction = (path[pathIndex].GetPosition() - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (Vector3.Distance(transform.position, path[pathIndex].GetPosition()) < 0.1f)
        {
            pathIndex++;
        }
    }

    private void UpdatePath()
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

    private void RotateTowardsTarget()
    {
        if (target == null)
            return;

        // If not in range of target look at where it is going 
        if (!CheckInRange())
        {
            if (path == null)
                return;
            Vector3 nextNodePosition = path[pathIndex].GetPosition();
            Vector3 direction = (nextNodePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
        else
        {
            // If in range of the target look at the target
            float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg + -90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
    }

    private void Fire()
    {
        // Spawn the bullet at where it is aiming at
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
        bullet.GetComponent<Bullet>().SetDmg(bulletDmg);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }

    private bool CheckInRange()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Debug.Log("Took Damage");
            collision.gameObject.GetComponent<Player>().TakeDmg(dmg);
        }
    }
}