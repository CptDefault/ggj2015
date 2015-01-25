using UnityEngine;
using System.Collections;

public class IntroDirector : MonoBehaviour {
	public static bool ChipPlaced;

	private static IntroDirector _instance;
	public static IntroDirector Instance {get {return _instance;}}

	public GameObject CockpitExplosion;

	private bool _waitingForDining;
	private PlayerManager[] players;

	void Awake () {
		_instance = this;
	}
	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(5f);

		CockpitExplosion.SetActive(true);

		yield return new WaitForSeconds(2.5f);

		AIText.ShowText("Attention Dancers of De Galaxy, a fault has been detetected in the ship's navigation systems.");

		yield return new WaitForSeconds(10f);

		AIText.ShowText("Please find the AI chip and place it in the cockpit to initiate system recovery.");
	}

	public static void PlaceChip() {
		ChipPlaced = true;
		AIText.ShowText("Thank you for your cooperation. Dinner is now ready in the central dining room.");
		_instance._waitingForDining = true;
		_instance.players = FindObjectsOfType<PlayerManager>();
		FindObjectOfType<FoodDispenserVisual>().Dispense();
	}

	IEnumerator StartGameInt() {
		// turn on the AI, lock the doors

		yield return new WaitForSeconds(3f);

		AIText.ShowText("I'm gonna fuck you up");

	}

	void Update() {
		if(_waitingForDining) {
			var count = 0;
			foreach(PlayerManager p in players) {
				if(p.InDiningRoom)
					count++;
			}

			if(count >= players.Length) {
				_waitingForDining = false;
				StartCoroutine(StartGameInt());
			}
		}
	}
}
