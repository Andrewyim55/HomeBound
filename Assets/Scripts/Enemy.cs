using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float dmg;

    public void TakeDmg(float _dmg)
    {
        health -= _dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Player>() != null)
        {
            Debug.Log("Took Damage");
            collision.gameObject.GetComponent<Player>().TakeDmg(dmg);
        }
    }
}
