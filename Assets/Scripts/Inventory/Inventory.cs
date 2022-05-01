using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	#region Singleton
    public static Inventory instance;

	void Awake ()
	{
		if(instance != null)
		{
			Debug.LogWarning("More that one instance of Inventory!");
			return;
		}
		instance = this;
	}
	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public List<Item> items = new List<Item>();

	public void Add (Item i)
	{
		if(i.isDefaultItem) return;

		items.Add(i);

		if(onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	public void Remove (Item i)
	{
		items.Remove(i);
		
		if(onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}
}
