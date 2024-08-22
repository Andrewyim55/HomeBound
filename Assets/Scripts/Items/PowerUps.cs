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
        NONE
    }
    [Header("References")]
    [SerializeField] protected Sprite healSprite;
    [SerializeField] protected Sprite bombSprite;
    [SerializeField] protected Sprite speedSprite;
    [SerializeField] protected AudioClip healClip;
    [SerializeField] protected AudioClip bombClip;
    [SerializeField] protected AudioClip speedClip;


    [Header("Attributes")]
    [SerializeField] private powerUp type;
    [SerializeField] private float healAmt;
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float speedDuration = 5f;

    private void Start()
    {
        /*
        type = (powerUp)Random.Range(0, (int)powerUp.NONE);
        switch (type)
        {
            case powerUp.health:
                GetComponent<SpriteRenderer>().sprite = healSprite;
                GetComponentInChildren<SpriteRenderer>().sprite = healSprite;
                break;
            case powerUp.speed:
                GetComponent<SpriteRenderer>().sprite = speedSprite;
                GetComponentInChildren<SpriteRenderer>().sprite = speedSprite;
                break;
            case powerUp.bomb:
                GetComponent<SpriteRenderer>().sprite = bombSprite;
                GetComponentInChildren<SpriteRenderer>().sprite = bombSprite;
                break;
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            switch (type)
            {
                case powerUp.health:
                    SoundManager.instance.PlaySfx(healClip, transform);
                    float hp = player.GetHealth() + healAmt;
                    player.SetHealth(Mathf.Min(hp, player.GetMaxHealth()));
                    player.UpdateHealthBar();
                    break;

                case powerUp.speed:
                    SoundManager.instance.PlaySfx(speedClip, transform);
                    player.ApplySpeedBoost(speedMultiplier, speedDuration);
                    break;

                case powerUp.bomb:
                    SoundManager.instance.PlaySfx(bombClip, transform);
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
