using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingScript : MonoBehaviour
{
    [Header("UIScreens")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject settingsUI;

    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        settingsUI.SetActive(false);
        BGMSlider.value = SoundManager.instance.GetBGMVol();
        SFXSlider.value = SoundManager.instance.GetSFXVol();
        BGMSlider.onValueChanged.AddListener(ChangeBGMVolume);
        SFXSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }
    public void ChangeBGMVolume(float value)
    {
        SoundManager.instance.SetBGMVol(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    // Method to change SFX volume
    public void ChangeSFXVolume(float value)
    {
        SoundManager.instance.SetSFXVol(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void CloseSettings()
    {
        MainUI.SetActive(true);
        settingsUI.SetActive(false);

    }
}
