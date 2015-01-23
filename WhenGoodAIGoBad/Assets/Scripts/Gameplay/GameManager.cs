using InControl;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

	protected void Start()
	{
	    for (int i = 0; i < InputManager.Devices.Count; i++)
	    {
	        var inputDevice = InputManager.Devices[i];
	        var playerGO = (GameObject)Instantiate(PlayerPrefab);

	        playerGO.GetComponent<PlayerInput>().SetController(inputDevice);
	    }
	}
}
