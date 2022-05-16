using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    float _currentHealth;
    Animator _animator;
	CharacterStats _stats;

	public float CurrentHealth { get { return _currentHealth; } }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
		_stats = GetComponent<CharacterStats>();
    }

    public void TakeDamage(float damage)
    {
        // Take damage
        _currentHealth -= Mathf.Max(damage - _stats.armor.GetValue(), 0);

        // Play hurt animation
        _animator.SetTrigger("Hurt");

        // Check if dead
        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
		Debug.Log("Enemy died");
		
        // Die animation
        _animator.SetBool("IsDead", true);

        // Disable the enemy
        Destroy(gameObject, 4f);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
		GetComponent<EnemyController>().enabled = false;
        this.enabled = false;
    }
}
