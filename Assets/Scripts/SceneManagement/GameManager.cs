using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	
	void Awake()
	{
		if(instance == null)
		{
			DontDestroyOnLoad(this);
			instance = this;
		}
	}

	public void EnterDungeon()
	{
		SavePlayerStats();
		LevelLoader.instance.LoadLevel(1);
	}

	public void ExitDungeon()
	{
		SavePlayerStats();
		LevelLoader.instance.LoadLevel(0);
	}

	public void SavePlayerStats()
	{
		PlayerStats stats = PlayerManager.instance.playerStats;

		PlayerPrefs.SetFloat("Armor", stats.armor.GetBaseValue());
		PlayerPrefs.SetFloat("Damage", stats.damage.GetBaseValue());
		PlayerPrefs.SetFloat("MaxHealth", stats.maxHealth.GetBaseValue());
		PlayerPrefs.SetFloat("XP", stats.experience);
		PlayerPrefs.Save();
	}
}
