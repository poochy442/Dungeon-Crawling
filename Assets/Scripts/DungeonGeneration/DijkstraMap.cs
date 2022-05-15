using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraMap
{
	public Dictionary<Room, List<Room>> Map { get; private set; }
	public Room BossRoom { get; private set; }
	public List<Room> MainPath { get; private set; }

	List<Room> _rooms;

    public DijkstraMap(Room start, List<Room> rooms)
	{
		_rooms = rooms;
		Map = new Dictionary<Room, List<Room>>();

		Explore(start, rooms);
		BossRoom = FindBossroom();
		MainPath = Map[BossRoom];
	}

	void Explore(Room room, List<Room> path)
	{
		Map.Add(room, path);
		
		foreach(Room connection in room.ConnectedTo)
		{
			if(Map.ContainsKey(connection))
			{
				if(Map[connection].Count > path.Count + 1)
				{
					path.Add(connection);
					Explore(connection, path);
				}
			} else
			{
				path.Add(connection);
				Explore(connection, path);
			}
		}
	}

	Room FindBossroom()
	{
		Room furthestRoom = _rooms[0];
		foreach(Room r in _rooms)
		{
			if(Map[furthestRoom].Count < Map[r].Count)
			{
				furthestRoom = r;
			}
		}

		return furthestRoom;
	}

}
