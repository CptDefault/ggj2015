using UnityEngine;
using System.Collections;
using System;

public class RepairTrigger : MonoBehaviour {
	public Tool.ToolType ToolRequired;

	public GameObject HealthBarPrefab;
	public float Health = 1;
	public HealthBar myBar;

    public float BurnTime = 10;
    public float RepairTime = 8;

    public AudioClipContainer RepairSound;
    public AudioClipContainer RepairEndSound;
    public AudioClipContainer ExplodeSound;

	public Action<float> OnIncrementHealth;
    private bool _repairing;
    private float _lastRepairTime;
    private AudioSource _audio;


    void Start () {
		if(myBar == null)
			myBar = (GameObject.Instantiate(HealthBarPrefab) as GameObject).GetComponent<HealthBar>();

		myBar.transform.parent = UICamera.mainCamera.transform.parent;
		myBar.transform.localScale = HealthBarPrefab.transform.localScale;

		OnIncrementHealth += myBar.SetHealth;
		myBar.SetHealth(Health);

		Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		screenPos.x -= (Screen.width/ 2.0f);
		screenPos.y -= (Screen.height / 2.0f);
		screenPos.y += Screen.height/15f;
		myBar.transform.localPosition = screenPos;
	}

	protected void OnTriggerEnter2D(Collider2D other) {
	    if(Health < 1f)
	    {
	        var playerInput = other.gameObject.GetComponent<PlayerInput>();
	        if(playerInput != null)
                playerInput.InitRepairs(this, true);
	    }
	}

	protected void OnTriggerExit2D(Collider2D other) {
		var playerInput = other.gameObject.GetComponent<PlayerInput>();
	    if(playerInput != null)
            other.gameObject.GetComponent<PlayerInput>().InitRepairs(null, false);
		
	}

    protected void Update()
    {
        if (Health < 0.95f && Health > 0)
        {
            Health -= 1/BurnTime/2 * Time.deltaTime;
            OnIncrementHealth(Health);

            if (Health < 0.2f)
            {
                AudioManager.PlayAlarm();
            }

            if (Health < 0)
                ExplodeSound.Play();
        }
    }

	public void Repair() {
        Health += 1 / (RepairTime - BurnTime / 2) / 4;
		OnIncrementHealth(Health);

	    _lastRepairTime = Time.time;

        if (!_repairing)
            StartCoroutine(RepairSoundRoutine());
	}

    private IEnumerator RepairSoundRoutine()
    {
        _repairing = true;

        _audio = RepairSound.Play();

        float vol = _audio.volume;

        while (Time.time - _lastRepairTime < 1f)
        {
            _audio.volume = vol*Mathf.Clamp01(2 - (Time.time - _lastRepairTime) * 4);
            yield return null;
        }
        _repairing = false;
        _audio.Stop();
    }

    public void CompleteRepair() {
		Health = 1f;
        RepairEndSound.Play();
        // play some kind of sound
    }

    public void Damage(float damageAmount)
    {
        Health *= 1 - damageAmount;
    }
}
