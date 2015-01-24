using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class AIController : MonoBehaviour
{
    private List<ITraversable> _path;
    private int _pathInd;
    private CharacterController _characterController;


    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    protected void Update()
    {
        if (_path == null || _pathInd >= _path.Count)
        {
            var room = GameManager.Instance.Rooms[Random.Range(0, GameManager.Instance.Rooms.Count)];
            AIPathfinding.PathToPoint(transform.position, room.transform.position);
            _pathInd = 0;
        }

        Vector2 toTarget = ((MonoBehaviour)_path[_pathInd]).transform.position - transform.position;

        _characterController.SetDesiredSpeed(Vector2.ClampMagnitude(toTarget, 1));
    }
}
