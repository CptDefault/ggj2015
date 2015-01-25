using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public UISprite sprite; 

	public UISprite needToolSprite;

	// Use this for initialization;
	public void SetRequiredSprite(Tool.ToolType type) {
		switch(type) {
			case Tool.ToolType.Generic:
				needToolSprite.spriteName = "generic";
				break;
			case Tool.ToolType.Battery:
				needToolSprite.spriteName = "battery";
				break;
			case Tool.ToolType.AIChip:
				needToolSprite.spriteName = "aiChip";
				break;
			default: 
				needToolSprite.spriteName = "generic";
				break;
		}
	}

	public void SetHealth(float amount) {
        gameObject.SetActive(amount < 0.99f);
		sprite.fillAmount = amount;

	}
	

}
