using UnityEngine;
using System.Collections;

public class FoodDispenserVisual : MonoBehaviour {
	public Transform[] points;
	public GameObject foodPlate;
	
	public void Dispense() {
		foreach(Transform point in points) {
			var plate = (GameObject)GameObject.Instantiate(foodPlate, transform.position, Quaternion.identity);
			LeanTween.move(plate, point.position, 0.5f).setEase(LeanTweenType.easeOutCubic);
		}
	}
}
