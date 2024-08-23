﻿using System.Collections;
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
    private float chargeCooldownTimer;

    protected override void Start()
    {
        chargeCooldownTimer = -chargeCooldown;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        if (Time.time >= chargeCooldownTimer + chargeCooldown)
        {
            StartCoroutine(PerformChargeAttack());
        }
    }

    private IEnumerator PerformChargeAttack()
    {
        chargeCooldownTimer = Time.time;
        Vector3 chargeDirection = (target.position - transform.position).normalized;
        float chargeEndTime = Time.time + chargeDistance / chargeSpeed;
        float animWaitTime = 0.5f;
        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(animWaitTime);
        while (Time.time < chargeEndTime + animWaitTime)
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
