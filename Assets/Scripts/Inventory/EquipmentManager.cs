using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton

	public static EquipmentManager instance;

	void Awake()
	{
		instance = this;
	}

	#endregion

	public delegate void OnEquipmentChanged(Mutation newMutation, Mutation oldMutation);
	public OnEquipmentChanged onEquipmentChanged;

	Inventory inventory;
	Mutation[] currentMutations;

	void Start()
	{
		inventory = Inventory.instance;
		int numSlots = System.Enum.GetNames(typeof(MutationType)).Length;
		currentMutations = new Mutation[numSlots];
	}

	public void Equip(Mutation newMutation)
	{
		int index = (int) newMutation.mutationType;

		Mutation oldMutation = currentMutations[index];

		if(currentMutations[index] != null)
			inventory.Add(oldMutation);
		
		if(onEquipmentChanged != null)
		{
			onEquipmentChanged(newMutation, oldMutation);
		}

		currentMutations[index] = newMutation;
	}

	public void UnEquip(int index)
	{
		Mutation oldMutation = currentMutations[index];
		
		if(currentMutations[index] != null)
			inventory.Add(oldMutation);
		
		if(onEquipmentChanged != null)
		{
			onEquipmentChanged(null, oldMutation);
		}
		
		currentMutations[index] = null;
	}

	public void UnEquipAll()
	{
		for(int i = 0; i < currentMutations.Length; i++)
		{
			UnEquip(i);
		}
	}
}
