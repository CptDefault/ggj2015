using UnityEngine;
using System.Collections;
using System;

public class RepairTrigger : MonoBehaviour {
	public Tool.ToolType ToolRequired;

	public float Health = 1;
	public HealthBar myBar; 

	public Action<float> OnIncrementHealth;

	// Use this for initialization
	void Start () {
		OnIncrementHealth += myBar.SetHealth;
		myBar.SetHealth(Health);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {
	    if(other.gameObject.tag == "Player" && Health < 1f){
	    	other.gameObject.GetComponent<PlayerInput>().InitRepairs(this, true);
	    }
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.gameObject.tag == "Player"){
			other.gameObject.GetComponent<PlayerInput>().InitRepairs(null, false);
		}
	}

	public void Repair() {
		Health += 0.05f;
		OnIncrementHealth(Health);
	}

	public void CompleteRepair() {
		Health = 1f;
		audio.Play();
		// play some kind of sound
	}
}
