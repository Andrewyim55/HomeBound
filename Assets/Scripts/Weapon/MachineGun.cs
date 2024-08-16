using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : Weapon
{
    private bool isFiring;

    private void Start()
    {
        isFiring = false;
    }
    // Start is called before the first frame update
    public override void Fire()
    {
        if(!isFiring && magazineSize > 0)
        {
            isFiring = true;
            StartCoroutine(FireContinuously());
        }
    }
    public override void StopFire()
    {
        isFiring = false;
    }
    private IEnumerator FireContinuously()
    {
        while (isFiring && magazineSize > 0)
        {
            FireBullet();
            // Control firing rate
            yield return new WaitForSeconds(1f / fireRate);
        }
        // Stop firing when out of ammo
        isFiring = false;
    }

    private void FireBullet()
    {
        SoundManager.instance.PlaySfx(sfxClip, transform);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.parent.parent.GetComponent<Collider2D>());
        bullet.GetComponent<Bullet>().SetDmg(bulletDmg);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
        magazineSize--;
    }
}
