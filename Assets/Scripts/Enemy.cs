using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        _animator = gameObject.GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        // Take damage
        currentHealth -= damage;

        // Play hurt animation
        _animator.SetTrigger("Hurt");

        // Check if dead
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Die animation
        _animator.SetBool("IsDead", true);

        // Disable the enemy
        Destroy(gameObject, 4f);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        this.enabled = false;
    }
}
