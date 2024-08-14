using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Attributes")]
    [SerializeField] private int numPellets;
    [SerializeField] private float coneSize;

    public override void Fire()
    {
        // If there is ammo in the magazine, fire
        if (magazineSize > 0)
        {
            for (int i = 0; i < numPellets; i++)
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
        }
        else
        {
            // If there is no ammo, reload gun
            isReloading = true;
        }
        base.Fire();
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
