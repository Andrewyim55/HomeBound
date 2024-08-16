using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : Weapon
{
    [Header("Flamethrower Attributes")]
    [SerializeField] private float coneSize;
    [SerializeField] private int numFire;
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
        for (int i = 0; i < numFire; i++)
        {
            // Calculate a random rotation within the cone
            float randomAngle = Random.Range(-coneSize / 2f, coneSize / 2f);
            Quaternion pelletRotation = Quaternion.Euler(0, 0, randomAngle);

            // Instantiate the bullet with the calculated rotation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * pelletRotation);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.parent.parent.GetComponent<Collider2D>());
            bullet.GetComponent<Bullet>().SetDmg(bulletDmg);
            bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * fireForce, ForceMode2D.Impulse);
        }
        magazineSize--;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the cone in the editor for visualization
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            float halfConeSize = coneSize / 2f;

            // Draw lines representing the cone
            Vector3 rightBoundary = Quaternion.Euler(0, 0, halfConeSize) * firePoint.right * 2f;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -halfConeSize) * firePoint.right * 2f;

            Gizmos.DrawRay(firePoint.position, rightBoundary);
            Gizmos.DrawRay(firePoint.position, leftBoundary);
        }
    }
}
