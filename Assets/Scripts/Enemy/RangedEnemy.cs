using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private float attackCooldown;
    [SerializeField] private float shootingDistance;

    private bool isShooting;
    private float attackCooldownTimer;

    protected override void Start()
    {
        base.Start();
        attackCooldownTimer = -attackCooldown;
    }

    protected override void Update()
    {
        if (target == null)
            return;

        base.Update();
        flipSprite();
    }

    protected override void UpdatePath()
    {
        // Only update the path if the enemy is not within shooting range
        if (pathfinding != null && target != null && !isShooting)
        {
            Node startNode = pathfinding.grid.GetGridObject(transform.position);
            Node endNode = pathfinding.grid.GetGridObject(target.position);
            path = pathfinding.FindPath(startNode.x, startNode.y, endNode.x, endNode.y, GetComponent<Collider2D>().bounds.extents.magnitude);
            pathIndex = 1;
        }
    }

    protected override void Attack()
    {
        if (Time.time >= attackCooldownTimer + attackCooldown)
        {
            StartCoroutine(PerformRangedAttack());
        }
    }

    IEnumerator PerformRangedAttack()
    {
        attackCooldownTimer = Time.time;
        animator.SetTrigger("isAttacking");
        Vector2 direction = (target.position - firePoint.position).normalized;

        yield return new WaitForSeconds(0.5f);
        // Calculate the rotation based on the direction to the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // Instantiate a bullet at the firePoint's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
        if (bullet.GetComponent<Bullet>() != null)
        {
            bullet.GetComponent<Bullet>().SetDmg(bulletDmg);
        }

        // Apply force to the bullet to shoot it forward
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(bullet.transform.right * fireForce, ForceMode2D.Impulse);
        }
        animator.ResetTrigger("isAttacking");
        yield return new WaitForSeconds(attackCooldown);
    }
#if UNITY_EDITOR
    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }
#endif
}
