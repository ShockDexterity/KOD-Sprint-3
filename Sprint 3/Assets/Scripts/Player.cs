using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D physics;    // Allows for collisions with movement
    private float speed;            // Speed of player
    public float jumpForce;         // Force of player jump
    private bool jumping;           // Are they jumping?

    private int maxHealth = 10;     // max health
    private int health;             // current health

    private Animator animator;      // Animator of the player
    private bool idle;              // Is the player idle?
    private bool blocking;          // Is the player currently blocking?

    // Start is called before the first frame update
    void Start()
    {
        physics = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        // Setting default values
        speed = 2.5f;
        jumpForce = 6f;
        jumping = false;
        idle = true;
        health = maxHealth;

        // The player is able to pass through what the enemies can't
        Physics2D.IgnoreLayerCollision(11, 12);
        //Physics2D.IgnoreLayerCollision(11, 10);
    }// end Start()

    // Update is called once per frame
    void Update()
    {
        // Checking for pitfall trap
        // FIXME: UPDATE THIS FOR A COLLISION TRIGGER RATHER THAN A POSITION CHECK
        if (this.transform.position.y < -5f)
        {
            TakeDamage(1);
        }//end if

        // We don't want to double/triple/infinitely jump
        if (!jumping)
        {
            // Multiple control options
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                // Player jumps
                physics.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                idle = false;
                jumping = true;
                animator.SetBool("isIdle", idle);
                animator.SetBool("isJumping", jumping);
            }//end if
        }//end if

        // Player can move while in the air
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            // Don't want to change out of the jump animation
            if (!jumping)
            {
                idle = false;
                animator.SetBool("isIdle", this.idle);
            }//end if

            // Change player direction
            this.transform.localScale = new Vector3(1, 1, 1);

            // Set new horizontal velocity while keeping vertical velocity
            physics.velocity = new Vector2(speed, physics.velocity.y);
        }//end if

        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            // Don't want to change out of the jump animation
            if (!jumping)
            {
                idle = false;
                animator.SetBool("isIdle", this.idle);
            }//end if

            // Change player direction
            this.transform.localScale = new Vector3(-1, 1, 1);

            // Set new horizontal velocity while keeping vertical velocity
            physics.velocity = new Vector2(-speed, physics.velocity.y);
        }//end else if

        else
        {
            // Don't want to change from jumping animation
            if (!jumping)
            {
                idle = true;
                animator.SetBool("isIdle", this.idle);
            }//end if
        }//end else
    }//end Update()

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // No longer jumping
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumping = false;
            animator.SetBool("isJumping", jumping);
        }//end if
    }//end OnCollisionEnter2D()

    // Player can take damage and die
    public void TakeDamage(int outsideDamage)
    {
        // Only take damage when not blocking
        if (!blocking)
        {
            health -= outsideDamage;
        }//end if

        //FIXME: RESPAWN INSTEAD OF DESTROYING
        if (health < 1)
        {
            Destroy(this.gameObject);
        }//end if
    }//end TakeDamage()
}//end Player