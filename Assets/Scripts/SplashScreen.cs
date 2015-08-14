using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {
	
	public GUISkin skin;
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.GameOver)
			GameManager.Instance.initLevel ();
	}

	void OnGUI()
	{
		GUI.skin = skin;
		GUI.Label (new Rect ((Screen.width-275)/2, 10, 275, 45), "Traffic Control");
		if (GUI.Button (new Rect ((Screen.width-400)/2, 150, 400, 100), "Play")) {
			Application.LoadLevel(1);
		}
		
		if (GUI.Button (new Rect ((Screen.width-400)/2, 300, 400, 100), "Quit")) {
			Application.Quit();
		}
	}
}
