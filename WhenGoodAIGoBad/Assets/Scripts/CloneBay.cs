using UnityEngine;
using System.Collections;

public class CloneBay : MonoBehaviour {

	public float RespawnTime=2f;

	private static CloneBay _instance;
	public static CloneBay Instance { get { return _instance;} }

	void Awake () {
		_instance = this;
	}


	public void RespawnPlayer(PlayerInput player) {
		StartCoroutine(RespawnPlayerInt(player));
	}

	IEnumerator RespawnPlayerInt(PlayerInput player) {
		yield return new WaitForSeconds(RespawnTime);

		player.Respawn();
		player.transform.position = transform.position;

		// play respawn sound
	}
}
