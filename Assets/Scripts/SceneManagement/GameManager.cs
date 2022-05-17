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

	public void MainMenu()
	{
		SavePlayerStats();
		LevelLoader.instance.LoadLevel(0);
	}

	public void EnterTown()
	{
		SavePlayerStats();
		LevelLoader.instance.LoadLevel(1);
	}
	
	public void EnterGame()
	{
		LevelLoader.instance.LoadLevel(1);
	}

	public void EnterDungeon()
	{
		SavePlayerStats();
		LevelLoader.instance.LoadLevel(2);
	}

	public void Lose()
	{
		Time.timeScale = 0;
		GameObject deathScreen = GameObject.Find("Death Screen");
		CanvasGroup canvasGroup = deathScreen.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
	}

	public void Win()
	{
		Time.timeScale = 0;
		GameObject winScreen = GameObject.Find("Win Screen");
		CanvasGroup canvasGroup = winScreen.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1;
		canvasGroup.blocksRaycasts = true;
	}

	public void SavePlayerStats()
	{
		PlayerStats stats = PlayerManager.instance.playerStats;

		PlayerPrefs.SetFloat("Armor", stats.armor.GetBaseValue());
		PlayerPrefs.SetFloat("Damage", stats.damage.GetBaseValue());
		PlayerPrefs.SetFloat("SpellDamage", stats.spellDamage.GetBaseValue());
		PlayerPrefs.SetFloat("MaxHealth", stats.maxHealth.GetBaseValue());
		PlayerPrefs.SetFloat("XP", stats.experience);
		PlayerPrefs.SetInt("Level", stats.level);
		PlayerPrefs.Save();
	}
}
