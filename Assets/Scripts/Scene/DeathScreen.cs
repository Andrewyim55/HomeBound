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
        GameLogic.instance.NewGame();
    }

    public void QuitGame()
    {
        GameLogic.instance.ChangeScene(1);
    }
}