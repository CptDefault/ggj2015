using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour {
	private static PauseScreen _instance;
	public static PauseScreen Instance {get {return _instance;}}
	public TweenScale scale;

	private bool _paused;

	public AudioClip open;
	public AudioClip close;

	private void Awake() {
		_instance= this;
		//gameObject.SetActive (false);
	}
	
	public static void TogglePause() {
		_instance.TogglePauseInt();
	}

	public void TogglePauseInt() {
		_paused = !_paused;

		if(_paused) {
			//_instance.gameObject.SetActive (true);
			Time.timeScale = 0;
			scale.PlayForward();
			audio.PlayOneShot(open);
		} else {
			Time.timeScale = 1;
			audio.PlayOneShot(close);
			StartCoroutine(AnimateOut());
		}
	}

	IEnumerator AnimateOut() {
		scale.PlayReverse();

		yield return new WaitForSeconds(0.3f);
		//gameObject.SetActive (false);
	}
}
