﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCD : MonoBehaviour
{
    private float CDtime = 0f;
    private float elapsedTime = 0f;
    private GUI gui;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(GUI.instance != null )
        {
            if (elapsedTime >= CDtime)
            {
                GUI.instance.CooldownImage.fillAmount = 1;
            }
            else
            {
                GUI.instance.CooldownImage.fillAmount = elapsedTime / CDtime;
            }
        }
    }

    public void dashCooldown(float time)
    {
        //print("Meow");
        elapsedTime = 0f;
        CDtime = time;
    }
}
