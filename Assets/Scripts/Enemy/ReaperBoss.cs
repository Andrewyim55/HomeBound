using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ReaperBoss : Enemy
{
    [Header("Ranged Attack References")]
    [SerializeField] private GameObject[] firePoints;
    [SerializeField] private GameObject bulletPrefab;

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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(RangedAttack());
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

    private void SummonMinions()
    {
        animator.SetTrigger("summon");
        animator.ResetTrigger("summon");
    }

    private void ChargeAttack()
    {
        animator.SetTrigger("charge");
        animator.ResetTrigger("charge");
    }

    IEnumerator RangedAttack()
    {
        isAttacking = true;
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

    private void AOEAttack()
    {
        animator.SetTrigger("special");
        animator.ResetTrigger("special");
    }

    private void BasicAttack()
    {
        animator.SetTrigger("attack");
        animator.ResetTrigger("attack");
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
