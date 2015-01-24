using System.Linq;
using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, ITraversable
{
    public Room[] Rooms;

    public SpriteRenderer FrontRenderer;
    public SpriteRenderer[] BackRenderers;
    public Collider2D DoorCollider;

    public AudioClipContainer LockSound;
    public AudioClipContainer OpenSound;

    private int _playerCount;
    private bool _locked;
    private bool _open;
    private float _unlockTime;
    private bool _aiOverride;

    protected void Awake()
    {
        foreach (var room in Rooms)
        {
            room.AddDoor(this);
        }
        if (DoorCollider == null)
            DoorCollider = GetComponent<Collider2D>();

        Open = true;
    }

#if UNITY_EDITOR
    protected void OnDrawGizmosSelected()
    {
        foreach (var room in Rooms)
        {
            if(room == null)
                continue;
            
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

    [ContextMenu("Find Rooms")]
    protected void FindRooms()
    {
        print("Finding Rooms!");
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

            Open = _playerCount > 0 && !_locked;
        }
    }

    public bool Open
    {
        get { return _open; }
        set
        {
            if (_locked && !_aiOverride)
                value = false;

            if(_open != value)
                (value ? OpenSound : LockSound).Play();

            if (FrontRenderer != null) FrontRenderer.enabled = !_open;

            foreach (var backRenderer in BackRenderers)
            {
                backRenderer.enabled = _open;
            }

            _open = value;
            if (DoorCollider != null) DoorCollider.enabled = !_open;
        }
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        var player = col.GetComponent<CharacterController>();

        if (player != null)
            _playerCount++;

        if (col.GetComponent<AIController>() != null)
            _aiOverride = true;

        Open = _playerCount > 0 && !_locked || _aiOverride;
    }
    protected void OnTriggerExit2D(Collider2D col)
    {
        var player = col.GetComponent<CharacterController>();

        if (player != null)
            _playerCount--;

        if (col.GetComponent<AIController>() != null)
            _aiOverride = false;

        Open = _playerCount > 0 && !_locked || _aiOverride;
    }

    public void LockFor(float duration)
    {
        Locked = true;
        StartCoroutine(UnlockAtTime(Time.time + duration));
    }

    private IEnumerator UnlockAtTime(float t)
    {
        if (_unlockTime > 0)
        {
            _unlockTime = Mathf.Max(_unlockTime, t);
            yield break;
        }
        _unlockTime = t;
        while (Time.time < _unlockTime)
            yield return null;
        Locked = false;
        _unlockTime = -1;
    }
}
