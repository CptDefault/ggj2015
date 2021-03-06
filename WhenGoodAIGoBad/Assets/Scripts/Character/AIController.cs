﻿using System;
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

    public enum DoorStage
    {
        Start,
        Door,
        Pass,
        Done
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
    private Vector3 _doorVelocity;
    private bool _approachingDoor;
    private DoorStage _doorStage;
    private Color _col;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _repairTriggers = FindObjectsOfType<RepairTrigger>();
    }

    public void PlayerDanced(PlayerManager player)
    {
        _targetPlayer = player;
        _goal = Goal.HuntPlayer;
        _path = AIPathfinding.PathToPoint(transform.position, _targetPlayer.transform.position, true);
        _pathInd = 0;

        //_characterController.MoveSpeed += 0.2f;
        //_characterController.Acceleration *= 0.4f;
    }

    protected void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, _debug);
            Gizmos.color = _col;
            Gizmos.DrawLine(_debug, _debug + _doorVelocity);
        }

    }


    protected void Update()
    {
        if(Time.time < _waitUntilTime)
            return;

        if (_path == null || _pathInd >= _path.Count)
        {
            int attempts = 10;

            if (Random.value > 0.5f)
            {
                while ((_target == null || _target.Health < .9f || _target.ToolRequired == Tool.ToolType.AIChip) &&
                       attempts-- > 0)
                    _target = _repairTriggers[Random.Range(0, _repairTriggers.Length)];
                _path = AIPathfinding.PathToPoint(transform.position, _target.transform.position, true);
                _pathInd = 0;
                _goal = Goal.BombTarget;
            }
            else
            {
                Room room = null;
                while ((room == null || room.Fire.Count == 0) && attempts-- > 0)
                    room = GameManager.Instance.Rooms[Random.Range(0, GameManager.Instance.Rooms.Count)];
                _path = AIPathfinding.PathToPoint(transform.position, room.transform.position, true);
                _pathInd = 0;
                _goal = Goal.LightFire;
            }
            _doorStage = DoorStage.Start;
            var dr = _path[_pathInd] as Door;
            if (dr != null)
                _doorVelocity = (Vector3.Dot(dr.DoorCollider.bounds.center - transform.position, dr.Facing) * dr.Facing).normalized;
        }

        var target = ((MonoBehaviour) _path[_pathInd]).transform.position;
        var d = _path[_pathInd] as Door;
        _col = Color.red;
        if (d != null)
        {
            target = d.Center;
            _col = Color.green;

            switch (_doorStage)
            {
                case DoorStage.Start:
                    target -= _doorVelocity;
                    if(Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = DoorStage.Door;
                    break;
                case DoorStage.Door:
                    if (Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = DoorStage.Pass;
                    break;
                case DoorStage.Pass:
                    target += _doorVelocity;
                    if (Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = DoorStage.Done;
                    break;
                case DoorStage.Done:
                    break;
            }
        }
        Vector2 toTarget = target - transform.position;

        _debug = target;

        _characterController.SetDesiredSpeed(d == null ? toTarget.normalized : Vector2.ClampMagnitude(toTarget,1));

        if (Vector3.SqrMagnitude(transform.position - target) < 0.4f && (d == null || _doorStage == DoorStage.Done))
        {
            _pathInd++;
            _passingDoor = false;
            _approachingDoor = false;

            _doorStage = DoorStage.Start;
            if (_pathInd < _path.Count)
            {
                var dr = _path[_pathInd] as Door;
                if (dr != null)
                    _doorVelocity =
                        (Vector3.Dot(dr.DoorCollider.bounds.center - transform.position, dr.Facing)*dr.Facing)
                            .normalized;
            }
        }

        if (_pathInd >= _path.Count)
        {
            if(GameManager.Instance.NonAgressive)
                return;

            switch (_goal)
            {
                case Goal.BombTarget:
                    _target.Damage(0.3f);
                    break;
                case Goal.LightFire:
                    var fires2 = _characterController.Room.Fire;
                    if(fires2.Count > 0)
                        fires2[Random.Range(0, fires2.Count)].gameObject.SetActive(true);
                    break;
                case Goal.HuntPlayer:
                    if (_targetPlayer.Character.Room == _characterController.Room)
                    {
                        foreach (var door in _characterController.Room.Doors)
                        {
                            door.LockFor(6);
                        }
                        var fires = _characterController.Room.Fire;
                        if(fires.Count > 0)
                            fires[Random.Range(0, fires.Count)].gameObject.SetActive(true);
                    }
                    break;
                case Goal.LockDoors:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void LevelUp()
    {
        _characterController.MoveSpeed *= 1.2f;
        _characterController.Acceleration *= 1.4f;
    }
}
