using UnityEngine;
using System.Collections;

public class RepairTrigger : MonoBehaviour {
	public Tool.ToolType ToolRequired;

	public float Health = 100;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
	    /*if(other.gameObject.tag == "Tool") {
	    	var tool = other.gameObject;
	    	if(tool.GetComponent<Tool>().Type == ToolRequired) {
	    		Repair();
	    		Destroy(tool);
	    	}
	    }*/

	    if(other.gameObject.tag == "Player"){
	    	other.gameObject.GetComponent<PlayerInput>().InitRepairs(this, true);
	    }
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "Player"){
			other.gameObject.GetComponent<PlayerInput>().InitRepairs(null, false);
		}
	}

	public void Repair() {
		Health = 100;
		audio.Play();
		// play some kind of sound
	}
}
