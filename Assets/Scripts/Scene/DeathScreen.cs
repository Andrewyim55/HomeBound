using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathScreenUI;
    void Awake()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
    public void NewGame()
    {
        print("New Game");
        Player.instance.isRestart = true;
        print(Player.instance.isRestart);
        Time.timeScale = 1f;
        SoundManager.instance.PlayBgm(1);
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Main Menu");

    }
}