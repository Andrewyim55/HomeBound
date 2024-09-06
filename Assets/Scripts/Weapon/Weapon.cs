using System.Collections;
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
    [SerializeField] protected AudioSource audioSource;

    [Header("Attributes")]
    [SerializeField] protected float fireForce;
    [SerializeField] public float bulletDmg;
    [SerializeField] public int magazineSize;
    [SerializeField] public float reloadSpeed;
    [SerializeField] protected float fireRate;
    [SerializeField] public bool isAutomatic;

    protected float reloadTime;
    public int magSize;
    protected bool isReloading;

    public virtual void Start()
    {
        reloadTime = 0;
        magSize = magazineSize;
        isReloading = false;
        audioSource.loop = true;
    }
    public virtual void Fire()
    {
        if (isReloading)
            return;
        audioSource.volume = SoundManager.instance.GetSFXVol();
        magazineSize--;

    }

    public virtual void StopFire()
    {

    }
    private void Update()
    {
        if (GameLogic.instance != null)
        {
            if (GameLogic.instance.GetPaused())
            {
                audioSource.Pause();
                return;
            }
            else if (!GameLogic.instance.GetPaused())
            {
                audioSource.UnPause();
            }
        }

        if (magazineSize <= 0 && !isReloading)
        {
            isReloading = true;
        }
        if(isReloading)
        {
            audioSource.volume = SoundManager.instance.GetSFXVol();
            audioSource.clip = reloadingClip;
            Reload();
        }
    }

    // This will be the reloading of the gun
    private void Reload()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        if (reloadTime <= reloadSpeed)
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
            audioSource.Stop();
        }
    }

    public bool GetReloading()
    {
        return isReloading;
    }
    public void SetReloading(bool reloading)
    {
        isReloading = reloading;
    }
}
