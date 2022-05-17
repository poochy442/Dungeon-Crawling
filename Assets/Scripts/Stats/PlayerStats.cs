using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
	public float experience;
	public int level;
	public Stat spellDamage;

	public float ExperienceToLevel { get { return 100 + (level - 1) * 10; } }

    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;

		armor.SetBaseValue(PlayerPrefs.GetFloat("Armor", 0));
		damage.SetBaseValue(PlayerPrefs.GetFloat("Damage", 6));
		spellDamage.SetBaseValue(PlayerPrefs.GetFloat("SpellDamage", 10));
		maxHealth.SetBaseValue(PlayerPrefs.GetFloat("MaxHealth", 50));
		currentHealth = maxHealth.GetValue();
		experience = PlayerPrefs.GetFloat("XP", 0);
		level = PlayerPrefs.GetInt("Level", 1);
    }

	void OnEquipmentChanged(Mutation newMutation, Mutation oldMutation)
	{
		if(newMutation != null)
		{
			armor.AddModifier(newMutation.armorModifier);
			damage.AddModifier(newMutation.damageModifier);
		}

		if(oldMutation != null)
		{
			armor.RemoveModifier(oldMutation.armorModifier);
			damage.RemoveModifier(oldMutation.damageModifier);
		}
	}

	public void GainExperience(float xp)
	{
		experience += xp;
		// Debug.Log($"Gained {xp} XP, now at {experience}");

		if(experience >= ExperienceToLevel)
		{
			experience -= (100 + (level - 1 * 10));
			level++;
			damage.SetBaseValue(damage.GetBaseValue() + 2);
			spellDamage.SetBaseValue(damage.GetBaseValue() + 4);
			maxHealth.SetBaseValue(maxHealth.GetBaseValue() + 2);
			currentHealth = maxHealth.GetValue();
		}
	}

	public override void Die()
	{
		GameManager.instance.Lose();
	}
}
