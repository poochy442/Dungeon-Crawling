using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public List<GameObject> enemies;

    // Start is called before the first frame update
    void Start()
    {
		Debug.Log("Spawning enemies");
        List<Room> rooms = BSPDungeonGeneration.instance.SpawnableRooms;

		

		foreach(Room r in rooms)
		{
			int enemiesToSpawn = Random.Range(2, 6);
			Debug.Log("Spawning for room " + r.Id + " at location " + r.RoomObject.transform.position);
			Rect spawnableArea = new Rect(r.RoomObject.transform.position.x, r.RoomObject.transform.position.z, (r.Rect.width) - 2, (r.Rect.height) - 2);

			for(int i = 0; i < enemiesToSpawn; i++)
			{
				Vector3 enemyPosition = new Vector3(spawnableArea.center.x, 0.5f, spawnableArea.center.y)
					+ (Quaternion.AngleAxis((360 / enemiesToSpawn) * i, Vector3.up) * r.RoomObject.transform.forward);
				GameObject.Instantiate(enemies[Random.Range(0, enemies.Count)], enemyPosition, Quaternion.identity);
				Debug.Log("Enemy spawned at: " + enemyPosition);
			}
		}
    }
}
