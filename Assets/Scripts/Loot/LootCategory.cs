using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LootCategory : ScriptableObject
{
        public string categoryName; 
        public GameObject[] items;
        public int dropChance;

}
