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
	public ItemPickup currentTarget;
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
			if(closest == null || (hit.gameObject.transform.position - transform.position).magnitude > (closest.transform.position - transform.position).magnitude)
				closest = hit.gameObject;
		}

		if(closest != null)
		{
			currentTarget = closest.GetComponent<ItemPickup>();

			Text text = closest.GetComponentInChildren<Text>();
			text.text = currentTarget.item.name;

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

	public void TakeDamage(float damage)
	{
		player.GetComponent<Animator>().SetTrigger("IsHurt");
		playerStats.TakeDamage(damage);
	}

	IEnumerator ResetLootText(Text text, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if(text != null)
			text.text = "";
	}

}
