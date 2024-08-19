using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("Flamethrower Attributes")]
    [SerializeField] private ParticleSystem flameParticles;
    private bool isFiring;

    private void Start()
    {
        isFiring = false;
    }
    // Start is called before the first frame update
    public override void Fire()
    {
        if (!isFiring && magazineSize > 0)
        {
            isFiring = true;
            flameParticles.Play();
            StartCoroutine(FireContinuously());
            SoundManager.instance.PlaySfx(sfxClip, transform);
        }
    }
    public override void StopFire()
    {
        isFiring = false;
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
