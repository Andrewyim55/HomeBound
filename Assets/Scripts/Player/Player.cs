using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] protected GameObject damagePopUpPrefab;

    [Header("Attributes")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashingCooldown;
    [SerializeField] TrailRenderer tr;
    [SerializeField] private float flashDuration;

    [Header("Pickup attributes")]
    [SerializeField] private float pickupRadius = 1f;
    [SerializeField] float minPickupSpeed = 0.2f;
    [SerializeField] private float maxPickupSpeed = 3f;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool canDash;
    private bool isDashing;
    //private SkillCD skillCD;
    private bool isAlive;
    private Material originalMaterial;

    private float ammoPercentage;
    private float increasedDmg;
    private float reloadSpd;
    private float dashReduce;
    private float timeAlive = 0f;
    // store the weapon that the player is able to pick up
    private Weapon nearbyWeapon;

    [Header("UI")]
    [SerializeField] public Text healthText;
    [SerializeField] public Image weaponDisplay;
    [SerializeField] public Image cooldownImage;
    [SerializeField] public GameObject deathScreenUI;
    [SerializeField] public GameObject LevelUpUI;
    [SerializeField] public Text timerText;
    [SerializeField] public Image healthBarImage;
    [SerializeField] public Text ammoCount;
    [SerializeField] public Animator skillCDAnimator;

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
        dashReduce = 1f;
        reloadSpd = 1f;
        increasedDmg = 1f;
        ammoPercentage = 1f;
        Time.timeScale = 1f;
        deathScreenUI.SetActive(false);
        isAlive = true;
        canDash = true;
        isDashing = false;
        originalMaterial = sr.material;
        walkAudioSource.clip = walkClip;
        walkAudioSource.loop = false;
        walkAudioSource.volume = SoundManager.instance.GetSFXVol();
        UpdateHealthBar();
    }
    private void Update()
    {
        if (!isAlive)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        // Increment the timeAlive by deltaTime each frame
        timeAlive += Time.deltaTime;
        // Update the timer UI
        UpdateTimerUI();

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
            if (weapon != null && !weapon.GetReloading() && !PauseScript.instance.GetPaused())
            {
                weapon.Fire();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (weapon != null && !weapon.GetReloading() && !PauseScript.instance.GetPaused())
            {
                weapon.StopFire();
            }
        }

        if (Input.GetButtonDown("Reload") && ammoCount.text != null)
        {
            weapon.StopFire();
            weapon.SetReloading(true);
            ammoCount.text = "Reloading";
        }

        if (Input.GetButtonDown("Jump") && canDash)
        {
            StartCoroutine(Dash());
        }
        if(Input.GetButtonDown("Pickup"))
        {
            pickUpWeapon();
        }
        if (weapon != null && ammoCount.text != null)
        {
            ammoCount.text = weapon.magazineSize + "/" + weapon.magSize;
        }
        
        AttractNearbyPickups();
        
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
        UpdateHealthBar();
        StartCoroutine(flashEffect());
        if (health <= 0 && isAlive)
        {
            StartCoroutine(Die());
        }
    }
    public void UpdateHealthBar()
    {
        float fillAmount = health / maxHealth;
        healthBarImage.fillAmount = fillAmount;
        healthText.text = health + "/" +maxHealth;
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
        skillCDAnimator.SetBool("isCoolDown", true);
        //skillCD.dashCooldown(dashingCooldown);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
        skillCDAnimator.SetBool("isCoolDown", false);
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
                weaponDisplay.sprite = weaponSpriteRenderer.sprite;
                weaponDisplay.color = new Color32(255, 255, 255, 255);

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
        isAlive = false;
        animator.SetTrigger("Death");
        if(weapon != null)
        {
            Destroy(weapon.gameObject);
        }
        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1);

        Time.timeScale = 0f;
        deathScreenUI.SetActive(true);
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
        print("lvlingup");
        if (type == "Speed")
        {
            PauseScript.instance.SetPaused(false);
            moveSpeed += value;
            LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
        }
        else if (type == "Max Health")
        {
            PauseScript.instance.SetPaused(false);
            health += value;
            maxHealth += value;
            LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            UpdateHealthBar();
        }
        else if (type == "Damage")
        {
            PauseScript.instance.SetPaused(false);
            LevelUpUI.SetActive(false);
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
            PauseScript.instance.SetPaused(false);
            LevelUpUI.SetActive(false);
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
            PauseScript.instance.SetPaused(false);
            LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            dashingCooldown *= dashReduce;
            dashReduce += value;
            dashingCooldown /= dashReduce;
        }
        else if (type == "Ammo Count")
        {
            PauseScript.instance.SetPaused(false);
            LevelUpUI.SetActive(false);
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
        float originalSpeed = moveSpeed;
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
    private void UpdateTimerUI()
    {
        // Update the timer UI (e.g., minutes:seconds:milliseconds format)
        int minutes = Mathf.FloorToInt(timeAlive / 60);
        int seconds = Mathf.FloorToInt(timeAlive % 60);
        int milliseconds = Mathf.FloorToInt((timeAlive * 1000) % 1000);
        if(timerText.text != null)
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    public void PlayerDied()
    {
        // Stop the timer when the player dies
        isAlive = false;

        // You can handle additional logic here, such as saving the time or displaying a "Game Over" screen
        Debug.Log($"Player survived for {timeAlive} seconds.");
    }
}
