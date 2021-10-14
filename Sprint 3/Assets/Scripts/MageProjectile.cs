using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageProjectile : MonoBehaviour
{
    public float moveSpeed = 0f;        // Movespeed of projectile
    public int damage;                  // Damage of projectile

    private BoxCollider2D boxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    public Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        this.damage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
        if (Mathf.Abs(transform.position.x) > 15 + Mathf.Abs(initialPos.x))
        {
            Destroy(this.gameObject);
            Debug.Log(this.gameObject.name + " went out of bounds");
        }
    }

    public void Direction(bool dir)
    {
        moveSpeed = (dir) ? 3f : -3f;
        if(dir)
        {
            this.transform.localScale = Vector3.one;
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "NoEnemy": break;

            case "Mage": break;

            case "Knight": break;

            case "Spearmen": break;

            case "Player":
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().TakeDamage(damage);
                Destroy(this.gameObject);
                break;

            default:
                Debug.Log(collision.name);
                Destroy(this.gameObject);
                break;
        }
    }
}