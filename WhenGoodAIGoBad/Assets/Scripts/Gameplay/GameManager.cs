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
    private float _startTime;
    private int _roundDuration = 45;
    private AIController _aiController;
    private Alien _alien;

    public bool NonAgressive { get; private set; }


    public static void RegisterRoom(Room room)
    {
        if (Instance == null)
            Instance = FindObjectOfType<GameManager>();

        Instance.Rooms.Add(room);
    }

    protected void Awake()
    {
        Instance = this;
        NonAgressive = true;
        _aiController = FindObjectOfType<AIController>();
        _alien = FindObjectOfType<Alien>();
        _alien.gameObject.SetActive(false);
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
	}

    public float GetProgress()
    {
        return (Time.time - _startTime)/(_roundDuration*4);
    }

    public void StartMainGame()
    {
        StartCoroutine(MainGameCoroutine());
    }

    private IEnumerator MainGameCoroutine()
    {
        foreach (var door in AIRoomDoors)
        {
            door.Locked = true;
        }
        NonAgressive = false;

        _startTime = Time.time;

        for (int index = 0; index < AIRoomDoors.Length; index++)
        {
            yield return new WaitForSeconds(_roundDuration);
            var door = AIRoomDoors[index];

            _aiController.LevelUp();

            if(index == 3)
                _alien.gameObject.SetActive(true);

            door.Locked = false;
        }
    }
}
