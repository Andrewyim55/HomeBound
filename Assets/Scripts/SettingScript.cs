using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SettingScript : MonoBehaviour
{
    public void CloseSettings()
    {
        //SceneManager.UnloadSceneAsync("Settings");
        if (SceneTracker.previousScene == "SampleScene")
        {
            Time.timeScale = 0f;
        }
        SceneManager.LoadScene(SceneTracker.previousScene);

        SceneTracker.previousScene = SceneManager.GetActiveScene().name;
    }
}
