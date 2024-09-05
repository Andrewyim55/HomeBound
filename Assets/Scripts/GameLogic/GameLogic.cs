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
                anim.SetTrigger("End");
                break;
            case 2:
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("SampleScene");
                SoundManager.instance.PlayBgm(1);
                if (Player.instance != null)
                {
                    Destroy(Player.instance.gameObject);
                }
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                anim.SetTrigger("End");
                GetComponent<EnemySpawner>().enabled = true;
                GetComponent<BreakablesSpawner>().enabled = true;
                GetComponent<DifficultyManager>().enabled = true;
                GetComponent<DifficultyManager>().Reset();
                Time.timeScale = 1f;
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                gameTime = 0f;
                isBossScene = false;
                isInGame = true;
                break;
            case 3:
                anim.SetTrigger("Start");
                GetComponent<EnemySpawner>().enabled = false;
                GetComponent<DifficultyManager>().enabled = false;
                GetComponent<BreakablesSpawner>().breakablesInScene.Clear();
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("Boss Fight");
                SoundManager.instance.PlayBgm(2);
                anim.SetTrigger("End");
                Player.instance.transform.position = new UnityEngine.Vector3(3.8f, -3f, 0);
                break;
            case 4:
                anim.SetTrigger("Start");
                yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
                SceneManager.LoadSceneAsync("Tutorial");
                SoundManager.instance.PlayBgm(0);
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
        if (Player.instance != null)
        {
            Destroy(Player.instance.gameObject);
        }
        StartCoroutine(ChangeScene(1));
    }

    // function will change to the boss scene
    public void BossScene()
    {
        StartCoroutine(ChangeScene(3));
    }

    private void Update()
    {
        // Increment the timeAlive by deltaTime each frame
        if (!isInGame)
            return;

        if(Player.instance != null)
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
}