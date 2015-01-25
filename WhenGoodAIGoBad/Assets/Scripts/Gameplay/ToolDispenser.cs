using UnityEngine;
using System.Collections;
using System;


public class ToolDispenser : MonoBehaviour {

	public GameObject ToolPrefab;
	public Transform DispensePoint;

	public float ReloadTime = 5;
	public float ReloadTimer = 0;
	public bool ToolTaken;

	void Start() {
		//SpawnTool();
	}

	public void SpawnTool() {
		if (!ToolTaken) {
			GameObject tool = (GameObject)GameObject.Instantiate (ToolPrefab, transform.position, Quaternion.identity);
			LeanTween.move(tool, DispensePoint.position, 0.25f).setEase(LeanTweenType.easeOutCubic);
			tool.GetComponent<Tool>().OnPickup += () => {ToolTaken = true;};
			audio.Play();
		}
	}

	void Update () {
		if (ToolTaken) {
			ReloadTimer += Time.deltaTime;
			if (ReloadTimer >= ReloadTime) {
				ToolTaken = false;
				ReloadTimer = 0;
				SpawnTool();
			}
		}
	}
}
