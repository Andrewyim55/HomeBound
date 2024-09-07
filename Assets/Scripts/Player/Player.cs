using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player instance;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObject aimArm;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip bossDeathClip;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] protected GameObject damagePopUpPrefab;
    [SerializeField] protected GameObject pistolPrefab;


    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] TrailRenderer tr;
    [SerializeField] private float flashDuration;
    [SerializeField] private float experience;
    [SerializeField] private float level;
    [SerializeField] private float maxExperience;
    [SerializeField] public bool isLeveling;
    [Header("Pickup attributes")]
    [SerializeField] private float pickupRadius = 1f;
    [SerializeField] float minPickupSpeed = 0.2f;
    [SerializeField] private float maxPickupSpeed = 3f;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool canDash;
    private bool isDashing;
    private bool isAlive;
    private Material originalMaterial;

    private float ammoPercentage;
    private float increasedDmg;
    private float reloadSpd;
    private float dashReduce;
    // store the weapon that the player is able to pick up
    public Weapon nearbyWeapon;
    private bool onExit;
    private float originalSpeed;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        onExit = false;
        dashReduce = 1f;
        reloadSpd = 1f;
        increasedDmg = 1f;
        ammoPercentage = 1f;
        Time.timeScale = 1f;
        experience = 0f;
        maxExperience = 100f;
        level = 1;
        GUI.instance.UpdateXPBar();
        isAlive = true;
        canDash = true;
        isDashing = false;
        originalMaterial = sr.material;
        walkAudioSource.clip = walkClip;
        walkAudioSource.loop = false;
        walkAudioSource.volume = SoundManager.instance.GetSFXVol();
        originalSpeed = moveSpeed;

    }
    private void Update()
    {
        if (!isAlive)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (GameLogic.instance.isSceneChanging)
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0);
            weapon.StopFire();
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
            if (weapon != null && !weapon.GetReloading() && !GameLogic.instance.GetPaused())
            {
                //gainXP(20);
                weapon.Fire();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (weapon != null && !weapon.GetReloading() && !GameLogic.instance.GetPaused())
            {
                weapon.StopFire();
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            weapon.StopFire();
            weapon.SetReloading(true);
        }

        if (Input.GetButtonDown("Jump") && canDash)
        {
            StartCoroutine(Dash());
        }
        if(Input.GetButtonDown("Pickup") && onExit)
        {
            GameLogic.instance.MainMenu();
        }
        else if(Input.GetButtonDown("Pickup"))
        {
            pickUpWeapon();
        }
        AttractNearbyPickups();
        
        // Update Animator parameters
        animator.SetFloat("Speed", movement.magnitude);
    }
    private void FixedUpdate()
    {
        if (!isAlive || GameLogic.instance.isSceneChanging)
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
        if (movement.magnitude > 0 && isAlive)
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
        StartCoroutine(flashEffect());
        if (health <= 0 && isAlive)
        {
            StartCoroutine(Die());
        }
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
            if (!isAlive)
                yield break;
            rb.MovePosition(Vector2.Lerp(originalPosition, dashPosition, elapsed / dashingTime));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(dashPosition);

        animator.ResetTrigger("Dash"); 
        tr.emitting = false;
        isDashing = false;
        GUI.instance.skillCDAnimator.SetBool("isCoolDown", true);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        GUI.instance.skillCDAnimator.SetBool("isCoolDown", false);
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

            SpriteRenderer weaponSpriteRenderer = weapon.GetComponent<SpriteRenderer>();

            if (weaponSpriteRenderer != null)
            {
                weaponSpriteRenderer.enabled = true;
            }

            Transform childTransform = weapon.transform.Find("Sprite Anim");
            if (childTransform != null)
            {
                SpriteRenderer childSpriteRenderer = childTransform.GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null && childSpriteRenderer != weaponSpriteRenderer)
                {
                    childSpriteRenderer.enabled = false;
                }
            }

            SoundManager.instance.PlaySfx(pickUpClip, transform);
            weapon.transform.SetParent(aimArm.transform);
            weapon.transform.localPosition = new Vector3(0, 0, 0);
            weapon.GetComponent<BoxCollider2D>().enabled = false;
            nearbyWeapon = null;
            weapon.transform.eulerAngles = aimArm.transform.eulerAngles;
            weapon.bulletDmg *= increasedDmg;
            weapon.reloadSpeed *= reloadSpd;

            float magazineSizeFloat = (float)weapon.magSize;
            magazineSizeFloat *= ammoPercentage;
            weapon.magSize = Mathf.RoundToInt(magazineSizeFloat);
            /// ==========================================================================================================================================
        }
    }

    // If player die, call this function
    private IEnumerator Die()
    {
        Time.timeScale = 0f;
        isAlive = false;
        animator.SetTrigger("Death");
        SoundManager.instance.PlaySfx(deathSound, transform);
        SoundManager.instance.bgmSource.Stop();
        if(GameObject.FindObjectOfType<ReaperBoss>() !=null)
        {
           FindObjectOfType<ReaperBoss>().GetComponent<Collider2D>().enabled = false;
           SoundManager.instance.PlaySfx(bossDeathClip, transform);
           Debug.Log("Boss sound");
        }
        if(weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        GetComponent<BoxCollider2D>().enabled = false;
        
        rb.velocity = Vector3.zero;

        //StopAllEnemies();   
        yield return new WaitForSeconds(1);
        
        GUI.instance.deathScreenUI.SetActive(true);
    }

    private void StopAllEnemies()
    {
        // Find all objects with the "enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Loop through each enemy
        foreach (GameObject enemy in enemies)
        {
            // Get the Rigidbody component
            Rigidbody rb = enemy.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Set the velocity to zero
                rb.velocity = Vector3.zero;
            }
            else
            {
                Debug.LogWarning($"No Rigidbody found on {enemy.name}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Weapon collidedWeapon = collision.GetComponent<Weapon>();
        if (collidedWeapon != null)
        {
            nearbyWeapon = collidedWeapon;
        }
        if(collision.gameObject.layer == 17)
        {
            onExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Weapon collidedWeapon = collision.GetComponent<Weapon>();
        if (collidedWeapon == nearbyWeapon)
        {
            nearbyWeapon = null;  // Clear the nearby weapon reference when leaving the collider
        }
        if (collision.gameObject.layer == 17)
        {
            onExit = false;
        }
    }

    private void AttractNearbyPickups()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Exp"))
            {
                // calc distance and direction
                Vector2 direction = (transform.position - collider.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                
                // increase speed based on closeness to player
                float speed = Mathf.Lerp(maxPickupSpeed, minPickupSpeed, distance / pickupRadius);

                // move pickup towards player
                collider.transform.position = Vector2.MoveTowards(collider.transform.position, transform.position, speed * Time.deltaTime);
            }
        }
    }

    public void levelUp(string type, float value)
    {
        if (type == "Speed")
        {
            GameLogic.instance.SetPaused(false);
            originalSpeed += value;
            moveSpeed += value;
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else if (type == "Max Health")
        {
            GameLogic.instance.SetPaused(false);
            health += value;
            maxHealth += value;
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else if (type == "Damage")
        {
            GameLogic.instance.SetPaused(false);
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            if (weapon != null)
            {
                weapon.bulletDmg /= increasedDmg;
                increasedDmg += value;
                weapon.bulletDmg *= increasedDmg;
            }
            else
            {
                increasedDmg += value;
            }
        }
        else if (type == "Reload Speed")
        {
            GameLogic.instance.SetPaused(false);
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            if (weapon != null)
            {
                weapon.reloadSpeed /= reloadSpd;
                reloadSpd += value;
                weapon.reloadSpeed *= reloadSpd;
            }
            else
            {
                reloadSpd += value;
            }
        }
        else if (type == "Dash Cooldown")
        {
            GameLogic.instance.SetPaused(false);
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            dashingCooldown *= dashReduce;
            dashReduce += value;
            dashingCooldown /= dashReduce;
        }
        else if (type == "Ammo Count")
        {
            GameLogic.instance.SetPaused(false);
            GUI.instance.LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            if (weapon != null)
            {
                float magazineSizeFloat = (float)weapon.magSize;
                magazineSizeFloat /= ammoPercentage;
                ammoPercentage += value;
                magazineSizeFloat *= ammoPercentage;

                weapon.magSize = Mathf.RoundToInt(magazineSizeFloat);
            }
            else
            {
                ammoPercentage += value;
            }
        }
        isLeveling = false;
    }
    public bool GetAlive()
    {
        return isAlive;
    }

    public void SetHealth(float hp)
    {
        health = hp;
    }

    public float GetHealth()
    {
        return health;
    }
    public void SetMaxHealth(float hp)
    {
        maxHealth = hp;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        moveSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed;
    }

    public void ApplyMagnet(float duration)
    {
        StartCoroutine(magnetCoroutine(duration));
    }

    private IEnumerator magnetCoroutine(float duration)
    {
        float originalPickupRadius = pickupRadius;
        float originalMinPickupSpeed = minPickupSpeed;
        float originalMaxPickupSpeed = maxPickupSpeed;

        pickupRadius = 500;
        minPickupSpeed = 10;
        maxPickupSpeed = 20;

        yield return new WaitForSeconds(duration);

        pickupRadius = originalPickupRadius;
        minPickupSpeed = originalMinPickupSpeed;
        maxPickupSpeed = originalMaxPickupSpeed;
    }

    public Weapon GetWeapon()
    {
        return weapon;
    }

    public void gainXP(float value)
    {
        float levelEXPNeeded = maxExperience * level;
        experience += value;
        if (experience >= levelEXPNeeded)
        {
            experience = 0;
            level += 1;
            isLeveling = true;
            GUI.instance.LevelUP();
        }
        
        GUI.instance.UpdateXPBar();
    }

    public (float experience, float xpNeeded, float level) GetExperience()
    {
        float levelEXPNeeded = maxExperience * level;
        return (experience, levelEXPNeeded, level);
    }
    public bool getStatus()
    {
        return isAlive;
    }

    private void Win()
    {
        Time.timeScale = 0f;
        isAlive = false;
        if (weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        GetComponent<BoxCollider2D>().enabled = false;

        rb.velocity = Vector3.zero;

        GUI.instance.winScreenUI.SetActive(true);
    }
}
