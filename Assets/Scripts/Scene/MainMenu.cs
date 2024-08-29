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
        GameLogic.instance.NewGame();
    }
    public void Settings()
    {
        MainUI.SetActive(false);
        settingsUI.SetActive(true);
    }
    public void Tutorial()
    {
        GameLogic.instance.ChangeScene(4);
    }
}
