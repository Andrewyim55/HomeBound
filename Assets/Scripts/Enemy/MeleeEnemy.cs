using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeEnemy : Enemy
{
    [Header("Charge Attack Attributes")]
    [SerializeField] private float chargeSpeed;
    // This is the distance at which the enemy will charge till
    [SerializeField] private float chargeDistance;
    [SerializeField] private float chargeCooldown;
    private float lastChargeTime;

    protected override void Start()
    {
        lastChargeTime = -chargeCooldown;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        if (Time.time >= lastChargeTime + chargeCooldown)
        {
            StartCoroutine(PerformChargeAttack());
        }
    }

    private IEnumerator PerformChargeAttack()
    {
        lastChargeTime = Time.time;
        Vector3 chargeDirection = (target.position - transform.position).normalized;
        float chargeEndTime = Time.time + chargeDistance / chargeSpeed;

        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(0.5f);
        while (Time.time < chargeEndTime)
        {
            rb.velocity = chargeDirection * chargeSpeed;
            yield return null;
        }
        rb.velocity = Vector2.zero;
        animator.ResetTrigger("isAttacking");
        yield return new WaitForSeconds(chargeCooldown);
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        // Optional: Add Gizmos for visualizing the charge range or area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);
    }
}
