using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingScript : MonoBehaviour
{
    [Header("UIScreens")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject settingsUI;
    private void Start()
    {
        //settingsUI.SetActive(false);
    }
    public void CloseSettings()
    {
        MainUI.SetActive(true);
        settingsUI.SetActive(false);
    }
}
