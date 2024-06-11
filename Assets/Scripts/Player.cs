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
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] TrailRenderer tr;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool canDash;
    private bool isDashing;

    void Start()
    {
        canDash = true;
        isDashing = false;
    }
    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Basic WASD Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // Animation to be done

        // If player is dashing player is not able to do other actions
        if(isDashing)
        {
            return;
        }

        // Attacking
        if(Input.GetMouseButtonDown(0))
        {
            weapon.Fire();
        }

        if(Input.GetButtonDown("Jump") && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

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

    private IEnumerator Dash()
    {
        Debug.Log("dash");
        canDash = false;
        isDashing = true;
        rb.MovePosition(rb.position + (movement * dashingPower));
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
