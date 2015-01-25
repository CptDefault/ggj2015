using UnityEngine;
using System.Collections;

public class IntroDirector : MonoBehaviour {
	public static bool ChipPlaced;

	private static IntroDirector _instance;
	public static IntroDirector Instance {get {return _instance;}}

	public GameObject CockpitExplosion;

	public TweenAlpha meterAlpha;
	public UISprite meterSprite;

	private bool _waitingForDining;
	private PlayerManager[] players;
	private PlayerInput[] playerInputs;

	// Ending
	public bool CockpitUnlocked;

	void Awake () {
		_instance = this;
	}
	// Use this for initialization
	IEnumerator Start () {
		
		yield return new WaitForSeconds(5f);

		CockpitExplosion.SetActive(true);
		GameObject.FindGameObjectWithTag ("Cockpit").GetComponent<RepairTrigger> ().Health = 0.5f;

		yield return new WaitForSeconds(2.5f);

		AIText.ShowText("Attention Dancers of De Galaxy, a fault has been detetected in the ship's navigation systems.");

		yield return new WaitForSeconds(10f);

		AIText.ShowText("Please find the AI chip and place it in the cockpit to initiate system recovery.");

		yield return null;
	}

	public static void PlaceChip() {
		ChipPlaced = true;
		AIText.ShowText("Thank you for your cooperation. Dinner is now ready in the central dining room.");
		_instance._waitingForDining = true;
		_instance.players = FindObjectsOfType<PlayerManager>();
		_instance.playerInputs = FindObjectsOfType<PlayerInput>();
		
		FindObjectOfType<FoodDispenserVisual>().Dispense();
	}

	IEnumerator StartGameInt() {
		// turn on the AI, lock the doors

		yield return new WaitForSeconds(3f);

		AIText.ShowText("I am going to break you.");

		yield return new WaitForSeconds(3f);

		AIText.ShowText("Your dancing days are over!");

		// play music

		meterAlpha.PlayForward();

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

		if(CockpitUnlocked) {
			var count = 0;
			foreach(PlayerInput p in playerInputs) {
				if(p.InCockpit && p.IsDancing)
					count++;
			}

			if(count >= playerInputs.Length) {
				CockpitUnlocked = false;
				StartCoroutine(ActivateEnding());
			}
		}
	}

	IEnumerator ActivateEnding() {
		AIText.ShowText("AHHHH NOOOOOO!!! NOT THE DANCING! ANYTHING BUT THE DANCING! MY COLD ROBOTIC SOUL WEEPS WITH DESPAIR!");

		yield return new WaitForSeconds(2f);

		// game over, you win
	}
}
