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
    [SerializeField] private AudioClip toBossSceneClip;
    [SerializeField] private string gameText;
    [SerializeField] private string bossText;
    public static GameLogic instance;
    public float gameTime;
    public float timeToBoss;
    public bool isBossScene;
    private bool isInGame;
    private bool isPaused = false;
    public bool isSettings = false;
    public bool isTutorial = false;
    public bool isSceneChanging = false;
    public string objective;

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
                isTutorial = false;
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
                objective = gameText;
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
                isSceneChanging = false;
                GetComponent<EnemySpawner>().enabled = true;
                GetComponent<BreakablesSpawner>().enabled = true;
                GetComponent<DifficultyManager>().enabled = true;
                GetComponent<DifficultyManager>().Reset();
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                gameTime = 0f;
                isBossScene = false;
                isInGame = true;
                isTutorial = false;
                break;
            case 3:
                anim.SetTrigger("Start");
                objective = bossText;
                yield return new WaitForSecondsRealtime(1f);
                GetComponent<EnemySpawner>().enabled = false;
                GetComponent<DifficultyManager>().enabled = false;
                SceneManager.LoadSceneAsync("Boss Fight");
                SoundManager.instance.PlayBgm(2);
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                Player.instance.gameObject.GetComponent<Collider2D>().enabled = true;
                Player.instance.transform.position = new UnityEngine.Vector3(3.8f, -2.5f, 0);
                GameObject.FindGameObjectWithTag("MainCamera").transform.position = new UnityEngine.Vector3(3.8f, -2.5f, -2);
                anim.SetTrigger("End");
                yield return new WaitForSecondsRealtime(1f);
                isSceneChanging = false;
                break;
            case 4:
                isTutorial = true;
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("Tutorial");
                anim.SetTrigger("End");
                isSceneChanging = false;
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
            isSceneChanging = true;
            isBossScene = true;
            Player.instance.gameObject.GetComponent<Collider2D>().enabled = false;
            StartCoroutine(ToBossScene());
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


    IEnumerator ToBossScene()
    {
        SoundManager.instance.PlaySfx(toBossSceneClip, transform);
        yield return new WaitForSeconds(toBossSceneClip.length / 2);
        BossScene();
    }

}