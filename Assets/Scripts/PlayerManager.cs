using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    # region Singleton

	public static PlayerManager _instance;

	void Awake ()
	{
		_instance = this;
	}

	# endregion

	public LayerMask _lootMask, _playerMask, _enemyMask;
	public GameObject _player;
	public Interactable _currentTarget;
	public PlayerStats _playerStats;

	// TODO: Change to stat
	float _pickupRadius = 3f;
	IEnumerator _lootTextCoroutine;

	void Update ()
	{
		// Check for loot in the area and allow the player to pick it up
		Collider[] hitColliders = Physics.OverlapSphere(_player.transform.position, _pickupRadius, _lootMask);
		GameObject closest = null;
		foreach(Collider hit in hitColliders)
		{
			closest = hit.gameObject;
		}

		if(closest != null)
		{
			_currentTarget = closest.GetComponent<ItemPickup>();

			// TODO: Change to loot name
			Text text = closest.GetComponentInChildren<Text>();
			text.text = _currentTarget.name;

			if(_lootTextCoroutine != null) StopCoroutine(_lootTextCoroutine);

			_lootTextCoroutine = ResetLootText(text, 1f);
			StartCoroutine(_lootTextCoroutine);
		}
	}

	IEnumerator ResetLootText(Text text, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if(text != null)
			text.text = "";
	}

}
