using UnityEngine;
using System.Collections;
using InControl;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach (InputDevice input in InputManager.Devices) {
						if (input.MenuWasPressed)
								Application.LoadLevel (1);
				}
	}
}
