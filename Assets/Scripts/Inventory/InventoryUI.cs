using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform itemsParent;
	public GameObject inventoryUI;
	InventorySlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
		Inventory.instance.onItemChangedCallback += UpdateUI;

		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
		if(InputManager.instance != null) inventoryUI.SetActive(InputManager.instance.isInventoryActive);
		else inventoryUI.SetActive(false);
    }

	void UpdateUI()
	{
		for(int i = 0; i < slots.Length; i++)
		{
			if(i < Inventory.instance.items.Count)
			{
				slots[i].AddItem(Inventory.instance.items[i]);
			} else {
				slots[i].ClearSlot();
			}
		}
	}
}
