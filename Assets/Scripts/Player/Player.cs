using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private GameObject aimArm;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioSource walkAudioSource;

    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] TrailRenderer tr;
    [SerializeField] private float flashDuration;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool canDash;
    private bool isDashing;
    private SkillCD skillCD;
    private bool isAlive;
    private Material originalMaterial;

    // store the weapon that the player is able to pick up
    private Weapon nearbyWeapon;

    void Start()
    {
        isAlive = true;
        canDash = true;
        isDashing = false;
        originalMaterial = sr.material;
        if (skillCD == null)
        {
            skillCD = GetComponent<SkillCD>();
        }
        walkAudioSource.clip = walkClip;
        walkAudioSource.loop = true;
        walkAudioSource.volume = SoundManager.instance.GetSFXVol();
    }
    private void Update()
    {
        if (!isAlive)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Basic WASD Movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // Animation to be done

        // If player is dashing player is not able to do other actions
        if (isDashing)
            return;

        // Attacking
        if (Input.GetMouseButtonDown(0))
        {
            if (weapon != null)
            {
                weapon.Fire();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (weapon != null)
            {
                weapon.StopFire();
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            weapon.Reload();
        }

        if (Input.GetButtonDown("Jump") && canDash)
        {
            StartCoroutine(Dash());
        }
        if(Input.GetButtonDown("Pickup"))
        {
            pickUpWeapon();
        }

        // Update Animator parameters
        animator.SetFloat("Speed", movement.magnitude);
    }
    private void FixedUpdate()
    {
        if (!isAlive)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (isDashing)
        {
            rb.MovePosition(rb.position + movement * dashingPower * Time.fixedDeltaTime);
            return;
        }

        // Moving of player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        RotatePlayer();
        if (movement.magnitude > 0)
        {
            if(!walkAudioSource.isPlaying)
                walkAudioSource.Play();
        }
        else
        {
            if(walkAudioSource.isPlaying)
                walkAudioSource.Stop();
        }
    }

    private void RotatePlayer()
    {
        // Rotation of weapon
        Vector2 aimDir = (mousePos - rb.position).normalized;
        float aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimArm.transform.eulerAngles = new Vector3(0, 0, aimAngle);

        if(aimDir.x < 0)
        {
            sr.flipX = true;
            if(weapon != null)
            {
                weapon.GetComponent<SpriteRenderer>().flipY = true;
            }
        }
        else if (aimDir.x > 0)
        {
            sr.flipX = false;
            if (weapon != null)
            {
                weapon.GetComponent<SpriteRenderer>().flipY = false;
            }
        }
    }

    public void TakeDmg(float _dmg)
    {
        health -= _dmg;
        UpdateHealthBar();
        StartCoroutine(flashEffect());
        if (health <= 0 && isAlive)
        {
            Die();
        }
    }
    private void UpdateHealthBar()
    {
        float fillAmount = health / 10;
        healthBarImage.fillAmount = fillAmount;
    }

    // Iframes after taking damage
    private IEnumerator flashEffect()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        sr.material = originalMaterial;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetTrigger("Dash");
        tr.emitting = true;
        SoundManager.instance.PlaySfx(dashClip, transform);
        Vector2 originalPosition = rb.position;
        Vector2 dashPosition = originalPosition + (movement * dashingPower);

        float elapsed = 0f;
        while (elapsed < dashingTime)
        {
            rb.MovePosition(Vector2.Lerp(originalPosition, dashPosition, elapsed / dashingTime));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(dashPosition);

        animator.ResetTrigger("Dash"); 
        tr.emitting = false;
        isDashing = false;

        skillCD.dashCooldown(dashingCooldown);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void pickUpWeapon()
    {
        if (nearbyWeapon != null)
        {
            // check if player already has a weapon
            if(weapon != null)
            {
                Destroy(weapon.gameObject);
            }
            weapon = nearbyWeapon;
            weapon.transform.SetParent(aimArm.transform);
            weapon.transform.localPosition = new Vector3(0, 0, 0);
            weapon.GetComponent<BoxCollider2D>().enabled = false;
            nearbyWeapon = null;
            weapon.transform.eulerAngles = aimArm.transform.eulerAngles;
        }
    }
    // If player die, call this function
    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("Death");
        if(weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon collidedWeapon = collision.GetComponent<Weapon>();
        if (collidedWeapon != null)
        {
            nearbyWeapon = collidedWeapon;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Weapon collidedWeapon = collision.GetComponent<Weapon>();
        if (collidedWeapon == nearbyWeapon)
        {
            nearbyWeapon = null;  // Clear the nearby weapon reference when leaving the collider
        }
    }
}
