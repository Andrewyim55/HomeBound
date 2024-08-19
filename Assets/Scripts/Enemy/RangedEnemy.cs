using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Range Attack References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Range Attack Attributes")]
    [SerializeField] private float firingRate;
    [SerializeField] private float bulletDmg;
    [SerializeField] private float fireForce;
    // attack delay is the time when it stops and starts shooting
    [SerializeField] private float attackDelay;
    [SerializeField] private float shootingDistance;


    private float fireTime;
    private bool isShooting;
    private float attackDelayTimer;

    protected override void Start()
    {
        base.Start();
        fireTime = 0;
        attackDelayTimer = attackDelay;
    }

    protected override void Update()
    {
        base.Update();

        // If the enemy is within shooting distance and not already shooting, start the attack delay timer
        if (Vector2.Distance(target.position, transform.position) <= shootingDistance && !isShooting)
        {
            attackDelayTimer -= Time.deltaTime;
            if (attackDelayTimer <= 0f)
            {
                // Timer has reached zero, the enemy can start attacking
                isShooting = true;
            }
        }
        else if (Vector2.Distance(target.position, transform.position) > shootingDistance)
        {
            // Reset the timer and stop shooting if the player moves out of shooting distance
            attackDelayTimer = attackDelay;
            isShooting = false;
        }

        // If the enemy is allowed to shoot, call the attack method
        if (isShooting)
        {
            Attack();
        }
    }
    protected override void RotateTowardsTarget()
    {
        if (target == null)
            return;

        if (Vector2.Distance(target.position, transform.position) <= shootingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + -90f;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100 * Time.deltaTime);
        }
        else
        {
            base.RotateTowardsTarget();
        }
    }

    protected override void UpdatePath()
    {
        // Only update the path if the enemy is not within shooting range
        if (pathfinding != null && target != null && !isShooting)
        {
            Node startNode = pathfinding.grid.GetGridObject(transform.position);
            Node endNode = pathfinding.grid.GetGridObject(target.position);
            path = pathfinding.FindPath(startNode.x, startNode.y, endNode.x, endNode.y);
            pathIndex = 1;
        }
    }

    protected override void Attack()
    {
        // If the enemy is ready to fire
        if (fireTime >= 1f / firingRate)
        {
            // Instantiate a bullet at the firePoint's position and rotation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.layer = 11;

            // Set the bullet's damage (if your bullet script handles this)
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDmg(bulletDmg);
            }

            // Apply force to the bullet to shoot it forward
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
            }

            // Reset the fire time
            fireTime = 0f;
        }
        else
        {
            // Increase the fire time counter
            fireTime += Time.deltaTime;
        }
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        // Optional: Add Gizmos for visualizing the charge range or area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }
}
