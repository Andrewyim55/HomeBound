using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChanger : MonoBehaviour
{
    public Image imageDisplay;
    public Sprite[] pictures;
    private int currentPictureIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(pictures.Length > 0)
        {
            imageDisplay.sprite = pictures[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentPictureIndex = (currentPictureIndex + 1) % pictures.Length;
            imageDisplay.sprite = pictures[currentPictureIndex];
            
        }
    }
}
