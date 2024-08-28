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
        SceneManager.LoadScene("SampleScene");

    }
    public void Settings()
    {
        MainUI.SetActive(false);
        settingsUI.SetActive(true);
    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");

    }
}
