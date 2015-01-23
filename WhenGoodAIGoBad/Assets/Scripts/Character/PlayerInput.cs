using InControl;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInput : MonoBehaviour
{
    private CharacterController _characterController;
    private InputDevice _inputDevice;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected void Start()
    {
        _inputDevice = InputManager.Devices[0];        
    }

    protected void Update()
    {
        _characterController.SetDesiredSpeed(_inputDevice.LeftStick.Vector);
    }
}
