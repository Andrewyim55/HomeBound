using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void Start()
    {
        base.Start();
    }
    public override void Fire()
    {
        // If there is ammo in the magazine, fire
        if (magazineSize > 0)
        {
            SoundManager.instance.PlaySfx(sfxClip, transform);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.parent.parent.GetComponent<Collider2D>());
            bullet.GetComponent<Bullet>().SetDmg(bulletDmg);
            bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
            base.Fire();
        }
        else
        {
            // If there is no ammo, reload gun
            isReloading = true;
        }
    }
}
