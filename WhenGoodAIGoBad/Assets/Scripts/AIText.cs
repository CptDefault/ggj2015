using UnityEngine;
using System.Collections;

public class AIText : MonoBehaviour {
	
	private static AIText _instance;
	public static AIText Instance { get {return _instance;} }
	public UILabel label;
	public TweenScale scale;

	private string _myText;
	private string _showingText; 

	private void Awake () {
		_instance = this;
	}

	public static void ShowText(string text) {
		_instance.StopCoroutine("ShowText");
		_instance._myText = text.ToUpper();
		_instance.StartCoroutine("ShowText");
	}

	IEnumerator ShowText() {
		_showingText = "";
		label.text = "";
		//scale.ResetToBeginning();
		//label.transform.localScale = new Vector3(0, 0.25f, 1);
		AudioManager.PlayAnnouncement();

		yield return new WaitForSeconds(2f);
		
		scale.PlayForward();
		yield return new WaitForSeconds(0.5f);

		char[] chars = _myText.ToCharArray();
		foreach (char c in chars) {
			_showingText += c;
			label.text = _showingText;
			audio.Play();
			yield return new WaitForSeconds (0.02f);
		}

		yield return new WaitForSeconds (3f);

		scale.PlayReverse();
	}
}
