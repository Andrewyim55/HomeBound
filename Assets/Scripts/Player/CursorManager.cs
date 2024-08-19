using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Texture2D cursorTexture;

    // CursorHotSpot is the point of the cursor 
    private Vector2 cursorHotSpot;
    // Start is called before the first frame update
    void Start()
    {
        // Call this to change the cursor
        cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
    }
}
