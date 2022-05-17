using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
	public Image icon;
	public Button removeButton;
    Item item;

	public void AddItem(Item newItem)
	{
		item = newItem;

		icon = gameObject.transform.GetChild(0).GetComponentInChildren<Image>(true);
		removeButton = gameObject.transform.GetChild(1).GetComponent<Button>();
		gameObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(useItem);

		icon.sprite = item.icon;
		icon.enabled = true;
		removeButton.interactable = true;
	}

	public void ClearSlot()
	{
		item = null;
		
		icon.sprite = null;
		icon.enabled = false;
		removeButton.interactable = false;
	}

	public void onRemoveButton()
	{
		Inventory.instance.Remove(item);
	}

	public void useItem()
	{
		if(item != null)
		{
			item.Use();
		}
	}
}
