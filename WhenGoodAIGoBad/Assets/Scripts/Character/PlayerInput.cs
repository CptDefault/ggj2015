﻿using InControl;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController _characterController;
    private InputDevice _inputDevice;
    private PlayerManager _playerManager;
	public SpriteRenderer DamageSprite;
    // Repairing functions
    private bool _canRepair = false;
    private RepairTrigger _machine;
    private float _repairTimer; 

    // Fire extinguisher
    public ParticleSystem ExtinguisherParticle;
    public Transform ExtinguisherRayCast;

    // Heading
    private Quaternion _heading;

    // Dancing
    public GameObject BoomBox;

    // Health + Damage
    public bool Alive { get; private set; }
    private bool _stunned;
    private float _stunTimer;
    private float _flashTimer;
    public AudioClipContainer HurtSound;
    public AudioClipContainer DieSound;
	public AudioClip pickupSound;
	public AudioClip dropSound;

    // Tutorial
    private bool _hasDanced; 
    private bool _hasUsedExtinguisher;

    // Ending 
    public bool IsDancing;
    public bool InCockpit;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
		

        Alive = true;
    }

    protected void Start()
    {
        if(_inputDevice == null)
            _inputDevice = InputManager.Devices[0]; 

        ExtinguisherParticle.Stop();       
        BoomBox.SetActive(false);
    }

    protected void Update()
    {
        if(_inputDevice.MenuWasPressed)
            PauseScreen.TogglePause();

        if(!Alive) {
            _characterController.SetDesiredSpeed(Vector2.zero);
            return;
        }

        if(_stunned) {
            _stunTimer += Time.deltaTime;
            _flashTimer += Time.deltaTime;

            if(_flashTimer >= 0.1f) {
				DamageSprite.enabled = !DamageSprite.enabled;
                _flashTimer = 0;
            }

            if(_stunTimer >= 3f) {
				DamageSprite.enabled = true;
                _stunned = false;
                _stunTimer = 0;
                _flashTimer = 0;
            } 
        }

        // start dancing
        if(_inputDevice.Action4.WasPressed) {
            IsDancing = true;
            //summon boombox
            BoomBox.SetActive(true);
            BoomBox.transform.localScale = Vector3.zero;
            LeanTween.scale(BoomBox, new Vector3(2.5f, 1.25f, 10) , 0.25f).setEase(LeanTweenType.easeOutElastic);
			//LeanTween.scale(BoomBox, new Vector3(0.23f, 0.065f, 1) , 0.25f).setLoopPingPong().setEase(LeanTweenType.easeOutCubic);
            BoomBox.audio.Play();

            //FindObjectOfType<GameOver>().ActivateGameOver();
            //AIText.ShowText("Stop dancing!");

            if(!_hasDanced) {
                _hasDanced = true;
                _playerManager.TipLabel.text = "";
            }
        } else if(_inputDevice.Action4.IsPressed) {
            _characterController.SetDesiredSpeed(Vector2.zero);
            return;
        }
        else if (_inputDevice.Action4.WasReleased) {
            IsDancing = false;
            BoomBox.SetActive(false);
            BoomBox.audio.Stop();
        }



        if(_inputDevice.LeftStick.Vector.magnitude > 0.3f) {
            float heading = Mathf.Atan2(_inputDevice.LeftStick.Vector.x,_inputDevice.LeftStick.Vector.y);
            _heading = Quaternion.Euler(-90f+heading*Mathf.Rad2Deg,90f,0f); 
        }
        ExtinguisherParticle.transform.rotation=_heading;


        var vector2 = _inputDevice.LeftStick.Vector + _inputDevice.DPad.Vector;

        _characterController.SetDesiredSpeed(Vector2.ClampMagnitude(vector2, 1));

        if (_inputDevice.Action2.WasPressed) {
		    _playerManager.PickupDropItem();
            ExtinguisherParticle.Stop ();

            if(!_hasUsedExtinguisher && _playerManager.CarriedTool != null && _playerManager.CarriedTool.Type == Tool.ToolType.Extinguisher) {
                _playerManager.TipLabel.text = "(A) Shoot fire extinguisher";
            }

        }
            

        if (_inputDevice.Action4.WasPressed)
        {
            var aiController = FindObjectOfType<AIController>();
            if (aiController != null) aiController.PlayerDanced(_playerManager);
        }

        if(_canRepair && _playerManager.CarriedTool != null && _machine != null) {
            
            if(_inputDevice.Action1.IsPressed && _playerManager.CarriedTool.Type == _machine.ToolRequired) {
                _repairTimer += Time.deltaTime;
                if(_repairTimer >= 0.25f) {
				    _machine.Repair();
                    _repairTimer = 0;
                }

                if(_machine.Health >= 0.99f) {
                    _machine.CompleteRepair();
                    _playerManager.ConsumeTool();
                    _playerManager.TipLabel.text = "";
                }
            }
        }

        // EXTINGUISHER
		if(_playerManager.CarriedTool != null && _playerManager.CarriedTool.Type == Tool.ToolType.Extinguisher) {
            if(_inputDevice.Action1.WasPressed) {
                ExtinguisherParticle.Play();
                ExtinguisherParticle.audio.Play();
                //play a sound

                if(!_hasUsedExtinguisher) {
                    _playerManager.TipLabel.text = "";
                    _hasUsedExtinguisher = true;
                }
                
            } else if (_inputDevice.Action1.IsPressed) {
                _characterController.SetDesiredSpeed(Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, ExtinguisherRayCast.position-transform.position, 4f, 1 << 8);
                if (hit.collider != null) {
                    DamageSprite.enabled = true;
					//hit.collider.gameObject.collider2D.enabled = false;
                    hit.collider.gameObject.SetActive(false);
					//LeanTween.alpha(hit.collider.gameObject, 0, 0.5f);
                    //Destroy(hit.collider.gameObject, 0.6f);
                }
            }
            else if (_inputDevice.Action1.WasReleased) {
				ExtinguisherParticle.Stop ();
				ExtinguisherParticle.Clear ();
                ExtinguisherParticle.audio.Stop ();
			}
        }
        
    }

    public void SetController(InputDevice inputDevice)
    {
        _inputDevice = inputDevice;
    }

    public void InitRepairs(RepairTrigger machine, bool canRepair, Tool.ToolType toolRequired) {
        _machine = machine;
        _canRepair = canRepair;

        if(_canRepair && _playerManager.CarriedTool != null) {
            if(_playerManager.CarriedTool.Type == toolRequired)
                _playerManager.TipLabel.text = "(A) Repair";
            else 
                _playerManager.TipLabel.text = "Wrong tool!";
        }
        else
            _playerManager.TipLabel.text = "";
    }

    protected void OnTriggerEnter2D(Collider2D other) {

        if(other.GetComponent<Fire>() != null) {
            GetStunned(other.gameObject.transform.position);
        } 

        if (other.gameObject.tag == "Cockpit") {
               InCockpit = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Cockpit") {
               InCockpit = false;
        }
    }

    private void GetStunned(Vector3 pos) {
		rigidbody2D.AddForce (400 * (transform.position - pos));
        DamageSprite.enabled = true;

        if(_stunned && _stunTimer >= 1f) {
            // die
            Die();
        } else {
            _stunned = true;
            HurtSound.Play();
        }
    }

    private void Die() {
        if(Alive)
        {
            DieSound.Play();
            Alive = false;
            CloneBay.Instance.RespawnPlayer(this);

            // DROP IT LIKE ITS HOT
            if(_playerManager.CarriedTool != null) 
                _playerManager.PickupDropItem();

            ExtinguisherParticle.Stop();
            ExtinguisherParticle.Clear();
            ExtinguisherParticle.audio.Stop();
        }
    }

    public void Respawn() {
        Alive = true;
        DamageSprite.enabled = true;
        _stunTimer = 0;
        _flashTimer = 0;
    }

    /*private void DetermineHeading() {
        print(_inputDevice.LeftStick.Vector.x + " " + _inputDevice.LeftStick.Vector.y);
        if(_inputDevice.LeftStick.Vector.x > 0.3f && _inputDevice.LeftStick.Vector.y > 0.3f) {
            _heading = Heading.NorthEast;
        }
        else if(_inputDevice.LeftStick.Vector.x > 0.3f && _inputDevice.LeftStick.Vector.y < -0.3f) {
            _heading = Heading.SouthEast;
            
        }
        else if(_inputDevice.LeftStick.Vector.x < -0.3f && _inputDevice.LeftStick.Vector.y < -0.3f) {
            _heading = Heading.SouthWest;
            
        }
        else if(_inputDevice.LeftStick.Vector.x < -0.3f && _inputDevice.LeftStick.Vector.y > 0.3f) {
            _heading = Heading.NorthWest;
            
        }
        else if(_inputDevice.LeftStick.Vector.x > 0) {
            _heading = Heading.East;
            
        }
        else if(_inputDevice.LeftStick.Vector.x < 0) {
            _heading = Heading.West;
            
        }
        else if(_inputDevice.LeftStick.Vector.y > 0) {
            _heading = Heading.North;
            
        }
        else if(_inputDevice.LeftStick.Vector.y < 0) {
            _heading = Heading.South;
            
        }
    }*/
}
