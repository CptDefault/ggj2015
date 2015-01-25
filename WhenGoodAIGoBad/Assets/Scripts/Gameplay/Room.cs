using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public interface ITraversable
{
};

public class Room : MonoBehaviour, ITraversable
{
    public readonly List<Door> Doors = new List<Door>();
    public readonly List<Fire> Fire = new List<Fire>();

    protected void Awake()
    {
        GameManager.RegisterRoom(this);
    }

    public void AddDoor(Door door)
    {
        Doors.Add(door);
    }

    public void AddFire(Fire fire)
    {
        Fire.Add(fire);
    }
}
