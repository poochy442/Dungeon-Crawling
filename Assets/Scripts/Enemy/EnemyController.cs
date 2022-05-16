using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public float lookRadius = 10f, rotationSpeed = 5f, attackAngle = 35f, attackSpeed = 1f, attackRange = 1f;

	Transform _target;
	NavMeshAgent _agent;
	CharacterStats _stats;
	Animator _animator;
	float _nextAttackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
		_target = PlayerManager.instance.player.transform;
        _agent = GetComponent<NavMeshAgent>();
		_stats = GetComponent<CharacterStats>();
		_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(_target.position, transform.position);
		Vector3 targetPosition = _target.position - (_target.position - transform.position).normalized * 0.5f;

		if(distance <= lookRadius)
		{
			if(distance <= _agent.stoppingDistance)
			{
				_agent.isStopped = true;
				bool isFacingTarget = FaceTarget();

				// Attack target
				if(Time.time > _nextAttackTime && isFacingTarget)
				{
					Attack();
				}
			} else {
				_agent.isStopped = false;
				_agent.SetDestination(targetPosition);
			}
		} else
		{
			_agent.isStopped = true;
		}

    }

	bool FaceTarget ()
	{
		Vector3 direction = (_target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

		if(Quaternion.Angle(transform.rotation, lookRotation) < attackAngle)
			return true;
		else
			return false;
	}

	void Attack ()
	{
		_animator.SetTrigger("Attack");
		float attackTime = 1f / attackSpeed;
		_nextAttackTime = Time.time + attackTime;
		StartCoroutine(DoDamage(attackTime * 0.75f));
	}

	IEnumerator DoDamage (float delay)
	{
		yield return new WaitForSeconds(delay);
		// Detect enemies in range of attack
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position + (transform.forward * 0.5f), attackRange, LayerMask.GetMask("Player"));

		if(hitPlayers.Length > 0)
		{
			PlayerManager.instance.playerStats.TakeDamage(_stats.damage.GetValue());
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}
