using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float timeTillDespawn;

    private float dmg;
    private float flyTime;

    private void Update()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDmg(dmg);
        }

        Destroy(gameObject);
    }
}
