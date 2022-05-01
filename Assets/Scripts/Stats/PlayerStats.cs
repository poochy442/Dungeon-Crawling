using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    // Start is called before the first frame update
    void Start()
    {
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
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
			damage.AddModifier(newMutation.damageModifier);
		}
	}
}
