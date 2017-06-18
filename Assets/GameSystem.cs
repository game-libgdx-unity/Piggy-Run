using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    public Texture2D cursor;
	// Use this for initialization
	void Start () {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	} 
}
