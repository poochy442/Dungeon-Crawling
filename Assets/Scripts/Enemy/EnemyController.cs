using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
	public float lookRadius = 10f, rotationSpeed = 5f, attackAngle = 35f;

	Transform target;
	NavMeshAgent agent;
	CharacterStats stats;

    // Start is called before the first frame update
    void Start()
    {
		target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
		stats = GetComponent<CharacterStats>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
		Vector3 targetPosition = target.position - (target.position - transform.position).normalized * 0.5f;

		if(distance <= lookRadius)
		{
			if(distance <= agent.stoppingDistance)
			{
				agent.SetDestination(transform.position);

				bool isFacingTarget = FaceTarget();

				// Attack target
				if(isFacingTarget)
				{
					Attack();
				}
			} else {
				agent.SetDestination(targetPosition);
			}
		}

    }

	bool FaceTarget ()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

		if(Quaternion.Angle(transform.rotation, lookRotation) < attackAngle)
			return true;
		else
			return false;
	}

	void Attack ()
	{

	}

	IEnumerator DoDamage (float delay)
	{
		yield return new WaitForSeconds(delay);
		PlayerManager.instance.playerStats.TakeDamage(stats.damage.GetValue());
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}
