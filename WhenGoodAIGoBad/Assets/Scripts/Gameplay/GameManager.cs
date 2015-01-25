using System.Collections.Generic;
using InControl;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public readonly List<Room> Rooms = new List<Room>();

    public GameObject PlayerPrefab;

    public Transform[] SpawnPoints;

    public Door[] InterestingDoors;

    public Door[] AIRoomDoors;

    public float RandomDamageTimer = 10;
    public float RandomDamageAmount = 0.2f;



    public static void RegisterRoom(Room room)
    {
        if (Instance == null)
            Instance = FindObjectOfType<GameManager>();

        Instance.Rooms.Add(room);
    }

    protected void Awake()
    {
        Instance = this;
    }

    protected void Start()
	{
	    for (int i = 0; i < InputManager.Devices.Count; i++)
	    {
	        var inputDevice = InputManager.Devices[i];
	        var playerGO = (GameObject)Instantiate(PlayerPrefab);

	        playerGO.transform.position = SpawnPoints[i % SpawnPoints.Length].position;

	        playerGO.GetComponent<PlayerInput>().SetController(inputDevice);
	    }

        StartCoroutine(MainGameCoroutine());
	}

    private IEnumerator MainGameCoroutine()
    {
        foreach (var door in AIRoomDoors)
        {
            door.Locked = true;
        }

        for (int index = 0; index < AIRoomDoors.Length; index++)
        {
            yield return new WaitForSeconds(5);
            var door = AIRoomDoors[index];

            door.Locked = false;
        }
    }
}
