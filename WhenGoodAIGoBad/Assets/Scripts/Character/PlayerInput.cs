using InControl;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController _characterController;
    private InputDevice _inputDevice;
    private PlayerManager _playerManager;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
    }

    protected void Start()
    {
        if(_inputDevice == null)
            _inputDevice = InputManager.Devices[0];        
    }

    protected void Update()
    {
        var vector2 = _inputDevice.LeftStick.Vector + _inputDevice.DPad.Vector;


        _characterController.SetDesiredSpeed(Vector2.ClampMagnitude(vector2, 1));

        if (_inputDevice.Action2.WasPressed)
            _playerManager.PickupDropItem();
    }

    public void SetController(InputDevice inputDevice)
    {
        _inputDevice = inputDevice;
    }
}
