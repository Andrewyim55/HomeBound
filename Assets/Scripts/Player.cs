using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Weapon weapon;

    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;

    private Vector2 movement;
    private Vector2 mousePos;
    void Start()
    {

    }
    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Basic WASD Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // Animation to be done


        // Attacking
        if(Input.GetMouseButtonDown(0))
        {
            weapon.Fire();
        }
    }
    private void FixedUpdate()
    {
        // Moving of player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        
        // Rotation of weapon
        Vector2 aimDir = (mousePos - rb.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        weapon.transform.parent.transform.eulerAngles = new Vector3(0,0,aimAngle);
    }

    public void TakeDmg(float _dmg)
    {
        health -= _dmg;
    }
}
