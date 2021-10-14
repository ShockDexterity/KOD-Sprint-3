using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    public Animator animator;           // Player animation control
    public float attackRate = 0.6f;     // Time between attacks
    private float nextAttack;           // Time of next attack
    private int damage = 2;             // Melee attack damage
    public Transform attackPoint;       // Where the attack range is centered
    public float attackRange = 0.5f;    // Range of the attack
    public LayerMask enemyLayers;       // What layer(s) the enemies are on

    // Start is called before the first frame update
    private void Start()
    {
        animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }//end Start()

    // Update is called once per frame
    void Update()
    {
        // Check if user can attack
        if (Input.GetMouseButtonDown(0) && (Time.time > nextAttack))
        {
            nextAttack = Time.time + attackRate;
            Attack();
        }
    }//end Update()

    // Attack control
    void Attack()
    {
        // Set melee animation
        animator.SetTrigger("isAttacking");

        // Gathering all enemies we hit, if any
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Checking who we hit
        foreach (Collider2D enemy in hitEnemies)
        {
            //FIXME: UNCOMMENT THIS WHEN MAGE AND KNIGHT ARE IMPLEMENTED
            //Mage mage = enemy.GetComponent<Mage>();
            //Knight knight = enemy.GetComponent<Knight>();

            //if (mage != null)
            //{
            //    mage.TakeDamage(damage);
            //}
            //else if (knight != null)
            //{
            //    knight.TakeDamage(damage);
            //}
        }// foreach
    }//end Attack()

    // Allows the range to be seen
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }//end OnDrawGizmosSelected()
}//end PlayerMeleeAttack