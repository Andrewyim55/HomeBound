using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    public RectTransform secondHand; // Reference to the RectTransform of the second hand
    public Image progressImage; // Reference to the Image component for the progress
    public float totalTime = 60f; // Total time in seconds for one full rotation

    private float elapsedTime = 0f; // Time elapsed since the start of the timer

    void Update()
    {
        // Update the elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate the rotation angle (360 degrees for a full circle)
        float rotationAngle = (elapsedTime / totalTime) * 360f;

        // Apply the rotation to the second hand
        secondHand.localRotation = Quaternion.Euler(0, 0, -rotationAngle);

        // Update the progress image fill amount
        progressImage.fillAmount = elapsedTime / totalTime;

        // Reset the timer if it completes one full rotation
        if (elapsedTime >= totalTime)
        {
            elapsedTime = 0f;
        }
    }
}
