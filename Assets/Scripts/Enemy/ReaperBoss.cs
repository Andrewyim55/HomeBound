using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ReaperBoss : Enemy
{
    [Header("Ranged Attack References")]
    [SerializeField] private GameObject[] firePoints;
    [SerializeField] private GameObject[] sugerChargedFirePoints;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Summon References")]
    [SerializeField] private List<GameObject> summonPrefab;
    [Header("Reaper References")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Reaper Attributes")]
    [SerializeField] private float attacksTillSuperCharge;
    [SerializeField] private float followDistance;

    [Header("Attack Attributes")]
    // attackCoolDown is the cooldown of its attacks
    [SerializeField] private float attackCoolDown;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDistance;
    [SerializeField] private float meleeDistance;


    [Header("Ranged Attack Attributes")]
    [SerializeField] private float bulletDmg;
    [SerializeField] private float fireForce;
    [SerializeField] private float shootingDistance;

    [Header("Summon Attributes")]
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnAmount;

    [Header("Charge Attack Attributes")]
    // This is the distance at which the enemy will charge till
    [SerializeField] private float chargeDistance;
    [SerializeField] private float chargeSpeed;

    [Header("Special Attack References")]
    [SerializeField] private GameObject explostionEffect;
    [Header("Special Attack Attributes")]
    [SerializeField] private float explostionDmg;


    [Header("VoiceLines References")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip summoningClip;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireBallClip;
    [SerializeField] private AudioClip dashAttackClip;


    private bool isAttacking = false;
    private Transform bossTransform;
    private float superChargeCounter;
    private bool superCharge;
    private bool isSpawning;

    // Start is called before the first frame update
    protected override void Start()
    {
        StartCoroutine(SpawnAnimation());
        isSpawning = true;
        superCharge = false;
        superChargeCounter = attacksTillSuperCharge;
        explostionEffect.GetComponent<Explosion>().SetExplosionDmg(explostionDmg);
        base.Start();
    }

    protected override void Attack()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if (target == null || isSpawning || !isAlive)
            return;
        if (!target.GetComponent<Player>().GetAlive())
        {
            if (path != null)
            {
                path = null;
                rb.velocity = Vector3.zero;
            }
            return;
        }

        bossFlipSprite();

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        
        if(distanceToPlayer > followDistance)
        {
            if (attackWaitTime <= 0)
            {
                pathUpdateTimer += Time.deltaTime;
                if (pathUpdateTimer >= pathUpdateInterval)
                {
                    UpdatePath();
                    pathUpdateTimer = 0f;
                }
        
                FollowPath();
                animator.SetBool("isFollowing", true);
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
            animator.SetBool("isFollowing", false);
            bossAttackLogic(distanceToPlayer);
        }
    }

    private void bossAttackLogic(float dist)
    {
        if (!isAttacking)
        {
            // Increment the attack counter
            if (superChargeCounter <= 0)
            {
                superChargeCounter = attacksTillSuperCharge;
                superCharge = true;
            }
            if (dist > shootingDistance)
            {
                if (Random.value < 0.3f)
                {
                    StartCoroutine(SummonMinions());
                    return;
                }
                else
                {
                    StartCoroutine(RangedAttack());
                    return;
                }
            }
            else if (dist <= meleeDistance)
            {
                StartCoroutine(BasicAttack());
                return;
            }
            else if (dist >= meleeDistance)
            {
                if (dist > chargeDistance)
                {
                    if (Random.value < 0.3f)
                    {
                        StartCoroutine(SummonMinions());
                        return;
                    }
                    else
                    {
                        StartCoroutine(RangedAttack());
                        return;
                    }
                }

                if (Random.value < 0.7f)
                {
                    StartCoroutine(ChargeAttack());
                    return;
                }
                else
                {
                    StartCoroutine(AOEAttack());
                    return;
                }
            }
        }
    }


    IEnumerator SummonMinions()
    {
        superChargeCounter--;
        isAttacking = true;
        SoundManager.instance.PlaySfx(summoningClip, transform);
        yield return new WaitForSeconds(1.1f);
        animator.SetTrigger("summon");
        attackWaitTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        float unitsToSpawn = spawnAmount;

        if (superCharge)
        {
            unitsToSpawn *= 2;
            superCharge = false;
        }
        // Code for spawning minions in the summoning radius
        for (int i = 0; i < unitsToSpawn; i++)
        {
            GameObject enemyToSpawn = ChooseEnemyType();
            Vector3 spawnPosition = GetSpawnPosition();
            Node spawnNode = pathfinding.grid.GetGridObject(spawnPosition);
            while (spawnNode == null || spawnNode.nodeType != Node.NodeType.FLOOR)
            {
                spawnPosition = GetSpawnPosition();
                spawnNode = pathfinding.grid.GetGridObject(spawnPosition);
            }
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }

        animator.ResetTrigger("summon");
        StartCoroutine(AttackCoolDown());
    }

    private GameObject ChooseEnemyType()
    {
        int spawnNum = Random.Range(0, summonPrefab.Count);
        return summonPrefab[spawnNum];
    }

    private Vector3 GetSpawnPosition()
    {
        float spawnAngle = Random.Range(0f, 360f);
        Vector3 spawnDirection = new Vector3(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle), 0).normalized;
        Vector3 spawnPosition = transform.position + spawnDirection * spawnDistance;
        return spawnPosition;
    }

    IEnumerator ChargeAttack()
    {
        superChargeCounter--;
        isAttacking = true;
        SoundManager.instance.PlaySfx(dashAttackClip, transform);
        animator.SetTrigger("charge");
        GetComponent<Collider2D>().isTrigger = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        Vector3 chargeDirection = (target.position - transform.position).normalized;
        float chargeEndTime = Time.time + chargeDistance / chargeSpeed;
        while (Time.time < chargeEndTime + animator.GetCurrentAnimatorStateInfo(0).length / 2 && !isInWall)
        {
            rb.velocity = chargeDirection * chargeSpeed;
            yield return null;
        }
        if (isInWall)
            isInWall = false;
        rb.velocity = Vector2.zero;
        animator.ResetTrigger("charge");
        GetComponent<Collider2D>().isTrigger = false;
        StartCoroutine(AttackCoolDown());
    }

    // Bosses ranged attackl
    IEnumerator RangedAttack()
    {
        superChargeCounter--;
        isAttacking = true;
        animator.SetTrigger("rangeAttack");
        attackWaitTime = animator.GetCurrentAnimatorStateInfo(0).length * 2;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        SoundManager.instance.PlaySfx(fireBallClip, transform);
        if (superCharge)
        {
            // Loop through each fire point and spawn one ShadowBall
            for (int i = 0; i < sugerChargedFirePoints.Length; i++)
            {
                Vector2 direction = (target.position - sugerChargedFirePoints[i].transform.position).normalized;
                GameObject shadowBall = Instantiate(bulletPrefab, sugerChargedFirePoints[i].transform.position, Quaternion.identity);
                shadowBall.GetComponent<ShadowBall>().SetDmg(bulletDmg);

                StartCoroutine(ShadowBallDelay(shadowBall, direction));
            }
            superCharge = false;
        }
        else
        {
            // Loop through each fire point and spawn one ShadowBall
            for (int i = 0; i < firePoints.Length; i++)
            {
                Vector2 direction = (target.position - firePoints[i].transform.position).normalized;
                GameObject shadowBall = Instantiate(bulletPrefab, firePoints[i].transform.position, Quaternion.identity);
                shadowBall.GetComponent<ShadowBall>().SetDmg(bulletDmg);

                StartCoroutine(ShadowBallDelay(shadowBall, direction));
            }
        }
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger("rangeAttack");
        StartCoroutine(AttackCoolDown());
    }
    private IEnumerator ShadowBallDelay(GameObject shadowBall, Vector2 direction)
    {
        yield return new WaitForSeconds(0.5f);
        Rigidbody2D rb = shadowBall.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * fireForce, ForceMode2D.Impulse);
        shadowBall.GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator AOEAttack()
    {
        superChargeCounter--;
        isAttacking = true;
        SoundManager.instance.PlaySfx(explosionClip, transform);
        animator.SetTrigger("special");
        yield return new WaitForSeconds(0.5f);
        explostionEffect.GetComponent<Animator>().SetTrigger("explosion");
        yield return new WaitForSeconds(explostionEffect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length / 2);
        explostionEffect.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        explostionEffect.GetComponent<Collider2D>().enabled = false;
        animator.ResetTrigger("special");
        StartCoroutine(AttackCoolDown());
    }

    IEnumerator BasicAttack()
    {
        superChargeCounter--;
        isAttacking = true;
        animator.SetTrigger("attack");
        attackWaitTime = 0.5f;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 4);
        CheckForHit();

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        CheckForHit();

        animator.ResetTrigger("attack");
        StartCoroutine(AttackCoolDown());
    }

    private void CheckForHit()
    {
        // Check for collisions with the player within the attack range
        Collider2D[] hitList = Physics2D.OverlapCircleAll(attackPoint.position, attackDistance, playerLayer);

        foreach (Collider2D hit in hitList)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<Player>().TakeDmg(dmg);
            }
        }
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    private IEnumerator SpawnAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetTrigger("spawn");
        yield return new WaitForSeconds(2f);
        isSpawning = false;
    }

    protected void bossFlipSprite()
    {
        // Rotation of weapon
        Vector2 aimDir = (new Vector2(target.position.x, target.position.y) - rb.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // Get the current scale
        Vector3 localScale = sr.transform.localScale;
        if (aimDir.x < 0)
        {
            // Flip the sprite by scaling on the X-axis by -1
            localScale.x = Mathf.Abs(localScale.x) * -1;
        }
        else if (aimDir.x > 0)
        {
            // Ensure the sprite is not flipped
            localScale.x = Mathf.Abs(localScale.x);
        }

        // Apply the new scale
        sr.transform.localScale = localScale;
    }

#if UNITY_EDITOR
    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
#endif
}
