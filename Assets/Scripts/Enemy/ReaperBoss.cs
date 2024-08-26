using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ReaperBoss : Enemy
{
    [Header("Ranged Attack References")]
    [SerializeField] private GameObject[] firePoints;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Summon References")]
    [SerializeField] private List<GameObject> summonPrefab;
    [SerializeField] private Pathfinding pathFinding;

    [Header("Attack Attributes")]
    // attackCoolDown is the cooldown of its attacks
    [SerializeField] private float attackCoolDown;

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

    private bool isAttacking = false;
    private Transform bossTransform;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(ChargeAttack());
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // Logic for which attack to do
    }

    IEnumerator SummonMinions()
    {
        animator.SetTrigger("summon");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);

        // Code for spawning minions in the summoning radius
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject enemyToSpawn = ChooseEnemyType();
            Vector3 spawnPosition = GetSpawnPosition();
            Node spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
            while (spawnNode == null || spawnNode.nodeType != Node.NodeType.FLOOR)
            {
                spawnPosition = GetSpawnPosition();
                spawnNode = pathFinding.grid.GetGridObject(spawnPosition);
            }
            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);
        }

        animator.ResetTrigger("summon");
        StartCoroutine(AttackCoolDown());
    }

    private GameObject ChooseEnemyType()
    {
        int spawnNum = Random.Range(0, summonPrefab.Count-1);
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
        animator.SetTrigger("charge");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        Vector3 chargeDirection = (target.position - transform.position).normalized;
        float chargeEndTime = Time.time + chargeDistance / chargeSpeed;
        while (Time.time < chargeEndTime + animator.GetCurrentAnimatorStateInfo(0).length / 2)
        {
            rb.velocity = chargeDirection * chargeSpeed;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        animator.ResetTrigger("charge");
        StartCoroutine(AttackCoolDown());
    }

    // Bosses ranged attackl
    IEnumerator RangedAttack()
    {
        animator.SetTrigger("rangeAttack");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);

        // Loop through each fire point and spawn one ShadowBall
        for (int i = 0; i < firePoints.Length; i++)
        {
            Vector2 direction = (target.position - firePoints[i].transform.position).normalized;
            GameObject shadowBall = Instantiate(bulletPrefab, firePoints[i].transform.position, Quaternion.identity);
            shadowBall.GetComponent<ShadowBall>().SetDmg(bulletDmg);

            StartCoroutine(ShadowBallDelay(shadowBall, direction));
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
        animator.SetTrigger("special");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        animator.ResetTrigger("special");
        StartCoroutine(AttackCoolDown());
    }

    IEnumerator BasicAttack()
    {
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
        animator.ResetTrigger("attack");
        StartCoroutine(AttackCoolDown());
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingDistance );
    }
}
