using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f, attackDamge = 4f, attackRate = 2f;
    public LayerMask enemyLayers;

    private Animator _animator;
    private float nextAttackTime = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextAttackTime){
            if(Input.GetKeyDown(KeyCode.Space)){
                // Play animation, which will trigger the attack function
                _animator.SetTrigger("Attack");
                nextAttackTime = Time.time + (1f / attackRate);
            }
        }
    }

    void Attack()
    {
        // Detect enemies in range of attack
        // TODO: Delay detection for animation trigger
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamge);
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
