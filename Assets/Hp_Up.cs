using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_up : MonoBehaviour
{
  // HP to be implemented afterwards (this needs to be updated)
    private void OnTriggerEnter2D(Collider2D other){
        if (other.tag == "Player") {
            print("health up");
            gameObject.SetActive(false);
        }
    }
}
