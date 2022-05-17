using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public Button townButton;

    // Start is called before the first frame update
    void Start()
    {
		townButton.onClick.AddListener(OnTown);
    }

    public void OnTown()
	{
		PlayerStats stats = PlayerManager.instance.playerStats;
		
		PlayerPrefs.SetFloat("Armor", stats.armor.GetBaseValue());
		PlayerPrefs.SetFloat("Damage", stats.damage.GetBaseValue());
		PlayerPrefs.SetFloat("SpellDamage", stats.spellDamage.GetBaseValue());
		PlayerPrefs.SetFloat("MaxHealth", stats.maxHealth.GetBaseValue());
		PlayerPrefs.SetFloat("XP", stats.experience);
		PlayerPrefs.SetInt("Level", stats.level);
		PlayerPrefs.Save();

		Time.timeScale = 1;
		GameManager.instance.EnterTown();
	}
}
