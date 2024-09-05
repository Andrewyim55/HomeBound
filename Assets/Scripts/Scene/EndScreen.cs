using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class EndScreen : MonoBehaviour
{
    public GameObject panel;
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
        panel.SetActive(false);
        GameLogic.instance.NewGame();
    }

    public void QuitGame()
    {
        panel.SetActive(false);
        GameLogic.instance.MainMenu();
    }
}
