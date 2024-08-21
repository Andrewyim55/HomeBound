﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour
{
    [Header("UIScreens")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsUI;
    private bool isPaused = false;
    void Awake()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
    private void Start()
    {
        pauseMenuUI.SetActive(false);
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
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
