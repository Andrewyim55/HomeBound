using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public RectTransform timerHand;
    public Image progressImage; 
    public float totalTime = 60f;

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float rotationAngle = (elapsedTime / totalTime) * 360f;
        timerHand.localRotation = Quaternion.Euler(0, 0, -rotationAngle);
        progressImage.fillAmount = elapsedTime / totalTime;

        if (elapsedTime >= totalTime)
        {
            elapsedTime = 0f;
        }
    }
}
