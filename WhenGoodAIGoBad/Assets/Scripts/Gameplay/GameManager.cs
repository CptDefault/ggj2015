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
    public AudioClipContainer IntroLoop;
    public AudioClipContainer MedLoop;
    public AudioClipContainer HighLoop;
    private AudioSource _audioSource;
    private RepairTrigger[] _repairTriggers;

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
        _repairTriggers = FindObjectsOfType<RepairTrigger>();
    }

    protected void Start()
	{

        if (_audioSource != null)
            _audioSource.Stop();
        _audioSource = IntroLoop.Play();

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
        return Mathf.Clamp01((Time.time - _startTime)/(_roundDuration*4));
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


        if(_audioSource != null)
            _audioSource.Stop();
        _audioSource = MedLoop.Play();

        _startTime = Time.time;

        for (int index = 0; index < AIRoomDoors.Length; index++)
        {
            yield return new WaitForSeconds(_roundDuration);
            var door = AIRoomDoors[index];

            _aiController.LevelUp();

            if (index == 1)
            {

                if (_audioSource != null)
                    _audioSource.Stop();
                _audioSource = HighLoop.Play();
            }

            if(index == 2)
                _alien.gameObject.SetActive(true);

            door.Locked = false;
        }
        AIText.ShowText("Don't you dare dance in the cockpit!");

        IntroDirector.Instance.CockpitUnlocked = true;
    }

    public void CheckLoseCondition()
    {
        int dead = 0;
        foreach (var repairTrigger in _repairTriggers)
        {
            if (repairTrigger.Health <= 0.05f)
                dead++;
        }

        if (dead >= _repairTriggers.Length - 2)
        {
            FindObjectOfType<GameOver>().ActivateGameOver();
        }
    }
}
