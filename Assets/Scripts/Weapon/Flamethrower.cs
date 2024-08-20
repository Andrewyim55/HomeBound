using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("Flamethrower Attributes")]
    [SerializeField] private ParticleSystem flameParticles;
    [SerializeField] private AudioSource flameSource;
    [SerializeField] private AudioClip flameClip;
    private bool isFiring;

    private void Start()
    {
        isFiring = false;
<<<<<<< HEAD
        
=======
        flameSource.clip = flameClip;
        flameSource.loop = true;
        flameSource.volume = SoundManager.instance.GetSFXVol();
>>>>>>> 43881da0aacc817b5f6bc751e84c99ffe589e1ef
    }
    // Start is called before the first frame update
    public override void Fire()
    {

        if (!isFiring && magazineSize > 0)
        {
            isFiring = true;
            flameParticles.Play();
            flameSource.Play();
            StartCoroutine(FireContinuously());
        }
    }
    public override void Equipped()
    {
        ammoText.text = magazineSize + "";
    }
    private void Update()
    {
 
        if (magazineSize == 0)
        {
            ammoText.text = "Out of Bullets";
        }
    }
    public override void StopFire()
    {
        isFiring = false;
        flameSource.Stop();
        flameParticles.Stop();
    }
    private IEnumerator FireContinuously()
    {
        while (isFiring && magazineSize > 0)
        {
            magazineSize -= 10;
            ammoText.text = magazineSize + "";
            // Control firing rate
            yield return new WaitForSeconds(1f / fireRate);
        }
        // Stop firing when out of ammo
        isFiring = false;
        flameSource.Stop();
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
