using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingScript : MonoBehaviour
{
    [Header("UIScreens")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject settingsUI;

    [Header("Audio")]
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        settingsUI.SetActive(false);
        BGMSlider.value = BGMSource.volume;
        SFXSlider.value = SFXSource.volume;
        BGMSlider.onValueChanged.AddListener(ChangeBGMVolume);
        SFXSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }
    public void ChangeBGMVolume(float value)
    {
        BGMSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    // Method to change SFX volume
    public void ChangeSFXVolume(float value)
    {
        SFXSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void CloseSettings()
    {
        MainUI.SetActive(true);
        settingsUI.SetActive(false);

    }
}
