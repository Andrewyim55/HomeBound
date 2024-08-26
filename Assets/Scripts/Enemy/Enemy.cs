using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public abstract class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Pathfinding pathfinding;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected GameObject damagePopUpPrefab;

    [Header("Attributes")]
    [SerializeField] protected float health;
    // dmg is the damage done to player if player collides with the enemy
    [SerializeField] protected float dmg;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int spawnWeight;
    [SerializeField] protected float attackWaitTime;

    // As each node in the path is at the bottom left of each tile, we can add it by half the cellsize in x and y to get the middle point
    protected List<Node> path;
    protected int pathIndex;

    protected float pathUpdateTimer;
    protected float pathUpdateInterval = 0.5f;
    private bool isAlive;
    private Loot loot;
    private float maxHealth;

    protected virtual void Start()
    {
        maxHealth = health;
        isAlive = true;
        loot = GetComponent<Loot>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        pathfinding = GameObject.FindGameObjectWithTag("Map").GetComponent<Pathfinding>();
        if (target.GetComponent<Player>().GetAlive())
        {
            UpdatePath();
            animator.SetBool("target", true);
        }
        else
        {
            animator.SetBool("target", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    protected virtual void Update()
    {
        if (!target.GetComponent<Player>().GetAlive() || !isAlive)
        {
            return;
        }

        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            UpdatePath();
            pathUpdateTimer = 0f;
        }
        // check if in range of player
        // if not in range follow path to player
        if (!CheckInRange())
        {
            if (attackWaitTime <= 0)
            {
                FollowPath();
            }
            else
            {
                attackWaitTime -= Time.deltaTime;
            }
        }
        else
        {
            // attack the player when in range
            path = null;
            rb.velocity = Vector2.zero;
            Attack();
        }
        flipSprite();
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

        if (Vector3.Distance(transform.position, path[pathIndex].GetPosition()) < 0.5f)
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
            path = pathfinding.FindPath(startNode.x, startNode.y, endNode.x, endNode.y, GetComponent<Collider2D>().bounds.extents.magnitude);
            pathIndex = 1;
            // update the target so that the enemy will look at where it is going to next
        }
    }

    // This will be called when the enemy is in attack range of the player
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
        DamagePopUp(transform.position, _dmg);
        if (healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public int GetSpawnWeight()
    {
        return spawnWeight;
    }

    private void flipSprite()
    {
        // Rotation of weapon
        Vector2 aimDir = (new Vector2(target.position.x, target.position.y) - rb.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        if (aimDir.x < 0)
        {
            sr.flipX = true;
        }
        else if (aimDir.x > 0)
        {
            sr.flipX = false;
        }
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
    public IEnumerator Die()
    {
        if(isAlive)
        {
            if (loot != null)
            {
                loot.DropLoot();
            }

            isAlive = false;
            animator.SetTrigger("Death");
            GetComponent<BoxCollider2D>().enabled = false;
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            Destroy(gameObject);
        }
    }

    public float GetHealth()
    {
        return health;
    }

    public void DamagePopUp(Vector3 position, float dmgAmount)
    {
        // Generate a random offset for the pop-up position
        float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f);  // Adjust the range as needed
        float offsetY = UnityEngine.Random.Range(-0.5f, 0.5f);  // Adjust the range as needed

        // Apply the offset to the position
        Vector3 randomOffset = new Vector3(offsetX, offsetY, 0);
        Vector3 popUpPosition = position + randomOffset;

        // Instantiate the damage pop-up at the new position
        Transform damagePopUpTransform = Instantiate(damagePopUpPrefab.transform, popUpPosition, Quaternion.identity);

        // Set the damage value on the pop-up
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetValue(dmgAmount);
    }
}
