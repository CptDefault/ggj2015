using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public UISprite sprite; 

	// Use this for initialization;

	public void SetHealth(float amount) {
		sprite.fillAmount = amount;
	}
	

}
