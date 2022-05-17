using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public List<GameObject> enemies;
	public List<GameObject> bosses;

    // Start is called before the first frame update
    void Start()
    {
        List<Room> rooms = BSPDungeonGeneration.instance.SpawnableRooms;
		DijkstraMap DijkstraMap = BSPDungeonGeneration.instance.DijkstraMap;

		rooms.Remove(DijkstraMap.BossRoom);
		SpawnEnemies(DijkstraMap.BossRoom, 6, 3, true);

		foreach(Room r in rooms)
		{
			SpawnEnemies(r, 8, 5, false);
		}
    }

	void SpawnEnemies(Room room, int maxEnemies, int maxEnemyGroups, bool boss)
	{
		int enemiesToSpawn = Random.Range(3, maxEnemies);
		int enemygroups = Random.Range(2, maxEnemyGroups);
		int groupCounter = enemygroups;

		Vector3 minPosition = room.RoomObject.transform.position - new Vector3(room.Rect.width * 2 - 2, 0, room.Rect.height * 2 - 2);

		List<Vector3> usedPositions = new List<Vector3>();

		if(boss)
		{
			Vector3 bossPosition = new Vector3(room.RoomObject.transform.position.x, 0.5f, room.RoomObject.transform.position.z);
			GameObject.Instantiate(bosses[Random.Range(0, bosses.Count)], bossPosition, Quaternion.identity);
			usedPositions.Add(bossPosition);
			groupCounter--;
		}

		while(groupCounter > 0)
		{
			int attemptCounter = 0;
			bool locationFound = false;
			Vector3 groupLocation;
			do {
				groupLocation = minPosition + new Vector3(Random.Range(0f, room.Rect.width * 4 - 2), 0, Random.Range(0f, room.Rect.height * 4 - 2));
				if(checkLocation(groupLocation, usedPositions))
					locationFound = true;
			} while(!locationFound && attemptCounter < 5);

			if(!locationFound)
			{
				Debug.Log($"No more locations could be found, spawning no more units in room {room.Id}");
				return;
			}

			for(int i = 0; i < enemiesToSpawn; i++)
			{
				Vector3 enemyPosition = groupLocation + (Quaternion.AngleAxis((360 / enemiesToSpawn) * i, Vector3.up) * room.RoomObject.transform.forward);
				GameObject.Instantiate(enemies[Random.Range(0, enemies.Count)], enemyPosition, Quaternion.identity);
				// Debug.Log($"Enemy spawned at: {enemyPosition}");
			}
			groupCounter--;
		}
	}

	bool checkLocation(Vector3 location, List<Vector3> usedLocations)
	{
		foreach(Vector3 l in usedLocations)
		{
			if((location - l).magnitude < 3f)
				return false;
		}
		return true;
	}
}
