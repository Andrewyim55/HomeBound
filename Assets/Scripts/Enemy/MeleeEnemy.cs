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
    private bool isCharging;
    private float lastChargeTime;

    protected override void Start()
    {
        base.Start();
        isCharging = false;
        lastChargeTime = -chargeCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if (isCharging)
        {
            ChargeTowardsTarget();
        }
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
        isCharging = true;
        lastChargeTime = Time.time;

        Vector3 chargeDirection = (target.position - transform.position).normalized;
        float chargeEndTime = Time.time + chargeDistance / chargeSpeed;

        while (Time.time < chargeEndTime)
        {
            rb.velocity = chargeDirection * chargeSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isCharging = false;
    }

    private void ChargeTowardsTarget()
    {
        // Continue charging if the enemy is still in the charge state
        if (isCharging)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * chargeSpeed;
        }
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        // Optional: Add Gizmos for visualizing the charge range or area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargeDistance);
    }
}
