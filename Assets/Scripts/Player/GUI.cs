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
        UpdateHealthBar();
        UpdateTimerUI();
    }

    public void UpdateHealthBar()
    {
        float fillAmount = Player.instance.GetHealth() / Player.instance.GetMaxHealth();
        healthBarImage.fillAmount = fillAmount;
        healthText.text = Player.instance.GetHealth() + "/" + Player.instance.GetMaxHealth();
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
}
