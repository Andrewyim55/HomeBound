using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNLevel : MonoBehaviour
{
    [Header("Level and XP Attributes")]
    [SerializeField] private Text XPText;
    [SerializeField] private Text LevelText;
    [SerializeField] private Image xpBarImage;
    [SerializeField] private GameObject LevelUpUI;
    [SerializeField] protected AudioClip LevelUpClip;
    private int XPLevel = 1;
    private float currentXP;
    private float maxExperience = 100;

    // Start is called before the first frame update
    void Start()
    {
        LevelUpUI.SetActive(false);
        currentXP = 0;
        UpdateXPBar();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    gainXP(10);
        //}
    }
    public void gainXP(float experience)
    {

        currentXP += experience;
        if (currentXP >= maxExperience)
        {
            PauseScript.instance.SetPaused(true);
            SoundManager.instance.PlaySfx(LevelUpClip,transform);
            LevelUpUI.SetActive(true);
            currentXP = 0;
            XPLevel += 1;
            LevelText.text = XPLevel.ToString();
            Time.timeScale = 0f;

        }
        currentXP = Mathf.Clamp(currentXP, 0, maxExperience);
        UpdateXPBar();
    }

    private void UpdateXPBar()
    {
        XPText.text = currentXP.ToString() + " / 100";
        float fillAmount = currentXP / maxExperience; // Calculate the fill amount as a fraction of current health over max health
        xpBarImage.fillAmount = fillAmount; // Set the fill amount of the health bar image
    }
}
