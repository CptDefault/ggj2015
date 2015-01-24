using System.Linq;
using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, ITraversable
{
    public Room[] Rooms;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private int _playerCount;
    private bool _locked;
    private bool _open;

    protected void Awake()
    {
        foreach (var room in Rooms)
        {
            room.AddDoor(this);
        }

        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

#if UNITY_EDITOR
    protected void OnDrawGizmosSelected()
    {
        foreach (var room in Rooms)
        {
            Gizmos.DrawLine(transform.position, room.transform.position);
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.Label((room.transform.position) + Vector3.back, (RoomDistance(room) * 100).ToString());
        }
    }

    protected void Reset()
    {
        print("Resetting!");
        Rooms = FindObjectsOfType<Room>().OrderBy(room => RoomDistance(room)).Take(2).ToArray();

    }

    private float RoomDistance(Room room)
    {
        PolygonCollider2D pc = room.collider2D as PolygonCollider2D;

        if (pc == null)
            return room.collider2D.bounds.SqrDistance(transform.position);

        float minDistanceSqr = Mathf.Infinity;

        // Scan all collider points to find nearest
        for (int index = 0; index < pc.points.Length; index++)
        {
            Vector3 colliderPoint = room.transform.TransformPoint(pc.points[index]);

            Vector3 diff = transform.position - colliderPoint;
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
            }

            Vector3 nextPoint = room.transform.TransformPoint(pc.points[(index + 1)%pc.points.Length]);
            Vector3 toNext = nextPoint - colliderPoint;
            if (Vector3.Dot(toNext, diff) > 0)
            {
                if (Vector3.Dot(toNext, nextPoint - transform.position) > 0)
                {
                    Vector3 perp = Vector3.Cross(toNext, Vector3.forward).normalized;
                    float dist = Vector3.Dot(perp, diff);
                    distSqr = dist*dist;

                    if (distSqr < minDistanceSqr)
                    {
                        minDistanceSqr = distSqr;
                    }
                }
            }
        }

        return minDistanceSqr;
    }
#endif

    public bool Locked
    {
        get { return _locked; }
        set
        {
            _locked = value;

            _spriteRenderer.color = value ? Color.red : Color.white;
        }
    }

    public bool Open
    {
        get { return _open; }
        set
        {
            _open = value;
            _collider.enabled = !_open;
            _spriteRenderer.enabled = !_open;
        }
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        var player = col.GetComponent<PlayerManager>();

        if (player != null && !_locked)
            _playerCount++;
        Open = _playerCount > 0;
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        var player = col.GetComponent<PlayerManager>();

        if (player != null)
            _playerCount--; 
        Open = _playerCount > 0;
    }
}
