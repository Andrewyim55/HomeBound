using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected AudioClip sfxClip;

    [Header("Attributes")]
    [SerializeField] protected float fireForce;
    [SerializeField] protected float bulletDmg;
    [SerializeField] protected int magazineSize;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected float fireRate;

    protected float reloadTime;
    protected int magSize;
    protected bool isReloading;

    private void Start()
    {
        magSize = magazineSize;
        isReloading = false;
    }
    public virtual void Fire()
    {
        magazineSize--;
    }

    private void Update()
    {
        if(isReloading)
        {
            Reload();
        }
    }

    // This will be the reloading of the gun
    public void Reload()
    {
        if(reloadTime <= reloadSpeed)
        {
            reloadTime += Time.deltaTime;
        }
        else
        {
            // Set the magzineSize to the full amount
            // Stop reloading
            // Reset the reloading time
            magazineSize = magSize;
            reloadTime = 0;
            isReloading = false;
        }
    }
}
