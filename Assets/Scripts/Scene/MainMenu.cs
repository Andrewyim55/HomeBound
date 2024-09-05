using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UIScreens")]
    [SerializeField] private GameObject MainUI;
    [SerializeField] private GameObject settingsUI;
    public void PlayGame()
    {
        SoundManager.instance.playButtonSound();
        GameLogic.instance.NewGame();
    }
    public void Settings()
    {
        SoundManager.instance.playButtonSound();
        MainUI.SetActive(false);
        settingsUI.SetActive(true);
    }
    public void Tutorial()
    {
        SoundManager.instance.playButtonSound();
        GameLogic.instance.ChangeScene(4);
    }
}
