using InControl;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController _characterController;
    private InputDevice _inputDevice;
    private PlayerManager _playerManager;

    // Repairing functions
    private bool _canRepair = false;
    private RepairTrigger _machine;
    private float _repairTimer; 

    // Fire extinguisher
    public ParticleSystem ExtinguisherParticle;
    public Transform ExtinguisherRayCast;

    // Heading
    private Quaternion _heading;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
    }

    protected void Start()
    {
        if(_inputDevice == null)
            _inputDevice = InputManager.Devices[0]; 

        ExtinguisherParticle.Stop();       
    }

    protected void Update()
    {
        // determine heading
        //if(_inputDevice.LeftStick.Vector != Vector2.zero)
        //    DetermineHeading();

        if(_inputDevice.LeftStick.Vector.magnitude > 0.3f) {
            float heading = Mathf.Atan2(_inputDevice.LeftStick.Vector.x,_inputDevice.LeftStick.Vector.y);
            _heading = Quaternion.Euler(-90f+heading*Mathf.Rad2Deg,90f,0f); 
        }
        ExtinguisherParticle.transform.rotation=_heading;


        var vector2 = _inputDevice.LeftStick.Vector + _inputDevice.DPad.Vector;

        //print(_inputDevice.LeftStick.Vector);
        _characterController.SetDesiredSpeed(Vector2.ClampMagnitude(vector2, 1));

        if (_inputDevice.Action2.WasPressed)
            _playerManager.PickupDropItem();

        if(_canRepair && _playerManager.CarriedTool != null && _machine != null) {
            
            if(_inputDevice.Action1.IsPressed && _playerManager.CarriedTool.Type == _machine.ToolRequired) {
                _repairTimer += Time.deltaTime;
                if(_repairTimer >= 0.25f) {
				    _machine.Repair();
                    _repairTimer = 0;
                }
            }

            if(_machine.Health >= 0.99f) {
                _machine.CompleteRepair();
                _playerManager.ConsumeTool();
            }
        }

		if(_playerManager.CarriedTool != null && _playerManager.CarriedTool.Type == Tool.ToolType.Extinguisher) {
            if(_inputDevice.Action3.WasPressed) {
                ExtinguisherParticle.Play();
                //play a sound
                
            } else if (_inputDevice.Action3.IsPressed) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, ExtinguisherRayCast.position, 4f, 1 << 8);
                if (hit.collider != null) {
					hit.collider.gameObject.collider2D.enabled = false;
					LeanTween.alpha(hit.collider.gameObject, 0, 0.5f);
                    Destroy(hit.collider.gameObject, 0.6f);
                }
            }
            else if (_inputDevice.Action3.WasReleased) {
				ExtinguisherParticle.Stop ();
			}
        }
        
    }

    public void SetController(InputDevice inputDevice)
    {
        _inputDevice = inputDevice;
    }

    public void InitRepairs(RepairTrigger machine, bool canRepair) {
        _machine = machine;
        _canRepair = canRepair;
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
