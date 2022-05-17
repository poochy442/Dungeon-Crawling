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

	public static Mutation GenerateMutation()
	{
		Mutation newMutation = ScriptableObject.CreateInstance<Mutation>();

		newMutation.mutationType = PickMutation(Random.Range(0f, 1f));
		newMutation.icon = PickSprite(newMutation.mutationType);
		newMutation.armorModifier = Random.Range(1, 5);
		newMutation.damageModifier = Random.Range(1, 5);
		newMutation.name = $"{newMutation.mutationType} mutation";

		return newMutation;
	}

	static MutationType PickMutation(float r)
	{
		if(r < 0.2f)
			return MutationType.Arm;
		if(r < 0.4f)
			return MutationType.Body;
		if(r < 0.6f)
			return MutationType.Brain;
		if(r < 0.8f)
			return MutationType.Leg;

		return MutationType.Spirit;
	}

	static Sprite PickSprite(MutationType type)
	{
		switch(type)
		{
			case MutationType.Arm:
				return Resources.Load<Sprite>("Arm");
			case MutationType.Body:
				return Resources.Load<Sprite>("Body");;
			case MutationType.Brain:
				return Resources.Load<Sprite>("Brain");;
			case MutationType.Leg:
				return Resources.Load<Sprite>("Leg");;
			default:
				return Resources.Load<Sprite>("Spirit");;
		}
	}
}
