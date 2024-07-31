using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {

        SceneTracker.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SampleScene");

    }
    public void Settings()
    {
        SceneTracker.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Settings");

    }
}
