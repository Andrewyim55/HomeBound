using System;
using System.Collections;
using UnityEngine;

public class Breakables : MonoBehaviour
{
    public event Action OnBreak;
    private Loot loot;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private bool hasBroken = false; //flag to check if already broken

    [SerializeField] private Sprite brokenSprite; 
    [SerializeField] private float destroyDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        loot = GetComponent<Loot>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
        StartCoroutine(FadeAndDestroy());
    }

     private IEnumerator FadeAndDestroy()
    {
        float elapsedTime = 0f;
        Color c = spriteRenderer.color;

        boxCollider2D.enabled = false;

        // fade before destroying
        Destroy(gameObject, destroyDelay);
        while (elapsedTime < destroyDelay)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / destroyDelay);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
    }
}
