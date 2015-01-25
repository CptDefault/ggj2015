using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Alien : MonoBehaviour
{
    private List<ITraversable> _path;
    private int _pathInd;
    private CharacterController _characterController;

    private PlayerManager _targetPlayer;
    private double _waitUntilTime;
    private bool _passingDoor;
    private Vector3 _doorVelocity;
    private bool _approachingDoor;
    private AIController.DoorStage _doorStage;
    private PlayerManager[] _playerManagers;
    private Vector3 _debug;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();

    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + _debug);
    }

    protected void Update()
    {
        if (Time.time < _waitUntilTime)
            return;

        if(_playerManagers == null || _playerManagers.Length == 0)
            _playerManagers = FindObjectsOfType<PlayerManager>();

        if (_targetPlayer == null)
        {
            foreach (var player in _playerManagers)
            {
                if (player.Character.Room == _characterController.Room && player.Input.Alive)
                {
                    _targetPlayer = player;
                    _path = null;
                    break;
                }
            }
        }

        if (_path == null || _pathInd >= _path.Count)
        {
            var randRoom = GameManager.Instance.Rooms[Random.Range(0, GameManager.Instance.Rooms.Count)];

            _path = AIPathfinding.PathToPoint(transform.position, _targetPlayer == null ? randRoom.transform.position : _targetPlayer.transform.position);
            _pathInd = 0;

            _doorStage = AIController.DoorStage.Start;
            var dr = _path[_pathInd] as Door;
            if (dr != null)
                _doorVelocity = (Vector3.Dot(dr.DoorCollider.bounds.center - transform.position, dr.Facing) * dr.Facing).normalized;
        }
        var d = _path[_pathInd] as Door;
        var target = ((MonoBehaviour)_path[_pathInd]).transform.position;

        /*if (d == null && _pathInd + 1 < _path.Count)
        {
            var goal = ((MonoBehaviour) _path[_pathInd + 1]).transform.position;
            var door = _path[_pathInd + 1] as Door;
            if (door != null)
                goal = door.Center;
            var direction = goal - transform.position;
            _debug = direction;
            var raycastHit2D = Physics2D.CircleCast(transform.position + direction.normalized * 0.4f, 0.02f, direction, direction.magnitude - 1, 1);
            if (raycastHit2D.collider == null)
            {
                _pathInd++;

                if (_pathInd >= _path.Count)
                    return;

                d = _path[_pathInd] as Door;
                target = ((MonoBehaviour)_path[_pathInd]).transform.position;
            }
            else
            {
                print("Advance interrupted by " + raycastHit2D.collider.gameObject.name);
            }
        }
*/
        if (d != null)
        {
            target = d.Center;

            switch (_doorStage)
            {
                case AIController.DoorStage.Start:
                    target -= _doorVelocity * 2;
                    if (Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = AIController.DoorStage.Door;
                    break;
                case AIController.DoorStage.Door:
                    if (Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = AIController.DoorStage.Pass;
                    break;
                case AIController.DoorStage.Pass:
                    target += _doorVelocity * 2;
                    if (Vector3.SqrMagnitude(transform.position - target) < 0.4f)
                        _doorStage = AIController.DoorStage.Done;
                    break;
                case AIController.DoorStage.Done:
                    break;
            }
        }
        if (_targetPlayer != null && _targetPlayer.Character.Room == _characterController.Room)
        {
            target = _targetPlayer.transform.position;
            if ((target - transform.position).sqrMagnitude < 0.5)
            {
                _targetPlayer.SendMessage("GetStunned", transform.position);
                _targetPlayer = null;
            }
        }

        Vector2 toTarget = target - transform.position;
        
        _characterController.SetDesiredSpeed(d == null ? toTarget.normalized : Vector2.ClampMagnitude(toTarget, 1));

        if (Vector3.SqrMagnitude(transform.position - target) < 0.4f && (d == null || _doorStage == AIController.DoorStage.Done))
        {
            _pathInd++;
            _passingDoor = false;
            _approachingDoor = false;

            _doorStage = AIController.DoorStage.Start;
            if (_pathInd < _path.Count)
            {
                var dr = _path[_pathInd] as Door;
                if (dr != null)
                    _doorVelocity =
                        (Vector3.Dot(dr.DoorCollider.bounds.center - transform.position, dr.Facing) * dr.Facing)
                            .normalized;
            }
        }
    }



    protected void OnTriggerEnter2D(Collider2D col)
    {
        var room = col.GetComponent<Room>();
        if (room != null)
        {
            if (_targetPlayer != null && _targetPlayer.Character.Room != room)
                _targetPlayer = null;
        }
    }
}
