using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro textMesh;

    [Header("Attributes")]
    [SerializeField] private float disappearTimer;
    [SerializeField] private float disappearSpeed;

    public static DamagePopUp instance;
    private Color textColor;

    // Start is called before the first frame update
    void Start()
    {
        textColor = textMesh.color;
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        if(disappearTimer <= 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetValue(float damageAmt)
    {
        textMesh.SetText(damageAmt.ToString());
    }
}
