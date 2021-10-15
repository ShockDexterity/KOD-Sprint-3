using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    public GameObject player;           // The player
    private float playerX;              // Player x coord
    private float knightX;              // Knight x coord
    private float playerY;              // Player y coord
    private float knightY;              // Knight y coord

    private Rigidbody2D physics;        // Holds the knight's rigidbody for movement
    public bool facingLeft;             // Is the mage facing left? based on initial spritesheet
    private int health = 8;             // Health of the knight

    public float attackRate;            // Time between attacks
    private float nextAttack;           // Time of next attack
    private int damage;                 // Damage of attack
    public Transform attackPoint;       // Center of attack range
    public float attackRange;           // Range of attack
    public LayerMask playerLayer;       // The layer the player is on

    private Vector2 speed;              // How fast the mage can go
    private float moveRate = 1f;        // How long the mage will move
    private float moveCounter = 0f;     // How long the mage has moved
    public int dirX;                    // Direction of movement
    public bool seesPlayer;             // Did the mage see the player?

    public Animator animator;           // Knight animation control
    public bool idle;                   // Is the knight idle?

    public BoxCollider2D boxCollider2D;
    public Vector3 lootSpawnPoint;
    private float bottomOfKnight;
    public GameObject lootPrefab;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = this.GetComponent<BoxCollider2D>();
        bottomOfKnight = boxCollider2D.size.y / 2f;

        // Enemies won't collide
        Physics2D.IgnoreLayerCollision(10, 10);

        // Setting defaults
        this.idle = true;
        this.damage = 2;
        this.facingLeft = false;
        this.attackRate = 2f;
        this.attackRange = 0.5f;
        this.seesPlayer = false;
        speed = new Vector2(1.5f, 0f);

        // Grabbing components
        this.player = GameObject.FindGameObjectWithTag("Player");
        this.physics = this.GetComponent<Rigidbody2D>();
        this.attackPoint = GameObject.Find("knightAttackPoint").GetComponent<Transform>();
        this.animator = this.GetComponent<Animator>();
    }//end Start()

    // Update is called once per frame
    void Update()
    {
        // If the knight doesn't see the player
        if (!this.seesPlayer)
        {
            // Update movement
            if (this.moveCounter > this.moveRate)
            {
                this.ChangeDirection();
                this.moveCounter = 0f;
            }

            // Which way is the knight facing?
            switch (this.dirX)
            {
                case 1:
                    this.idle = false;
                    animator.SetBool("isIdle", this.idle);
                    this.transform.localScale = Vector3.one;
                    this.facingLeft = false;
                    break;

                case -1:
                    this.idle = false;
                    animator.SetBool("isIdle", this.idle);
                    this.transform.localScale = new Vector3(-1, 1, 1);
                    this.facingLeft = true;
                    break;

                case 0:
                    this.idle = true;
                    animator.SetBool("isIdle", this.idle);
                    break;
            }

            // Move the knight
            this.physics.velocity = speed * dirX;

            moveCounter += Time.deltaTime;

            // Look for player
            FindPlayer();
        }
        else // After the knight sees the player
        {

            // Look for player and move towards them
            playerX = player.transform.position.x;
            knightX = this.transform.position.x;
            if (playerX > knightX)
            {
                this.transform.localScale = Vector3.one;
                this.facingLeft = false;
                dirX = 1;
            }
            else if (playerX < knightX)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);
                this.facingLeft = true;
                dirX = -1;
            }
            this.physics.velocity = speed * dirX;

            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                Attack();
            }
        }
    }//end Update()

    // Randomly chooses movement direction
    private void ChangeDirection()
    {
        // (inclusive, exclusive)
        this.dirX = Random.Range(-1, 2);
    }

    // Look for player location
    private void FindPlayer()
    {
        this.playerX = player.transform.position.x;
        this.playerY = player.transform.position.y;
        this.knightX = this.transform.position.x;
        this.knightY = this.transform.position.y;

        if (Mathf.Abs(this.playerY - this.knightY) < 2)
        {
            if (this.playerX > this.knightX && !this.facingLeft)
            {
                if (this.playerX - this.knightX <= 5f)
                {
                    this.seesPlayer = true;
                    // No longer idle
                    animator.SetBool("isIdle", false);
                    return;
                }//end if
            }//end if
            else if (this.playerX < this.knightX && this.facingLeft)
            {
                if (Mathf.Abs(this.playerX - this.knightX) <= 5f)
                {
                    this.seesPlayer = true;
                    // No longer idle
                    animator.SetBool("isIdle", false);
                    return;
                }//end if
            }//end else if
        }//end if
    }//end FindPlayer()

    // Knight can take damage and die
    public void TakeDamage(int outsideDamage)
    {
        this.health -= outsideDamage;
        if (this.health < 1)
        {
            DropLoot();
            Destroy(this.gameObject);
        }
    }

    private void DropLoot()
    {
        float xPos = this.transform.position.x;
        float yPos = this.transform.position.y;
        float zPos = this.transform.position.z;
        CircleCollider2D lootCollider = lootPrefab.GetComponent<CircleCollider2D>();
        float lootCenter = lootCollider.radius;

        Vector3 coinSpawnPoint = new Vector3(xPos, yPos - bottomOfKnight + lootCenter, zPos);

        Instantiate(lootPrefab, coinSpawnPoint, Quaternion.identity);
    }

    // Knight attack control
    void Attack()
    {
        this.animator.SetTrigger("isAttacking");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D player in hitEnemies)
        {
            player.GetComponent<Player>().TakeDamage(damage);
        }

    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    // }
}//end Knight