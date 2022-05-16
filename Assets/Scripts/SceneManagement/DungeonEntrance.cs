using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
	GameManager _manager;
	bool hasEntered = false;

	void Start()
	{
		_manager = GameManager.instance;
	}

    // Check if player enters the dungeon, if they do enter scene
    void Update()
    {
        if(!hasEntered && PlayerManager.instance.player != null)
		{
			float distance = (PlayerManager.instance.player.transform.position - transform.position).magnitude;

			if(distance < 3f)
			{
				_manager.EnterDungeon();
				hasEntered = true;
			}
		}
    }
}
