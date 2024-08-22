﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected AudioClip sfxClip;
    [SerializeField] protected AudioClip reloadingClip;
    [SerializeField] protected AudioSource reloadingSource;

    [Header("Attributes")]
    [SerializeField] protected float fireForce;
    [SerializeField] public float bulletDmg;
    [SerializeField] public int magazineSize;
    [SerializeField] protected float reloadSpeed;
    [SerializeField] protected float fireRate;
    [SerializeField] public bool isAutomatic;

    protected float reloadTime;
    public int magSize;
    protected bool isReloading;

    public virtual void Start()
    {
        magSize = magazineSize;
        isReloading = false;
        reloadingSource.clip = reloadingClip;
        reloadingSource.loop = true;
    }
    public virtual void Fire()
    {
        magazineSize--;
    }

    public virtual void StopFire()
    {

    }
    private void Update()
    {
        if(magazineSize <= 0 && !isReloading)
        {
            reloadingSource.Play();
            isReloading = true;
        }
        if (isReloading)
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
            reloadingSource.Stop();
        }
    }
}
