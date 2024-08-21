using System;
using UnityEngine;

public class Breakables : MonoBehaviour
{
    public event Action OnBreak;
    private Loot loot;
    private SpriteRenderer spriteRenderer;
    private bool hasBroken = false; //flag to check if already broken

    [SerializeField] private Sprite brokenSprite; 
    [SerializeField] private float destroyDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        loot = GetComponent<Loot>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if hit by player bullet
        if(collision.gameObject.layer == 12)
        {
            BreakBreakable();
        }
    }

    public void BreakBreakable()
    {
        // check if already broken
        if (hasBroken) return;

        // set flag
        hasBroken = true;
        Debug.Log("Breakable broken");

        // replace sprite upon breaking
        if (spriteRenderer != null && brokenSprite != null)
        {
            spriteRenderer.sprite = brokenSprite;
        }

        // drop loot
        if (loot != null)
        {
            loot.DropLoot();
        }

        OnBreak?.Invoke(); 
        Destroy(gameObject, destroyDelay);
    }
}
