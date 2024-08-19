using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    enum powerUp
    {
        health,
        speed,
        bomb
    }
    [Header("Attributes")]
    [SerializeField] private powerUp type;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            switch (type)
            {
            case powerUp.health:
                    Debug.Log("Health Pick Up");
                    break;
            case powerUp.speed:
                    Debug.Log("Speed Pick Up");
                    break;
            case powerUp.bomb:
                    Debug.Log("Bomb Pick Up");
                    break;
            }
            gameObject.SetActive(false);
        }
    }
}
