using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class AIController : MonoBehaviour
{
    private List<ITraversable> _path;
    private int _pathInd;
    private CharacterController _characterController;

    public Transform TestThingy;

    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected void Update()
    {
        if (_path == null || _pathInd >= _path.Count)
        {
            var room = GameManager.Instance.Rooms[Random.Range(0, GameManager.Instance.Rooms.Count)];
            _path = AIPathfinding.PathToPoint(transform.position, room.transform.position);
            _pathInd = 0;
        }
        TestThingy = ((MonoBehaviour)_path[_pathInd]).transform;

        var target = ((MonoBehaviour) _path[_pathInd]).transform.position;
        Vector2 toTarget = target - transform.position;

        _characterController.SetDesiredSpeed(toTarget.normalized);

        if (Vector3.SqrMagnitude(transform.position - target) < 0.2f)
            _pathInd++;
    }
}
