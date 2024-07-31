﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Start()
    {
        if (SceneTracker.previousScene == "Settings")
        {
            pauseMenuUI.SetActive(true);
        }
        else {
        pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        print("Resume");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        print("Restart");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pauseMenuUI.SetActive(false);
    }

    void Pause()
    {
        Time.timeScale = 0f;
        print("Pause");
        pauseMenuUI.SetActive(true);
        
        isPaused = true;
    }
    public void Settings()
    {
        print("Settings");
        Time.timeScale = 1f;
        SceneTracker.previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Settings");

    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
