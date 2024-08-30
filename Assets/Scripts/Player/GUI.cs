using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public static GUI instance;

    [Header("UI")]
    [SerializeField] public Text healthText;
    [SerializeField] public Image weaponDisplay;
    [SerializeField] public Image cooldownImage;
    [SerializeField] public GameObject deathScreenUI;
    [SerializeField] public GameObject LevelUpUI;
    [SerializeField] public Text timerText;
    [SerializeField] public Image healthBarImage;
    [SerializeField] public Image xpBarImage;
    [SerializeField] public Text xpText;
    [SerializeField] public Text levelText;
    [SerializeField] public Text ammoCount;
    [SerializeField] public Animator skillCDAnimator;
    [SerializeField] public Image CooldownImage;

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

    // Start is called before the first frame update
    void Start()
    {
        deathScreenUI.SetActive(false);
        Player.instance.skillCDAnimator = cooldownImage.GetComponent<Animator>();
    }
    private void Update()
    {
        if (Player.instance != null)
        {
            print(Player.instance);
        }
        UpdateHealthBar();
        UpdateTimerUI();
    }

    public void UpdateHealthBar()
    {
        if (Player.instance != null)
        {
            float fillAmount = Player.instance.GetHealth() / Player.instance.GetMaxHealth();
            healthBarImage.fillAmount = fillAmount;
            healthText.text = Player.instance.GetHealth() + "/" + Player.instance.GetMaxHealth();
        }
    }

    private void UpdateTimerUI()
    {
        // Update the timer UI (e.g., minutes:seconds:milliseconds format)
        int minutes = Mathf.FloorToInt(GameLogic.instance.GameTime / 60);
        int seconds = Mathf.FloorToInt(GameLogic.instance.GameTime % 60);
        int milliseconds = Mathf.FloorToInt((GameLogic.instance.GameTime * 1000) % 1000);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    private void UpdatePlayerWeapon()
    {
        if(Player.instance.GetWeapon() != null)
        {
            weaponDisplay.sprite = Player.instance.GetWeapon().gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void LevelUP()
    {
        PauseScript.instance.SetPaused(true);
        //SoundManager.instance.PlaySfx(LevelUpClip, transform);
        StartCoroutine(LevelUpPanel.instance.UpdateLevelUpUI());
        (float experience, float xpNeeded, float level) = Player.instance.GetExperience();
        xpText.text = experience + " / " + xpNeeded;
        levelText.text = level.ToString();
        PauseScript.instance.SetPaused(true);
    }
    public void UpdateXPBar()
    {
        (float experience,float xpNeeded, float level) = Player.instance.GetExperience();
        xpText.text = experience + " / " + xpNeeded;
        float currentXP = Mathf.Clamp(experience, 0, xpNeeded);
        float fillAmount = currentXP / xpNeeded; // Calculate the fill amount as a fraction of current health over max health
        xpBarImage.fillAmount = fillAmount; // Set the fill amount of the health bar image
    }
}
