using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] public AudioClip victoryClip;
    [SerializeField] private Animator anim;
    public static GameLogic instance;
    public float gameTime;
    public float timeToBoss;
    public bool isBossScene;
    private bool isInGame;
    private bool isPaused = false;
    public bool isSettings = false;

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
    public IEnumerator ChangeScene(int sceneNum)
    {
        switch (sceneNum)
        {
            case 1:
                anim.SetTrigger("Start");
                GetComponent<EnemySpawner>().enabled = false;
                GetComponent<BreakablesSpawner>().enabled = false;
                GetComponent<DifficultyManager>().enabled = false;
                isBossScene = false;
                isInGame = false;
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("Main Menu");
                SoundManager.instance.PlayBgm(0);
                if (Player.instance != null)
                {
                    Destroy(Player.instance.gameObject);
                }
                anim.SetTrigger("End");

                break;
            case 2:
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(1f);
                SceneManager.LoadSceneAsync("SampleScene");
                SoundManager.instance.PlayBgm(1);
                if (Player.instance != null)
                {
                    Destroy(Player.instance.gameObject);
                }
                SetPaused(true);
                anim.SetTrigger("End");
                yield return new WaitForSecondsRealtime(1f);
                SetPaused(false);
                GetComponent<EnemySpawner>().enabled = true;
                GetComponent<BreakablesSpawner>().enabled = true;
                GetComponent<DifficultyManager>().enabled = true;
                GetComponent<DifficultyManager>().Reset();
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                gameTime = 0f;
                isBossScene = false;
                isInGame = true;
                break;
            case 3:
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(1f);
                GetComponent<EnemySpawner>().enabled = false;
                GetComponent<DifficultyManager>().enabled = false;
                SceneManager.LoadSceneAsync("Boss Fight");
                SoundManager.instance.PlayBgm(2);
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                Player.instance.transform.position = new UnityEngine.Vector3(3.8f, -2.5f, 0);
                GameObject.FindGameObjectWithTag("MainCamera").transform.position = new UnityEngine.Vector3(3.8f, -2.5f, -2);
                anim.SetTrigger("End");
                yield return new WaitForSecondsRealtime(1f);
                break;
            case 4:
                Debug.Log("TUT");
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("Tutorial");
                anim.SetTrigger("End");
                break;
        }
    }

    public void NewGame()
    {
        StartCoroutine(ChangeScene(2));
    }

    public void MainMenu()
    {
        StartCoroutine(ChangeScene(1));
    }

    // function will change to the boss scene
    public void BossScene()
    {
        StartCoroutine(ChangeScene(3));
    }
    
    public void TutorialScene()
    {
        StartCoroutine(ChangeScene(4));
    }

    private void Update()
    {
        // Increment the timeAlive by deltaTime each frame
        if (!isInGame)
            return;

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else if (!isPaused)
        {
            Time.timeScale = 1f;
        }

        if (Player.instance != null)
        {
            if(Player.instance.getStatus())
            {
                gameTime += Time.deltaTime;
            }
        }

        if (gameTime >= timeToBoss && !isBossScene)
        {
            isBossScene = true;
            BossScene();
        }
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