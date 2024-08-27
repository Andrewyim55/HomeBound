using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] protected float timeTillDespawn;

    private float dmg;
    private float flyTime;

    protected virtual void Update()
    {
        flyTime += Time.deltaTime;
        if (flyTime >= timeTillDespawn)
        {
            Destroy(gameObject);
        }
    }

    public void SetDmg(float _dmg)
    {
        dmg = _dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null && gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDmg(dmg);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && gameObject.layer == 16)
        {
            collision.gameObject.GetComponent<Player>().TakeDmg(dmg);
        }
        else if (collision.gameObject.GetComponent<Enemy>() != null && gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDmg(dmg);
        }
        Destroy(gameObject);
    }
}
