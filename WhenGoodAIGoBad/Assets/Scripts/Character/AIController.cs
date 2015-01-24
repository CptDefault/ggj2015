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

    public Transform TestThingy;
    private RepairTrigger[] _repairTriggers;
    private RepairTrigger _target;
    private Goal _goal;
    private PlayerManager _targetPlayer;
    private double _waitUntilTime;

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
        TestThingy = ((MonoBehaviour)_path[_pathInd]).transform;

        var target = ((MonoBehaviour) _path[_pathInd]).transform.position;
        Vector2 toTarget = target - transform.position;

        _characterController.SetDesiredSpeed(toTarget.normalized);

        if (Vector3.SqrMagnitude(transform.position - target) < 0.2f)
            _pathInd++;

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
