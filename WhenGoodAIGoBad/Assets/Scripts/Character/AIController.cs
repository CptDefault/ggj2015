using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class AIController : MonoBehaviour
{
    public enum Goal
    {
        BombTarget,
        LightFire,
        HuntPlayer,
        LockDoors,
    }

    private List<ITraversable> _path;
    private int _pathInd;
    private CharacterController _characterController;

    private RepairTrigger[] _repairTriggers;
    private RepairTrigger _target;
    private Goal _goal;
    private PlayerManager _targetPlayer;
    private double _waitUntilTime;
    private bool _passingDoor;
    private Vector3 _debug;
    private Vector2 _doorVelocity;
    private bool _approachingDoor;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _repairTriggers = FindObjectsOfType<RepairTrigger>();
    }

    public void PlayerDanced(PlayerManager player)
    {
        _targetPlayer = player;
        _goal = Goal.HuntPlayer;
        _path = AIPathfinding.PathToPoint(transform.position, _targetPlayer.transform.position);
        _pathInd = 0;
    }

    protected void OnDrawGizmos()
    {
        if(Application.isPlaying)
            Gizmos.DrawLine(transform.position, _debug);
    }


    protected void Update()
    {
        if(Time.time < _waitUntilTime)
            return;

        if (_path == null || _pathInd >= _path.Count)
        {
            int attempts = 10;
            while((_target == null || _target.Health < .9f) && attempts-- > 0)
                _target = _repairTriggers[Random.Range(0, _repairTriggers.Length)];
            _path = AIPathfinding.PathToPoint(transform.position, _target.transform.position);
            _pathInd = 0;
            _goal = Goal.BombTarget;
        }

        var target = ((MonoBehaviour) _path[_pathInd]).transform.position;
        var d = _path[_pathInd] as Door;
        if (d != null)
        {
            target = d.DoorCollider.bounds.center;

            if (!_approachingDoor)
            {
                _doorVelocity = target - transform.position;
                _approachingDoor = true;
            }
            if (_approachingDoor && ! _passingDoor)
            {
                target += (Vector3)d.Facing.normalized * 1 * -Mathf.Sign(Vector2.Dot(d.Facing, _doorVelocity));
                print("Approaching");
            }
            if (_passingDoor || Vector3.SqrMagnitude(transform.position - target) < 0.4f)
            {
                print("Passing" + (Vector3)d.Facing.normalized + " : " + Mathf.Sign(Vector2.Dot(d.Facing, _doorVelocity)));
                _passingDoor = true;
                target = d.DoorCollider.bounds.center;
                target += (Vector3)d.Facing.normalized * 1 * Mathf.Sign(Vector2.Dot(d.Facing, _doorVelocity));
            }
            else if (!_approachingDoor)
            {
                print("Nothing special");                
            }
        }
        Vector2 toTarget = target - transform.position;

        _debug = target;

        _characterController.SetDesiredSpeed(d == null ? toTarget.normalized : Vector2.ClampMagnitude(toTarget,1));

        if (Vector3.SqrMagnitude(transform.position - target) < (d == null ? 0.4f : 0.1f))
        {
            _pathInd++;
            _passingDoor = false;
            _approachingDoor = false;
        }

        if (_pathInd >= _path.Count)
        {
            switch (_goal)
            {
                case Goal.BombTarget:
                    _target.Damage(0.3f);
                    break;
                case Goal.LightFire:
                    break;
                case Goal.HuntPlayer:
                    if (_targetPlayer.GetComponent<CharacterController>().Room == _characterController.Room)
                    {
                        foreach (var door in _characterController.Room.Doors)
                        {
                            door.LockFor(6);
                        }

                    }
                    break;
                case Goal.LockDoors:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
