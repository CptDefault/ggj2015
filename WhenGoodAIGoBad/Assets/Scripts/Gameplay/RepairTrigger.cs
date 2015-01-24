using UnityEngine;
using System.Collections;
using System;

public class RepairTrigger : MonoBehaviour {
	public Tool.ToolType ToolRequired;

	public GameObject HealthBarPrefab;
	public float Health = 1;
	public HealthBar myBar; 

	public Action<float> OnIncrementHealth;


	void Start () {
		if(myBar == null)
			myBar = (GameObject.Instantiate(HealthBarPrefab) as GameObject).GetComponent<HealthBar>();

		myBar.transform.parent = UICamera.mainCamera.transform.parent;
		myBar.transform.localScale = HealthBarPrefab.transform.localScale;

		OnIncrementHealth += myBar.SetHealth;
		myBar.SetHealth(Health);

		Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.x -= (Screen.width/ 2.0f);
		screenPos.y -= (Screen.height / 2.0f);
		screenPos.y += Screen.height/15f;
		myBar.transform.localPosition = screenPos;
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
