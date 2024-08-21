using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public LootCategory[] lootCategories;

    public void DropLoot()
    {
        if (lootCategories.Length == 0) 
        {
            Debug.Log("No loot categories assigned");
            return;
        }

        // choose category of items to drop
        GameObject[] selectedCategory = ChooseLootCategory();
        if(selectedCategory != null && selectedCategory.Length >0)
        {
            // drop an item from within category
            DropItem(selectedCategory);
        }
    }

    private GameObject[] ChooseLootCategory()
    {
        // get total weight
        int totalWeight = 0;
        foreach (var category in lootCategories)
        {
            totalWeight += category.dropChance;
        }
        if (totalWeight <= 0) return null;

        // get random number
        int lootNum = Random.Range(0, totalWeight);
        
        // select category
        int currentWeight = 0;
        foreach (var category in lootCategories)
        {
            currentWeight += category.dropChance;
            if (lootNum < currentWeight)
            {
                Debug.Log("Loot dropped from: " + category.categoryName);
                // return list of items within category
                return category.items;
            }
        }

        // fallback
        return lootCategories[lootCategories.Length - 1].items;
    }

    private void DropItem(GameObject[] items)
    {
        if (items.Length > 0)
        {
            int randomIndex = Random.Range(0, items.Length);
            GameObject droppedItem = Instantiate(items[randomIndex], transform.position, Quaternion.identity);

            Animator itemAnimator = droppedItem.GetComponentInChildren<Animator>();
            if (itemAnimator != null)
            {
                // set ItemDropped trigger to start animation
                itemAnimator.SetTrigger("ItemDropped");
            }
        }
    }
}
