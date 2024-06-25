using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XPNLevel : MonoBehaviour
{
    public Text XPText;
    public Text LevelText;
    public Image xpBarImage;
    private int XPLevel = 1;
    private float currentXP;
    private float maxExperience = 100;
    // Start is called before the first frame update
    void Start()
    {
        currentXP = 0;
        UpdateXPBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            gainXP(10);
            print(currentXP);
        }
    }
    public void gainXP(float experience)
    {

        currentXP += experience;
        if (currentXP >= maxExperience)
        {
            currentXP = 0;
            XPLevel += 1;
            LevelText.text = XPLevel.ToString();
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
