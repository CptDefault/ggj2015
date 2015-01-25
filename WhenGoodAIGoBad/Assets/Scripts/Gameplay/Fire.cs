using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour
{

    public float FireSpreadInternalTime = 4;
    public float FireSpreadExternalTime = 12;
    private Room _room;

    protected void Start()
    {
        var overlapPointAll = Physics2D.OverlapPointAll(transform.position, 1 << 2);
        foreach (var col in overlapPointAll)
        {
            _room = col.GetComponent<Room>();
            if (_room != null)
            {
                _room.AddFire(this);
                break;
            }
        }
        gameObject.SetActive(false);
    }

    protected void FixedUpdate()
    {
        if (Random.value < 1 / FireSpreadInternalTime * Time.fixedDeltaTime)
        {
            for (int i = 0; i < 30; i++)
            {
                var fire = _room.Fire[Random.Range(0, _room.Fire.Count)];
                if(fire.gameObject.activeSelf)
                    continue;

                fire.gameObject.SetActive(true);

                break;
            }
        }

        foreach (var door in _room.Doors)
        {
            if (Random.value < 1 / FireSpreadExternalTime * Time.fixedDeltaTime)
            {
                var room = door.Rooms[door.Rooms[0] == _room ? 1 : 0];

                if (room.Fire.Count == 0)
                    continue;

                for (int i = 0; i < 30; i++)
                {
                    var fire = room.Fire[Random.Range(0, room.Fire.Count)];
                    if (fire.gameObject.activeSelf)
                        continue;

                    fire.gameObject.SetActive(true);

                    break;
                }
            }
        }
    }
}
