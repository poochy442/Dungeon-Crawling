using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float _duration, _speed, _explosionRange;
	public Animator _animator;

	float _timer;
	LayerMask _enemyMask, _wallMask;
	bool exploding = false;

	void Start()
	{
		_timer = 0f;
		_enemyMask = LayerMask.GetMask("Enemy");
		_wallMask = LayerMask.GetMask("Wall");
	}

	void Update()
	{
		if(exploding)
			return;
		
		Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 1, _enemyMask);
		Collider[] hitWalls = Physics.OverlapSphere(transform.position, 1, _wallMask);

		if(hitEnemies.Length > 0 || hitWalls.Length > 0)
		{
			Explode();
		}

		transform.position += transform.forward * _speed * Time.deltaTime;
		_timer += Time.deltaTime;

		if(_timer >= _duration)
		{
			Explode();
		}
	}

	void Explode()
	{
		_animator.SetTrigger("Explode");
		exploding = true;

		// Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, _explosionRange, _enemyMask);

        // Damage enemies
        foreach (Collider enemy in hitEnemies)
        {
			float damageToDo = PlayerManager.instance.playerStats.spellDamage.GetValue();
            enemy.GetComponent<Enemy>().TakeDamage(damageToDo);
        }

		GameObject.Destroy(gameObject, 0.25f);
		GetComponent<Collider>().enabled = false;
		this.enabled = false;
	}
}
