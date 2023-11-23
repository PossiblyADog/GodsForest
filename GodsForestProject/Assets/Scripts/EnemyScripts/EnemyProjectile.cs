using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damageToDeal;
    public float knockbackForce, travelSpeed, splitVal;
    private Rigidbody2D projBody;
    private Vector2 targetPosition;
    private bool wallOnly = false;
    private Animator animator;
    public bool canBounce = false, armorPiercing;

    public AudioClip hitSfx;


    void Awake()
    {
        projBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void SetBulletParams(float speed, int damage, float knockForce, Vector2 targetPos, float sprayRange, float degree, bool bounces)
    {
        transform.GetComponent<Collider2D>().enabled = true;
        travelSpeed = speed;
        damageToDeal = damage;
        targetPosition = targetPos.normalized;
        splitVal = degree;
        if(knockForce > 0)
        {
            knockbackForce = knockForce;
        }

        if(sprayRange != 0)
        {
            targetPosition.x += Random.Range(-sprayRange, sprayRange);
            targetPosition.y += Random.Range(-sprayRange, sprayRange);
        }
        
        canBounce = bounces;

        Fire(targetPosition);


    }


    /*private void Update()
    {
        if (projBody != null)
        {
            transform.right = projBody.velocity;
        }
    }*/

    private void Fire(Vector2 direction)
    {
        //Debug.Log();
        if (direction.magnitude != 0)
        { projBody.AddForce(direction * travelSpeed, ForceMode2D.Impulse); }
        transform.right = projBody.velocity;
        if (splitVal != 0)
        {
            projBody.AddForce(transform.up * splitVal, ForceMode2D.Impulse);
            transform.right = projBody.velocity;
        }


    }

    public void WallDestroyOnly()
    {
        wallOnly = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.gameObject.tag == "Player")
        {

            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damageToDeal);


            if (knockbackForce > 0)
            {
                if (projBody != null)
                {
                    collision.gameObject.GetComponent<PlayerController>().GetKnocked(projBody.velocity.normalized, knockbackForce);
                    gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                else
                {
                    collision.gameObject.GetComponent<PlayerController>().GetKnocked(transform.parent.parent.GetComponent<Rigidbody2D>().velocity.normalized, knockbackForce);
                    transform.SetParent(null);
                }

            }

            


            if(hitSfx != null)
            {AudioSource.PlayClipAtPoint(hitSfx, transform.position, GameManager.instance.sfxVolume); }

            if (!wallOnly)
            {
                if (animator != null) 
                { animator.SetTrigger("isHit"); }

                transform.GetComponent<Collider2D>().enabled = false;

                if (canBounce)
                {
                    GetComponent<BounceBullet>().Bounce();
                }
                Destroy(this.gameObject, .35f); 
            }
        }

        else if (collision.gameObject.layer == 3)//3 is current Layer number for 'Obstacles' like walls and rocks(2^3)
        {
            if (projBody != null)
            { gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; }

            if (animator != null)            
            { animator.SetTrigger("isHit"); }

            
            transform.GetComponent<Collider2D>().enabled = false;

            if (canBounce)
            {
                GetComponent<BounceBullet>().Bounce();
            }
            
            Destroy(this.gameObject, .35f);
        }
        else
        {

        }

    }
}
