using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    # region Singleton

	public static PlayerManager instance { get; private set; }

	void Awake ()
	{
		if(instance == null)
			instance = this;
	}

	# endregion

	public LayerMask lootMask, playerMask, enemyMask;
	public GameObject player;
	public Interactable currentTarget;
	public PlayerStats playerStats;

	float _pickupRadius = 3f;
	IEnumerator _lootTextCoroutine;

	void Update ()
	{
		if(player == null) return;
		
		// Check for loot in the area and allow the player to pick it up
		Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, _pickupRadius, lootMask);
		GameObject closest = null;
		foreach(Collider hit in hitColliders)
		{
			closest = hit.gameObject;
		}

		if(closest != null)
		{
			currentTarget = closest.GetComponent<ItemPickup>();

			// TODO: Change to loot name
			Text text = closest.GetComponentInChildren<Text>();
			text.text = currentTarget.name;

			if(_lootTextCoroutine != null) StopCoroutine(_lootTextCoroutine);

			_lootTextCoroutine = ResetLootText(text, 1f);
			StartCoroutine(_lootTextCoroutine);
		}
	}

	public void MovePlayer(Vector3 location)
	{
		if(player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			playerStats = player.GetComponent<PlayerStats>();
		}
		player.transform.position = location;
	}

	public void SetPlayer(GameObject obj)
	{
		player = obj;
		playerStats = player.GetComponent<PlayerStats>();
	}

	IEnumerator ResetLootText(Text text, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if(text != null)
			text.text = "";
	}

}
