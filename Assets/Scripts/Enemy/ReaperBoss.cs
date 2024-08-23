using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ReaperBoss : Enemy
{
    [Header("Ranged Attack Attributes")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletDmg;
    [SerializeField] private float fireForce;
    // attack delay is the time when it stops and starts shooting
    [SerializeField] private float attackDelay;
    [SerializeField] private float shootingDistance;


    [Header("Charge Attack Attributes")]
    [SerializeField] private float chargeSpeed;
    // This is the distance at which the enemy will charge till
    [SerializeField] private float chargeDistance;
    [SerializeField] private float chargeCooldown;
    private float lastChargeTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        return;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void SummonAttack()
    {

    }

    private void ChargeAttack()
    {

    }

    private void RangedAttack()
    {

    }

    private void AOEAttack()
    {

    }

    private void BasicAttack()
    {

    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }
}
