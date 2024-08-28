using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNLevel : MonoBehaviour
{
    public static XPNLevel instance;

    [Header("Level and XP Attributes")]
    [SerializeField] private Text XPText;
    [SerializeField] private Text LevelText;
    [SerializeField] private Image xpBarImage;
    [SerializeField] private GameObject LevelUpScreen;
    [SerializeField] public GameObject ButtonUI;
    [SerializeField] public GameObject TextUI;
    [SerializeField] protected AudioClip LevelUpClip;

    private LevelUpPanel LevelUpPanelScript;
    private int XPLevel = 1;
    private float currentXP;
    private float maxExperience = 100;
    private float levelEXPNeeded;
    public bool isLeveling;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        levelEXPNeeded = maxExperience * XPLevel;
        currentXP = 0;
        UpdateXPBar();
        LevelUpPanelScript = LevelUpScreen.GetComponent<LevelUpPanel>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            gainXP(10);
        }
    }

    public void gainXP(float experience)
    {

        currentXP += experience;
        if (currentXP >= levelEXPNeeded)
        {
            isLeveling = true;
            LevelUP();
        }
        currentXP = Mathf.Clamp(currentXP, 0, levelEXPNeeded);
        UpdateXPBar();
    }

    private void LevelUP()
    {
        PauseScript.instance.SetPaused(true);
        SoundManager.instance.PlaySfx(LevelUpClip, transform);
        StartCoroutine(LevelUpPanelScript.UpdateLevelUpUI());

        currentXP = 0;
        XPLevel += 1;
        levelEXPNeeded = maxExperience * XPLevel;
        LevelText.text = XPLevel.ToString();
        PauseScript.instance.SetPaused(true);
    }

    private void UpdateXPBar()
    {
        XPText.text = currentXP.ToString() + " / " + levelEXPNeeded;
        float fillAmount = currentXP / levelEXPNeeded; // Calculate the fill amount as a fraction of current health over max health
        xpBarImage.fillAmount = fillAmount; // Set the fill amount of the health bar image
    }
}
