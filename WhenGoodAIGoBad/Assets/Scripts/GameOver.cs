using UnityEngine;
using System.Collections;
using InControl;

public class GameOver : MonoBehaviour {
	public UILabel Label;

	public GameObject Explosions;

	private TweenAlpha _alpha;
	public GameObject TryAgainAlpha;

	private bool _gameOver;

	// Use this for initialization
	void Start () {
		_alpha = GetComponent<TweenAlpha>();
	}

	void Update () {
		if(_gameOver) {
			foreach (InputDevice input in InputManager.Devices) {
				if (input.MenuWasPressed)
					Application.LoadLevel (1);
			}
		}
	}

	public void ActivateGameOver() {
		StartCoroutine(GameOverInt());
	}

	IEnumerator GameOverInt() {
		Explosions.SetActive(true);

		yield return new WaitForSeconds(1.5f);

		_alpha.PlayForward();

		yield return new WaitForSeconds(0.5f);

		_gameOver = true;


		Label.text = "";
		string showingText = "";
		char[] chars = "GROOVE IS DEAD".ToCharArray();
		foreach (char c in chars) {
			showingText += c;
			Label.text = showingText;
			audio.Play();
			yield return new WaitForSeconds (0.25f);
		}

		yield return new WaitForSeconds(0.5f);

		TryAgainAlpha.SetActive(true);
	}
}
