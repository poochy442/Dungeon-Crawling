using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	public float currentHealth {get; protected set;}

	public Stat maxHealth;
    public Stat damage;
	public Stat armor;

	void Start()
	{
		currentHealth = maxHealth.GetValue();
	}

	public void TakeDamage (float damage)
	{
		damage = Mathf.Max(damage - armor.GetValue(), 0);

		currentHealth -= damage;
		Debug.Log(transform.name + " takes " + damage + " damage.");

		if(currentHealth <= 0)
		{
			Die();
		}
	}

	public virtual void Die ()
	{
		// Die in some way, meant to be overwritten
		Debug.Log(transform.name + " died.");
	}
}
