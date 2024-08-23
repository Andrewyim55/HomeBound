using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    enum powerUp
    {
        health,
        speed,
        bomb,
        exp,
        NONE
    }
    [Header("References")]
    [SerializeField] protected AudioClip pickupClip;

    [Header("Attributes")]
    [SerializeField] private powerUp type;
    [SerializeField] private float healAmt;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float speedDuration;
    [SerializeField] private float expAmt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            XPNLevel xpSystem = FindObjectOfType<XPNLevel>();
            SoundManager.instance.PlaySfx(pickupClip, transform);

            switch (type)
            {
                case powerUp.health:
                    float hp = player.GetHealth() + healAmt;
                    player.SetHealth(Mathf.Min(hp, player.GetMaxHealth()));
                    player.UpdateHealthBar();
                    break;

                case powerUp.speed:
                    player.ApplySpeedBoost(speedMultiplier, speedDuration);
                    break;

                case powerUp.bomb:
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (GameObject enemy in enemies)
                    {
                        enemy.GetComponent<Enemy>().TakeDmg(enemy.GetComponent<Enemy>().GetHealth());
                    }
                    break;
                case powerUp.exp:
                    xpSystem.gainXP(expAmt);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
