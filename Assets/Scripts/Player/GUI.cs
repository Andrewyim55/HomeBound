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
    [SerializeField] public GameObject winScreenUI;
    [SerializeField] public GameObject bossHealthBarPanel;
    [SerializeField] public Image bossHealthBarImage;
    [SerializeField] public AudioClip levelUpClip;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsUI;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuUI.SetActive(false);
        deathScreenUI.SetActive(false);
        UpdatePlayerWeapon();
        UpdateAmmoCount();
    }
    private void Update()
    {
        UpdateHealthBar();
        if (Player.instance != null)
        {
            if(GameLogic.instance.isTutorial)
            {
                timerText.gameObject.SetActive(false);
            }

            UpdateTimerUI();
            UpdateAmmoCount();
            UpdatePlayerWeapon();
        }

        if(GameLogic.instance.isBossScene)
        {
            if(bossHealthBarPanel != null)
            {
                bossHealthBarPanel.SetActive(true);
                UpdateBossHealthBar();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameLogic.instance.isSettings) // not in settings
            {
                if (!pauseMenuUI.activeSelf)
                {
                    Pause();
                }
                else
                {
                    if (!Player.instance.isLeveling)
                    {
                        Resume();
                    }
                    pauseMenuUI.SetActive(false);
                }

            }
            else //in settings
            {
                GameLogic.instance.isSettings = false;
                pauseMenuUI.SetActive(true);
                settingsUI.SetActive(false);
            }
        }
    }
    public void Resume()
    {
        SoundManager.instance.playButtonSound();
        pauseMenuUI.SetActive(false);
        GameLogic.instance.SetPaused(false);
    }

    public void RestartGame()
    {
        SoundManager.instance.playButtonSound();
        GameLogic.instance.SetPaused(false);
        pauseMenuUI.SetActive(false);
        GameLogic.instance.NewGame();
    }

    public void Pause()
    {
        SoundManager.instance.playButtonSound();

        pauseMenuUI.SetActive(true);
        GameLogic.instance.SetPaused(true);
    }
    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        GameLogic.instance.isSettings = true;
    }
    public void outOfSettings()
    {
        SoundManager.instance.playButtonSound();
        GameLogic.instance.isSettings = false;
    }
    public void QuitGame()
    {
        SoundManager.instance.playButtonSound();
        pauseMenuUI.SetActive(false);
        GameLogic.instance.MainMenu();
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

    public void UpdateBossHealthBar()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            float fillAmount = boss.GetComponent<ReaperBoss>().GetHealth() / boss.GetComponent<ReaperBoss>().GetMaxHealth();
            bossHealthBarImage.fillAmount = fillAmount;
        }
        else
        {
            bossHealthBarPanel.SetActive(false);
        }
    }

    private void UpdateTimerUI()
    {
        // Update the timer UI (e.g., minutes:seconds:milliseconds format)
        int minutes = Mathf.FloorToInt(GameLogic.instance.gameTime / 60);
        int seconds = Mathf.FloorToInt(GameLogic.instance.gameTime % 60);
        int milliseconds = Mathf.FloorToInt((GameLogic.instance.gameTime * 1000) % 1000);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }

    private void UpdatePlayerWeapon()
    {
        if (Player.instance.GetWeapon() != null)
        {
            weaponDisplay.sprite = Player.instance.GetWeapon().gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void LevelUP()
    {
        GameLogic.instance.SetPaused(true);
        SoundManager.instance.PlaySfx(levelUpClip, transform);
        StartCoroutine(LevelUpPanel.instance.UpdateLevelUpUI());
        (float experience, float xpNeeded, float level) = Player.instance.GetExperience();
        xpText.text = experience + " / " + xpNeeded;
        levelText.text = level.ToString();
    }
    public void UpdateXPBar()
    {
        (float experience,float xpNeeded, float level) = Player.instance.GetExperience();
        xpText.text = experience + " / " + xpNeeded;
        levelText.text = level.ToString();
        float currentXP = Mathf.Clamp(experience, 0, xpNeeded);
        float fillAmount = currentXP / xpNeeded; // Calculate the fill amount as a fraction of current health over max health
        xpBarImage.fillAmount = fillAmount; // Set the fill amount of the health bar image
    }

    public void UpdateAmmoCount()
    {
        ammoCount.text = Player.instance.GetWeapon().magazineSize + "/" + Player.instance.GetWeapon().magSize;
    }
}
