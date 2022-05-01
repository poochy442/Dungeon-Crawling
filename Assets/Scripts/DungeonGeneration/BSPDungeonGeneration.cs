using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Created with inspiration from the article: https://varav.in/archive/dungeon/ about Dungeon Generation using Binary Space Partitioning */
public enum Direction
{
	North, South, East, West, None
}
public struct Room
{
	public int Id {get; set;}
	public Rect Rect {get; set;}
	public GameObject RoomObject {get; set;}
	public Dictionary<Direction, List<Room>> Connections {get; set;}
	public Dictionary<Direction, GameObject> CorridorStarts {get; set;}
	public Room(int id, Rect rect, GameObject roomObject, Dictionary<Direction, GameObject> corridorStarts)
	{
		Id = id;
		Rect = rect;
		RoomObject = roomObject;
		Connections = new Dictionary<Direction, List<Room>>();
		CorridorStarts = corridorStarts;
	}
}

public struct Corridor
{
	public Room StartRoom {get; set;}
	public Room EndRoom {get; set;}
	public GameObject CorridorObject {get; set;}
	public Corridor(Room start, Room end, GameObject corridorObject)
	{
		StartRoom = start;
		EndRoom = end;
		CorridorObject = corridorObject;
	}
}

public class BSPDungeonGeneration : MonoBehaviour
{
	public float minRoomLength = 5f, maxRoomLength = 20f, roomSpacing = 2f;
	public int maxRoomAmount = 8;
	public List<GameObject> floorPrefabs, wallPrefabs, pillarPrefabs;
	public GameObject emptyPrefab;	

	List<Rect> _partitions;
	List<Room> _rooms;
	List<Corridor> _corridors;

    // Start is called before the first frame update
    void Start()
    {
		Vector2 maxDiagonal = new Vector2(maxRoomLength + roomSpacing + Random.Range(0, 10), maxRoomLength + roomSpacing + Random.Range(0, 10)) * maxRoomAmount;
		float angleBetween = Vector2.Angle(Vector2.right, maxDiagonal) * (Mathf.PI / 180);
		float generationWidth = Mathf.Cos(angleBetween) * maxDiagonal.magnitude / 4;
		float generationHeight = Mathf.Sin(angleBetween) * maxDiagonal.magnitude / 4;

		Debug.Log("Generating dungeon with size: " + generationWidth + " x " + generationHeight);

        _partitions = BinarySpacePartition(generationWidth, generationHeight);
		_rooms = CreateRooms();
		_corridors = CreateCorridors();
    }

	
    List<Rect> BinarySpacePartition(float generationWidth, float generationHeight)
	{
		List<Rect> rects = new List<Rect>();
		Stack<Rect> workingRects = new Stack<Rect>();
		workingRects.Push(new Rect(0, 0, generationWidth, generationHeight));

		// Keep splitting rects up until they are all at the correct size and added to the list
		while(workingRects.Count > 0)
		{
			Rect currentRect = workingRects.Pop();
			float balance = Random.Range(0.25f, 0.75f), splitRatio = Random.Range(1.5f, 2.5f);
			
			if(currentRect.width / 2 < minRoomLength || currentRect.height / 2 < minRoomLength){
				// Ensure no side is too big
				if(currentRect.width > maxRoomLength) // Split vertically
				{
					Rect newRect = currentRect;
					currentRect.xMax -= currentRect.width / splitRatio;
					newRect.xMin = currentRect.xMax;

					workingRects.Push(currentRect);
					workingRects.Push(newRect);
				} else if (currentRect.height > maxRoomLength) // Split horizontally
				{
					Rect newRect = currentRect;
					currentRect.yMax -= currentRect.height / splitRatio;
					newRect.yMin = currentRect.yMax;

					workingRects.Push(currentRect);
					workingRects.Push(newRect);
				} else
				{
					rects.Add(currentRect);
				}
			} else {
				if(Random.Range(0f, 1f) < balance) // Split horizontally
				{
					Rect newRect = currentRect;
					currentRect.yMax -= currentRect.height / splitRatio;
					newRect.yMin = currentRect.yMax;

					workingRects.Push(currentRect);
					workingRects.Push(newRect);
				} else // Split vertically
				{
					Rect newRect = currentRect;
					currentRect.xMax -= currentRect.width / splitRatio;
					newRect.xMin = currentRect.xMax;

					workingRects.Push(currentRect);
					workingRects.Push(newRect);
				}
			}
		}
		
		return rects;
	}

	List<Room> CreateRooms()
	{
		List<Room> r = new List<Room>();
		int roomAmount = 0;
		
		List<int> availableRooms = new List<int>();
		for(int i = 0; i < _partitions.Count; i++)
		{
			availableRooms.Add(i);
		}

		for(int i = 0; i < _partitions.Count; i++){
			// Guards
			if(roomAmount >= maxRoomAmount)
				return r;
			else if(_partitions[i].width - roomSpacing < minRoomLength || _partitions[i].height - roomSpacing < minRoomLength)
				continue;
			
			// Choose random rooms in case there are more rooms than max rooms
			int index = Random.Range(0, availableRooms.Count);
			Rect chosenRect = _partitions[availableRooms[index]];
			availableRooms.RemoveAt(index);

			// Spawn rooms
			int w = (int) Mathf.Round(Random.Range(minRoomLength, Mathf.Min((chosenRect.width - roomSpacing), maxRoomLength)));
			int h = (int) Mathf.Round(Random.Range(minRoomLength, Mathf.Min((chosenRect.height - roomSpacing), maxRoomLength)));
			int x = (int) Mathf.Round(chosenRect.center.x);
			int y = (int) Mathf.Round(chosenRect.center.y);
			Dictionary<Direction, GameObject> corridorStarts;
			GameObject roomObject = CreateRoom(x, y, w, h, i, out corridorStarts);
			Room newRoom = new Room(i, new Rect(x - (w / 2), y - (h / 2), w, h), roomObject, corridorStarts);
			r.Add(newRoom);
			roomAmount++;
		}
		return r;
	}

	GameObject CreateRoom(int xPosition, int yPosition, int width, int height, int id, out Dictionary<Direction, GameObject> corridorStarts)
	{
		corridorStarts = new Dictionary<Direction, GameObject>();

		// Parent object
		Vector3 location = new Vector3(xPosition, 0, yPosition);
		GameObject room = GameObject.Instantiate(emptyPrefab, location, Quaternion.identity);
		room.name = "Room " + id;

		// Floors
		for(int i = - width / 2; i < width / 2; i++){
			for(int j = - height / 2; j < height / 2; j++){
				GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), location + new Vector3(i, 0, j), Quaternion.identity, room.transform);
				f.transform.position += new Vector3(0, 0, 0.5f);
				f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
			}
		}

		// Walls
		Quaternion xQuat = new Quaternion(); xQuat.SetLookRotation(new Vector3(1, 0, 0));
		Quaternion zQuat = new Quaternion(); zQuat.SetLookRotation(new Vector3(0, 0, 1));

		float rand1 = Random.Range(0.25f, 0.75f), rand2 = Random.Range(0.25f, 0.75f);
		for(int i = - height / 2; i < height / 2; i++)
		{
			GameObject w1 = GameObject.Instantiate(ChooseInstance(wallPrefabs), location + new Vector3(- width / 2, 0, i), xQuat, room.transform);
			w1.transform.position += new Vector3(-0.5f, 0, 0.5f);
			if(!corridorStarts.ContainsKey(Direction.West))
			{
				if(rand1 < (float) (i + height / 2) / height){
					corridorStarts.Add(Direction.West, w1);
				}
			}

			GameObject w2 = GameObject.Instantiate(ChooseInstance(wallPrefabs), location + new Vector3(width / 2, 0, i), xQuat, room.transform);
			w2.transform.Rotate(new Vector3(0, 1, 0), 180);
			w2.transform.position += new Vector3(-0.5f, 0, 0.5f);
			if(!corridorStarts.ContainsKey(Direction.East))
			{
				if(rand2 < (float) (i + height / 2) / height){
					corridorStarts.Add(Direction.East, w2);
				}
			}
		}

		float rand3 = Random.Range(0.25f, 0.75f), rand4 = Random.Range(0.25f, 0.75f);
		for(int i = - width / 2; i < width / 2; i++)
		{
			GameObject w1 = GameObject.Instantiate(ChooseInstance(wallPrefabs), location + new Vector3(i, 0, - height / 2), zQuat, room.transform);
			if(!corridorStarts.ContainsKey(Direction.South))
			{
				if(rand3 < (float) (i + width / 2) / width){
					corridorStarts.Add(Direction.South, w1);
				}
			}
			
			GameObject w2 = GameObject.Instantiate(ChooseInstance(wallPrefabs), location + new Vector3(i, 0, height / 2), zQuat, room.transform);
			w2.transform.Rotate(new Vector3(0, 1, 0), 180);
			if(!corridorStarts.ContainsKey(Direction.North))
			{
				if(rand4 < (float) (i + width / 2) / width){
					corridorStarts.Add(Direction.North, w2);
				}
			}
		}

		// Pillar
		GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), location + new Vector3(- width / 2, 0, - height / 2), Quaternion.identity, room.transform);
		p1.transform.position += new Vector3(-0.5f, 1f, 0); // Correction
		p1.transform.position += new Vector3(0.25f, 0, 0.25f); // Placement
		p1.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), location + new Vector3(width / 2, 0, - height / 2), Quaternion.identity, room.transform);
		p2.transform.position += new Vector3(-0.5f, 1f, 0); // Correction
		p2.transform.position += new Vector3(-0.25f, 0, 0.25f); // Placement
		p2.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		GameObject p3 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), location + new Vector3(- width / 2, 0, height / 2), Quaternion.identity, room.transform);
		p3.transform.position += new Vector3(-0.5f, 1f, 0); // Correction
		p3.transform.position += new Vector3(0.25f, 0, -0.25f); // Placement
		p3.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		GameObject p4 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), location + new Vector3(width / 2, 0, height / 2), Quaternion.identity, room.transform);
		p4.transform.position += new Vector3(-0.5f, 1f, 0); // Correction
		p4.transform.position += new Vector3(-0.25f, 0, -0.25f); // Placement
		p4.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));

		return room;
	}

	// Helper function to find the nearest unconnected room
	Room FindNearestUnconnectedRoom(List<Room> connected, List<Room> remaining, out Room c)
	{
		Room connection = connected[0], nearest = remaining[0];
		float distanceToClosest = float.PositiveInfinity;

		for(int i = 0; i < connected.Count; i++)
		{
			for(int j = 0; j < remaining.Count; j++)
			{
				if((connected[i].Rect.center - remaining[j].Rect.center).magnitude < distanceToClosest)
				{
					connection = connected[i];
					nearest = remaining[j];
					distanceToClosest = (connection.Rect.center - remaining[j].Rect.center).magnitude;
				}
			}
		}
		c = connection;
		return nearest;
	}

	List<Direction> FindCorridorDirection(Room r1, Room r2)
	{
		List<Direction> directions = new List<Direction>();

		// Check y-direction
		if(r1.Rect.yMin > r2.Rect.yMax) // South
		{
			directions.Add(Direction.South);
		} else if(r1.Rect.yMax < r2.Rect.yMin) // North
		{
			directions.Add(Direction.North);
		}

		// Check x-direction
		if(r1.Rect.xMin > r2.Rect.xMax) // West
		{
			directions.Add(Direction.West);
		} else if(r1.Rect.xMax < r2.Rect.xMin) // East
		{ 
			directions.Add(Direction.East);
		}

		return directions;
	}

	Vector3 GetDirectionVector(Direction direction)
	{
		switch(direction)
		{
			case Direction.North:
				return new Vector3(0, 0, 1);
			case Direction.South:
				return new Vector3(0, 0, -1);
			case Direction.East:
				return new Vector3(1, 0, 0);
			case Direction.West:
				return new Vector3(-1, 0, 0);
		}
		return new Vector3(0, 0, 0);
	}

	// Creates the links between the rooms and then generates the Gameobjects
	List<Corridor> CreateCorridors()
	{
		GameObject corridorsObject = GameObject.Instantiate(emptyPrefab);
		corridorsObject.name = "Corridors";

		List<Corridor> corridors = new List<Corridor>();
		List<Room> connected = new List<Room>(_rooms.GetRange(0, 1)), remaining = new List<Room>(_rooms.GetRange(1, _rooms.Count - 1));

		while(remaining.Count > 0)
		{
			Room connection, nearest = FindNearestUnconnectedRoom(connected, remaining, out connection);
			
			List<Direction> connectionDirections = FindCorridorDirection(connection, nearest), nearestDirections = FindCorridorDirection(nearest, connection);
			Direction connectionDirection = connectionDirections[Random.Range(0, connectionDirections.Count)], nearestDirection = nearestDirections[Random.Range(0, nearestDirections.Count)];
			
			if(!connection.Connections.ContainsKey(connectionDirection))
				connection.Connections.Add(connectionDirection, new List<Room>(){nearest});
			else
				connection.Connections[connectionDirection].Add(nearest);
			
			if(!nearest.Connections.ContainsKey(nearestDirection))
				nearest.Connections.Add(nearestDirection, new List<Room>(){connection});
			else
				nearest.Connections[nearestDirection].Add(connection);

			remaining.Remove(nearest);
			connected.Add(nearest);
		}

		// Create a corridor for each connection
		List<Vector3> corridorTileLocations = new List<Vector3>(), roomTileLocations = new List<Vector3>();
		List<List<Room>> createdConnections = new List<List<Room>>();
		foreach(Room r1 in _rooms)
		{
			if(r1.Connections.Count > 0)
			{
				foreach(Direction d in new List<Direction>(){Direction.North, Direction.South, Direction.East, Direction.West})
				{
					if(!r1.Connections.ContainsKey(d))
						continue;
					
					List<Room> roomConnections = r1.Connections[d];

					foreach(Room r2 in roomConnections)
					{
						bool alreadyCreated = false;
						foreach(List<Room> conn in createdConnections)
						{
							if(conn.Contains(r1) && conn.Contains(r2))
							{
								alreadyCreated = true;
								break;
							}
						}

						Debug.Log(r1.Id + " -> " + r2.Id + " is created: " + alreadyCreated);
						if(!alreadyCreated)
						{
							GameObject newCorridor = CreateCorridor(r1, r2, corridorTileLocations, roomTileLocations, corridorsObject);
							corridors.Add(new Corridor(r1, r2, newCorridor));
							createdConnections.Add(new List<Room>(){r1, r2});
						}
					}
				}
			}
		}

		// Create corridor walls
		Quaternion xQuat = new Quaternion(); xQuat.SetLookRotation(new Vector3(1, 0, 0));
		Quaternion zQuat = new Quaternion(); zQuat.SetLookRotation(new Vector3(0, 0, 1));

		GameObject walls = GameObject.Instantiate(emptyPrefab, corridorsObject.transform);
		walls.name = "Walls";

		foreach(Vector3 tileLocation in corridorTileLocations)
		{
			// Walls
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(1, 0, 0))) // E
			{
				GameObject w = GameObject.Instantiate(ChooseInstance(wallPrefabs), tileLocation + new Vector3(0.5f, 0, 0), xQuat, walls.transform);
				w.transform.Rotate(new Vector3(0, 1, 0), 180);
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0))) // W
			{
				GameObject w = GameObject.Instantiate(ChooseInstance(wallPrefabs), tileLocation + new Vector3(-0.5f, 0, 0), xQuat, walls.transform);
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, 1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, 1))) // N
			{
				GameObject w = GameObject.Instantiate(ChooseInstance(wallPrefabs), tileLocation + new Vector3(0, 0, 0.5f), zQuat, walls.transform);
				w.transform.Rotate(new Vector3(0, 1, 0), 180);
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, -1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, -1))) // S
			{
				GameObject w = GameObject.Instantiate(ChooseInstance(wallPrefabs), tileLocation + new Vector3(0, 0, -0.5f), zQuat, walls.transform);
			}

			// Pillars
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(1, 0, 0)) &&
				!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, 1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, 1))) // NE
			{
				GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(0.5f, 1, 0.5f), Quaternion.identity, walls.transform);
				p1.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
				GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(-0.5f, 1, -0.5f), Quaternion.identity, walls.transform);
				p2.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(1, 0, 0)) &&
				!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, -1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, -1))) // NW
			{
				GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(0.5f, 1, -0.5f), Quaternion.identity, walls.transform);
				p1.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
				GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(-0.5f, 1, 0.5f), Quaternion.identity, walls.transform);
				p2.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0)) &&
				!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, 1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, 1))) // SE
			{
				GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(-0.5f, 1, 0.5f), Quaternion.identity, walls.transform);
				p1.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
				GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(0.5f, 1, -0.5f), Quaternion.identity, walls.transform);
				p2.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
			}
			if(!corridorTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0)) && !roomTileLocations.Contains(tileLocation + new Vector3(-1, 0, 0)) &&
				!corridorTileLocations.Contains(tileLocation + new Vector3(0, 0, -1)) && !roomTileLocations.Contains(tileLocation + new Vector3(0, 0, -1))) // SW
			{
				GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(-0.5f, 1, -0.5f), Quaternion.identity, walls.transform);
				p1.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
				GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), tileLocation + new Vector3(0.5f, 1, 0.5f), Quaternion.identity, walls.transform);
				p2.transform.rotation  = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
			}
		}

		return corridors;
	}

	GameObject CreateCorridor(Room r1, Room r2, List<Vector3> corridorTileLocations, List<Vector3> roomTileLocations, GameObject corridorsObject)
	{
		GameObject corridor = GameObject.Instantiate(emptyPrefab, corridorsObject.transform);
		corridor.name = "Corridor " + r1.Id + "-" + r2.Id;
		List<Direction> r1Directions = FindCorridorDirection(r1, r2), r2Directions = FindCorridorDirection(r2, r1);
		Direction r1Direction = r1Directions[Random.Range(0, r1Directions.Count)], r2Direction = r2Directions[Random.Range(0, r2Directions.Count)];
		GameObject r1Wall = r1.CorridorStarts[r1Direction], r2Wall = r2.CorridorStarts[r2Direction];

		roomTileLocations.Add(r1Wall.transform.position - GetDirectionVector(r1Direction) * 0.5f);
		roomTileLocations.Add(r2Wall.transform.position - GetDirectionVector(r2Direction) * 0.5f);

		// Spawn door pillars
		Vector3 r1PillarOffset = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * GetDirectionVector(r1Direction) * 0.5f,
			r2PillarOffset = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * GetDirectionVector(r2Direction) * 0.5f;

		GameObject p1 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), r1Wall.transform.position + r1PillarOffset, Quaternion.identity, corridor.transform);
		p1.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		p1.transform.position += new Vector3(0, 1, 0);
		GameObject p2 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), r1Wall.transform.position - r1PillarOffset, Quaternion.identity, corridor.transform);
		p2.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		p2.transform.position += new Vector3(0, 1, 0);

		GameObject p3 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), r2Wall.transform.position + r2PillarOffset, Quaternion.identity, corridor.transform);
		p3.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		p3.transform.position += new Vector3(0, 1, 0);
		GameObject p4 = GameObject.Instantiate(ChooseInstance(pillarPrefabs), r2Wall.transform.position - r2PillarOffset, Quaternion.identity, corridor.transform);
		p4.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
		p4.transform.position += new Vector3(0, 1, 0);

		// Create floors
		if(r1Direction == Direction.North)
		{
			if(r2Direction == Direction.South)
			{
				int mainDirectionLength = (int) Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z),
					directionChangeIndex = Mathf.RoundToInt(Random.Range(mainDirectionLength * 0.4f, mainDirectionLength * 0.6f));
				Vector3 offset = new Vector3(0, 0, 0.5f);

				for(int i = 0; i < directionChangeIndex; i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), newTileLocation, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z++;
				}

				int direction = r1Wall.transform.position.x < r2Wall.transform.position.x ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x += direction;
				}

				while(r1Wall.transform.position.z + offset.z < r2Wall.transform.position.z)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z++;
				}
			} else {
				Vector3 offset = new Vector3(0, 0, -0.5f);

				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z++;
				}

				int direction = r1Wall.transform.position.x < r2Wall.transform.position.x ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x += direction;
				}
			}
		} else if(r1Direction == Direction.South)
		{
			if(r2Direction == Direction.North)
			{
				int mainDirectionLength = (int) Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z),
					directionChangeIndex = Mathf.RoundToInt(Random.Range(mainDirectionLength * 0.4f, mainDirectionLength * 0.6f));
				Vector3 offset = new Vector3(0, 0, -0.5f);

				for(int i = 0; i < directionChangeIndex; i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z--;
				}

				int direction = r1Wall.transform.position.x < r2Wall.transform.position.x ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x += direction;
				}

				while(r1Wall.transform.position.z + offset.z > r2Wall.transform.position.z)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z--;
				}
			} else {
				Vector3 offset = new Vector3(0, 0, 0.5f);

				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z--;
				}

				int direction = r1Wall.transform.position.x < r2Wall.transform.position.x ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x += direction;
				}
			}
		} else if(r1Direction == Direction.East)
		{
			if(r2Direction == Direction.West)
			{
				int mainDirectionLength = (int) Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x),
					directionChangeIndex = Mathf.RoundToInt(Random.Range(mainDirectionLength * 0.4f, mainDirectionLength * 0.6f));
				Vector3 offset = new Vector3(0.5f, 0, 0f);

				for(int i = 0; i < directionChangeIndex; i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x++;
				}

				int direction = r1Wall.transform.position.z < r2Wall.transform.position.z ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z += direction;
				}

				while(r1Wall.transform.position.x + offset.x < r2Wall.transform.position.x)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x++;
				}
			} else {
				Vector3 offset = new Vector3(-0.5f, 0, 0);

				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x++;
				}

				int direction = r1Wall.transform.position.z < r2Wall.transform.position.z ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z += direction;
				}
			}
		} else
		{
			if(r2Direction == Direction.East)
			{
				int mainDirectionLength = (int) Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x),
					directionChangeIndex = Mathf.RoundToInt(Random.Range(mainDirectionLength * 0.4f, mainDirectionLength * 0.6f));
				Vector3 offset = new Vector3(-0.5f, 0, 0f);

				for(int i = 0; i < directionChangeIndex; i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x--;
				}

				int direction = r1Wall.transform.position.z < r2Wall.transform.position.z ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z += direction;
				}

				while(r1Wall.transform.position.x + offset.x > r2Wall.transform.position.x)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x--;
				}
			} else {
				Vector3 offset = new Vector3(0.5f, 0, 0);

				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.x - r2Wall.transform.position.x); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.x--;
				}

				int direction = r1Wall.transform.position.z < r2Wall.transform.position.z ? 1 : -1;
				for(int i = 0; i < Mathf.Abs(r1Wall.transform.position.z - r2Wall.transform.position.z); i++)
				{
					Vector3 newTileLocation = r1Wall.transform.position + offset;
					if(!corridorTileLocations.Contains(newTileLocation))
					{
						GameObject f = GameObject.Instantiate(ChooseInstance(floorPrefabs), r1Wall.transform.position + offset, Quaternion.identity, corridor.transform);
						f.transform.Rotate(new Vector3(-90, Random.Range(0, 4) * 90, 0));
						corridorTileLocations.Add(newTileLocation);
					}
					offset.z += direction;
				}
			}
		}

		GameObject.Destroy(r1Wall); GameObject.Destroy(r2Wall);
		return corridor;
	}

	GameObject ChooseInstance(List<GameObject> references)
	{
		// Chooses instances based on a curve, so the more 'clean' elements are more common
		if(references.Count == 1) // Just return the element if there is only one
		{
			return references[0];
		}
		
		// Calculate the sum 1 + 2 + 3 ... Count
		float sum = 0.5f * references.Count * (references.Count + 1);
		
		// Choose instance
		int helperSum = 0;
		for(int i = 0; i < references.Count; i++)
		{
			helperSum += (references.Count - i);
			float r = Random.Range(0f, 1f);
			if(r <= helperSum / sum)
			{
				return references[i];
			}
		}

		// Default
		return references[0];
	}

	Dictionary<Rect, Color> gizmoColors;
	void OnDrawGizmosSelected()
	{
		if(_partitions != null)
		{
			if(gizmoColors == null)
			{
				gizmoColors = new Dictionary<Rect, Color>();
				int i = 1;
				foreach(Rect r in _partitions)
				{
					int rand = i % 6;
					gizmoColors.Add(r, rand == 1 ? Color.black : rand == 2 ? Color.red : rand == 3 ? Color.blue : rand == 4 ? Color.green : Color.gray);
					i++;
				}
			} else {
				foreach(Rect r in _partitions)
				{
					Gizmos.color = gizmoColors[r];
					Gizmos.DrawCube(new Vector3(r.center.x, 0, r.center.y), new Vector3(r.width, 2, r.height));
				}
			}
		}

		if(_corridors != null)
		{
			Gizmos.color = Color.red;
			
			foreach(Corridor corridor in _corridors)
			{
				Gizmos.DrawLine(corridor.StartRoom.RoomObject.transform.position, corridor.EndRoom.RoomObject.transform.position);
			}
		}
	}

}
