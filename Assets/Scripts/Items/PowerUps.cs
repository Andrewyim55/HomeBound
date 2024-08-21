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
        bomb
    }
    [Header("Attributes")]
    [SerializeField] private powerUp type;
    [SerializeField] private float healAmt;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float speedDuration = 5f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

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
            }
            Destroy(gameObject);
        }
    }
}
