using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
	public float experience;

    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;

		armor.SetBaseValue(PlayerPrefs.GetFloat("Armor", 0));
		damage.SetBaseValue(PlayerPrefs.GetFloat("Damage", 4));
		maxHealth.SetBaseValue(PlayerPrefs.GetFloat("MaxHealth", 100));
		experience = PlayerPrefs.GetFloat("XP", 0);
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
}
