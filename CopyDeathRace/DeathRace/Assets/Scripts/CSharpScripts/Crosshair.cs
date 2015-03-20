using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {
	
	int sizeX = 32;
	int sizeY = 32;
	
	
	Vector3 mousePos;
	int xDrawOff = 0;
	int yDrawOff = 0; 
	public int playerNumber = 1;
	public int xDraw = 0;
	public int yDraw = 0;
	string p_RAX, p_RAY;
	
	public Texture2D crosshair;
	// Use this for initialization
	void Awake () {
		if( playerNumber == 1 ) { 		p_RAX = "RAX1"; 	p_RAY = "RAY1"; }
		else if ( playerNumber == 2 ) { p_RAX = "RAX2"; 	p_RAY = "RAY2"; }
		Screen.showCursor = false;
		mousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		
		if( mousePos.x != Input.mousePosition.x || mousePos.y != Input.mousePosition.y )
			{ mousePos = Input.mousePosition; xDrawOff = 0; yDrawOff = 0;}
	
		xDrawOff += (int)(Input.GetAxis(p_RAX)*Time.deltaTime*400);
		yDrawOff += (int)(Input.GetAxis(p_RAY)*Time.deltaTime*400);
		xDraw = (int)mousePos.x + xDrawOff;
		yDraw = (int)mousePos.y - yDrawOff;
		if( xDraw < 0 ) xDraw = 0;
		if( yDraw < 0 ) yDraw = 0;
		if( xDraw > Screen.width ) xDraw = Screen.width;
		if( yDraw > Screen.height ) yDraw = Screen.height;
		//print (xDraw + " " + yDraw);
		
	}
	
	void OnGUI() 
	{
		GUI.DrawTexture ( new Rect(xDraw-sizeX/2,Screen.height-(yDraw+sizeY/2), sizeX , sizeY ),
							crosshair, ScaleMode.ScaleToFit, true, 1.0f );
	} 
	
}
