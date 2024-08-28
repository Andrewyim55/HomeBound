using System.Collections;
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
    private bool isSettings = false;
    public static PauseScript instance;

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
        if (instance == null)
            instance = this;
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else if(!isPaused)
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isSettings) // not in settings
            {
                if (!pauseMenuUI.activeSelf)
                {
                    Pause();
                }
                else
                {
                    if (!XPNLevel.instance.isLeveling)
                    {
                        Resume();
                    }
                    pauseMenuUI.SetActive(false);
                }

            }
            else //in settings
            {
                isSettings = false;
                pauseMenuUI.SetActive(true);
                settingsUI.SetActive(false);
            }
        }
    }

    public void Resume()
    {
        print("Resume");
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void RestartGame()
    {
        print("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        pauseMenuUI.SetActive(false);
    }

    public void Pause()
    {
        print("Pause");
        pauseMenuUI.SetActive(true);
        
        isPaused = true;
    }
    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        isSettings = true;
    }
    public void outOfSettings()
    {
        isSettings = false;
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }
    public void SetPaused(bool pause)
    {
        isPaused = pause;
    }

    public bool GetPaused()
    {
        return isPaused;
    }
}
