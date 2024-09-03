using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;
    public float gameTime;
    public float timeToBoss;

    private bool isBossScene;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        isBossScene = false;
    }

    // 1 is main mene, 2 is sample scene, 3 is boss fight, 4 is tutorial
    public void ChangeScene(int sceneNum)
    {
        switch (sceneNum)
        {
            case 1:
                SceneManager.LoadScene("Main Menu");
                break;
            case 2:
                SceneManager.LoadScene("SampleScene");
                break;
            case 3:
                SceneManager.LoadScene("Boss Fight");
                break;
            case 4:
                SceneManager.LoadScene("Tutorial");
                break;
        }
    }

    public void NewGame()
    {
        SoundManager.instance.PlayBgm(1);
        if (Player.instance != null)
        {
            Destroy(Player.instance.gameObject);
        }
        ChangeScene(2);
        GetComponent<EnemySpawner>().enabled = true;
        GetComponent<BreakablesSpawner>().enabled = true;
        GetComponent<DifficultyManager>().enabled = true;
        Time.timeScale = 1f;
        GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
        gameTime = 0f;
        isBossScene = false;
    }

    public void MainMenu()
    {
        SoundManager.instance.PlayBgm(0);
        ChangeScene(0);
        if (Player.instance != null)
        {
            Destroy(Player.instance.gameObject);
        }
        ChangeScene(2);
        GetComponent<EnemySpawner>().enabled = false;
        GetComponent<BreakablesSpawner>().enabled = false;
        GetComponent<DifficultyManager>().enabled = false;
        isBossScene = false;
    }

    // function will change to the boss scene
    public void BossScene()
    {
        ChangeScene(3);
    }

    private void Update()
    {
        // Increment the timeAlive by deltaTime each frame
        gameTime += Time.deltaTime;

        if (gameTime >= timeToBoss && !isBossScene)
        {
            isBossScene = true;
            BossScene();
        }
    }
}
