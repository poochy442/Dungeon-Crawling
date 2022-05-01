using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MutationType {
	Brain,
	Body,
	Arm,
	Leg,
	Spirit
}

[CreateAssetMenu(fileName = "New Mutation", menuName = "Inventory/Mutation")]
public class Mutation : Item
{
    public MutationType mutationType;

	public int armorModifier;
	public int damageModifier;

	public override void Use()
	{
		base.Use();
		EquipmentManager.instance.Equip(this);
		RemoveFromInventory();
	}
}
