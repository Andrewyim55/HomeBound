using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCD : MonoBehaviour
{
    public Image CooldownImage;
    private float CDtime = 0f;
    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= CDtime)
        {
            CooldownImage.fillAmount = 1;
        }
        else
        {
            CooldownImage.fillAmount = elapsedTime / CDtime;
        }
    }

    public void dashCooldown(float time)
    {
        //print("Meow");
        elapsedTime = 0f;
        CDtime = time;
    }
}
