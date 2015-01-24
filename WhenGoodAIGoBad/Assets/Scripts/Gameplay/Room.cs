using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public interface ITraversable
{
};

public class Room : MonoBehaviour, ITraversable
{
    public readonly List<Door> Doors = new List<Door>();

    protected void Awake()
    {
        GameManager.RegisterRoom(this);
    }

    public void AddDoor(Door door)
    {
        Doors.Add(door);
    }
}
