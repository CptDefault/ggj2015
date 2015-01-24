using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private UISprite sprite; 

	// Use this for initialization
	void Start () {
		sprite = GetComponent<UISprite>();
	}

	public void SetHealth(float amount) {
		sprite.fillAmount = amount;
	}
	

}
