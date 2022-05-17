using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public bool isBoss = false;
	public float experienceReward = 10f;
	public GameObject lootObject;
    Animator _animator;
	CharacterStats _stats;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
		_stats = GetComponent<CharacterStats>();
    }

    public void TakeDamage(float damage)
    {
        // Take damage
        _stats.TakeDamage(damage);

        // Play hurt animation
        _animator.SetTrigger("Hurt");

        // Check if dead
        if(_stats.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Die animation
        _animator.SetBool("IsDead", true);

		PlayerManager.instance.playerStats.GainExperience(experienceReward);

		if(Random.Range(0f, 1f) > 0.8f)
		{
			Mutation newMutation = Mutation.GenerateMutation();
			GameObject loot = GameObject.Instantiate(lootObject, transform.position, Quaternion.identity);
			loot.GetComponent<ItemPickup>().item = newMutation;
		}

		if(isBoss)
		{
			GameManager.instance.Win();
		}

        // Disable the enemy
        Destroy(gameObject, 4f);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
		GetComponent<EnemyController>().enabled = false;
        this.enabled = false;
    }
}
