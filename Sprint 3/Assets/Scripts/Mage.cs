using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    public GameObject player;               // the player
    private float playerX;                  // player x coord
    private float mageX;                    // mage x coord
    private float playerY;                  // player x coord
    private float mageY;                    // mage x coord

    private Rigidbody2D physics;            // holds the mage's rigidbody for movement
    public bool facingLeft = true;          // is the mage facing left? based on initial spritesheet
    public int health = 4;                  // health of the mage

    public GameObject attackPrefab;         // holds the prefab for the mage's attack
    public float attackRate;                // time between attacks
    private float nextAttack;               // time of next attack

    private Vector2 speed;                  // how fast the mage can go
    private float moveRate = 1f;            // how long the mage will move
    private float moveCounter = 0f;         // how long the mage has moved
    public int dirX;                        // direction of movement
    public bool seesPlayer = false;         // did the mage see the player?

    // Animation booleans
    public Animator anim;
    public bool idle;
    public bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(11, 11);

        player = GameObject.FindGameObjectWithTag("Player");

        this.idle = true;
        this.attacking = false;

        attackRate = 2f;
        this.nextAttack = 0f;

        // grabbing the rigidbody component for movement
        this.physics = this.gameObject.GetComponent<Rigidbody2D>();
        this.anim = this.gameObject.GetComponent<Animator>();
        this.speed = new Vector2(1.25f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.seesPlayer)
        {
            if (this.moveCounter > this.moveRate)
            {
                this.ChangeDirection();
                this.moveCounter = 0f;
            }

            switch (this.dirX)
            {
                case 1:
                    this.idle = false;
                    anim.SetBool("isIdle", this.idle);
                    this.transform.localScale = new Vector3(-1, 1, 1);
                    this.facingLeft = false;
                    break;

                case -1:
                    this.idle = false;
                    anim.SetBool("isIdle", this.idle);
                    this.transform.localScale = Vector3.one;
                    this.facingLeft = true;
                    break;

                case 0:
                    this.idle = true;
                    anim.SetBool("isIdle", this.idle);
                    break;
            }

            if (Mathf.Abs(this.physics.velocity.x) < speed.x)
            {
                this.physics.AddForce(speed * dirX);
            }
            else
            {
                this.physics.velocity = speed * dirX;
            }

            moveCounter += Time.deltaTime;

            FindPlayer();
        }
        else
        {
            this.idle = true;
            anim.SetBool("isIdle", this.idle);

            try
            {
                playerX = player.transform.position.x;
                this.mageX = this.transform.position.x;

                if (playerX > mageX)
                {
                    this.transform.localScale = new Vector3(-1, 1, 1);
                    this.facingLeft = false;
                }
                else if (playerX < mageX)
                {
                    this.transform.localScale = Vector3.one;
                    this.facingLeft = true;
                }
            }
            catch { }

            if (Time.time > nextAttack)
            {
                this.attacking = true;
                this.anim.SetBool("isAttacking", this.attacking);
                this.nextAttack = Time.time + this.attackRate;

                float xOffset = (!facingLeft) ? 0.3f : -0.3f;

                GameObject projectile = Instantiate(attackPrefab, new Vector3(transform.position.x + xOffset, transform.position.y + .25f, 0), Quaternion.identity);

                projectile.GetComponent<MageProjectile>().Direction(facingLeft);
            }
            else
            {
                this.attacking = false;
                this.anim.SetBool("isAttacking", this.attacking);
            }

        }
    }

    // Randomly generates the direction of movement
    private void ChangeDirection()
    {
        this.dirX = Random.Range(-1, 2);
    }

    // Looks for the player
    private void FindPlayer()
    {
        playerX = player.transform.position.x;
        playerY = player.transform.position.y;
        mageX = this.transform.position.x;
        mageY = this.transform.position.y;

        if (Mathf.Abs(playerY - mageY) < 2)
        {
            if (playerX > mageX && !this.facingLeft)
            {
                if (playerX - mageX <= 5f)
                {
                    seesPlayer = true;
                    Debug.Log("I SAW THE PLAYER");
                    return;
                }
            }
            else if (playerX < mageX && facingLeft)
            {
                if (Mathf.Abs(playerX - mageX) <= 5f)
                {
                    seesPlayer = true;
                    Debug.Log("I SAW THE PLAYER");
                    return;
                }
            }
        }
    }

    // Allows the mage to take damage and die
    public void TakeDamage(int damage)
    {
        this.health -= damage;

        if (this.health < 1)
        {
            Destroy(this.gameObject);
        }
    }
}
