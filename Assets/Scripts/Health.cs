using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public Image healthBarImage;
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TakeDamage(1);
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Decrease current health by the damage value
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp current health to ensure it stays between 0 and maxHealth
        UpdateHealthBar(); // Update the health bar to reflect the new health value
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount; // Increase current health by the heal amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp current health to ensure it stays between 0 and maxHealth
        UpdateHealthBar(); // Update the health bar to reflect the new health value
    }

    private void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth; // Calculate the fill amount as a fraction of current health over max health
        healthBarImage.fillAmount = fillAmount; // Set the fill amount of the health bar image
    }
}
