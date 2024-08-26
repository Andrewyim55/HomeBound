using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float explostionDmg;

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().TakeDmg(explostionDmg);
        }
    }

    public void SetExplosionDmg(float dmg)
    {
        explostionDmg = dmg;
    }
}
