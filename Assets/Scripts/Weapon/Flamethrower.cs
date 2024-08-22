using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("Flamethrower Attributes")]
    [SerializeField] private ParticleSystem flameParticles;
    [SerializeField] private AudioClip flameClip;
    private bool isFiring;

    public override void Start()
    {
        isFiring = false;
        base.Start();
    }
    // Start is called before the first frame update
    public override void Fire()
    {
        if (!isFiring && magazineSize > 0)
        {
            isFiring = true;
            audioSource.volume = SoundManager.instance.GetSFXVol();
            audioSource.clip = flameClip;
            audioSource.Play();
            flameParticles.Play();
            StartCoroutine(FireContinuously());

        }
        else
        {
            isFiring = false;
            isReloading = true;
        }
    }
    public override void StopFire()
    {
        isFiring = false;
        audioSource.Stop();
        flameParticles.Stop();
    }
    private IEnumerator FireContinuously()
    {
        while (isFiring && magazineSize > 0)
        {
            magazineSize -= 10;
            // Control firing rate
            yield return new WaitForSeconds(1f / fireRate);
        }
        // Stop firing when out of ammo
        isFiring = false;
        audioSource.Stop();
        flameParticles.Stop();
    }

    private void OnParticleCollision(GameObject collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDmg(bulletDmg);

        }
    }
}
