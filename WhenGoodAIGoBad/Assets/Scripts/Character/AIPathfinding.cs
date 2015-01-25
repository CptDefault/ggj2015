using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class AIPathfinding
{
    private class RoomCostPair
    {
        public Room Room;
        public float Cost;
        public List<ITraversable> Path; 
        public RoomCostPair(Room room, float cost, List<ITraversable> path)
        {
            Room = room;
            Cost = cost;
            Path = path;
        }
    }

	public static List<ITraversable> PathToPoint(Vector3 startPosition, Vector3 endPosition)
	{
        Room startRoom = FindRoom(startPosition);
        Room goalRoom = FindRoom(endPosition);

	    HashSet<Room> closed = new HashSet<Room>();
        List<RoomCostPair> frontier = new List<RoomCostPair>{new RoomCostPair(startRoom, 0, new List<ITraversable>{startRoom})};
 
        while (frontier.Count > 0)
        {
            RoomCostPair node = frontier[0];
            foreach (var pair in frontier)
            {
                if (pair.Cost < node.Cost && !closed.Contains(pair.Room))
                    node = pair;
            }

            if (goalRoom == node.Room)
                return node.Path;

            frontier.Remove(node);
            closed.Add(node.Room);

            foreach (var door in node.Room.Doors)
            {
                var room = door.Rooms[door.Rooms[0] == node.Room ? 1 : 0];
                var next = new RoomCostPair(room, node.Cost, new List<ITraversable>(node.Path));
                next.Cost += Vector3.Distance(node.Room.transform.position, door.transform.position);
                next.Cost += Vector3.Distance(door.transform.position, room.transform.position);

                next.Path.Add(door);
                next.Path.Add(room);

                frontier.Add(next);
            }
        }
	    return new List<ITraversable>{startRoom};
	}

    private static Room FindRoom(Vector3 position)
    {
        Room room = null;
        var collider2Ds = Physics2D.OverlapPointAll(position);
        foreach (var col in collider2Ds)
        {
            room = col.GetComponent<Room>();
            if (room != null)
                return room;
        }

        float bestDist = float.MaxValue;
        foreach (var r in GameManager.Instance.Rooms)
        {
            var bounds = r.collider2D.bounds;
            if (bounds.Contains(position))
            {
                return r;
            }

            float dist = bounds.SqrDistance(position);
            if (dist < bestDist)
            {
                bestDist = dist;
                room = r;
            }
        }
        return room;
    }
}
