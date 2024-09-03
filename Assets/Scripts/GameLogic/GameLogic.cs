using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] public AudioClip victoryClip;
    public static GameLogic instance;
    public float gameTime;
    public float timeToBoss;
    public bool isBossScene;
    private bool isInGame;

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
        isInGame = false;
    }

    // 1 is main mene, 2 is sample scene, 3 is boss fight, 4 is tutorial
    public void ChangeScene(int sceneNum)
    {
        switch (sceneNum)
        {
            case 1:
                SoundManager.instance.PlayBgm(0);
                SceneManager.LoadScene("Main Menu");
                break;
            case 2:
                SoundManager.instance.PlayBgm(1);
                SceneManager.LoadScene("SampleScene");
                break;
            case 3:
                SoundManager.instance.PlayBgm(2);
                SceneManager.LoadScene("Boss Fight");
                break;
            case 4:
                SoundManager.instance.PlayBgm(0);
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
        GetComponent<DifficultyManager>().Reset();
        Time.timeScale = 1f;
        GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
        gameTime = 0f;
        isBossScene = false;
        isInGame = true;
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
        isInGame = false;
    }

    // function will change to the boss scene
    public void BossScene()
    {
        GetComponent<EnemySpawner>().enabled = false;
        GetComponent<DifficultyManager>().enabled = false;
        GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
        ChangeScene(3);
    }

    private void Update()
    {
        // Increment the timeAlive by deltaTime each frame
        if (!isInGame)
            return;

        gameTime += Time.deltaTime;

        if (gameTime >= timeToBoss && !isBossScene)
        {
            isBossScene = true;
            BossScene();
        }
    }
}
