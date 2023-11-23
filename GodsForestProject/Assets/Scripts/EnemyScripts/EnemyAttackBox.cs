using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackBox : MonoBehaviour //Class to be attatched to static* enemy attacks sends damage and force info to player,
                                            //decides whether or not the hitbox is destroyed in the exchange
{
    [SerializeField]
    int damageToDeal;

    [SerializeField]
    bool knockbackPlayer = false, destroyOnHit = false;

    [SerializeField]
    float knockbackForce;

    public Collider2D attackCollider;
    private bool movingBox = false;
    private Vector2 moveVector;
    public Collider2D parentBody;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hit, Current Damage: " + damageToDeal);
            PlayerController.instance.TakeDamage(damageToDeal);

            try
            {
                if (knockbackPlayer)
                {
                    var direction = (((Vector2)collision.transform.position - parentBody.ClosestPoint(collision.transform.position)).normalized);
                    collision.gameObject.GetComponent<PlayerController>().GetKnocked(direction, knockbackForce);
                    //Debug.Log("Knockback Attempted");
                }
            }
            catch
            {

            }

            if(destroyOnHit)
            {
                Destroy(attackCollider);
            }

           
        }
    }

    public void SetBoxMovement(float speedX, float speedY)
    {
        movingBox = true;
        moveVector = new Vector2(speedX, speedY);

    }


    private void Update()
    {
        if(movingBox == true)
        {
            transform.position += new Vector3(moveVector.x, moveVector.y);
        }
    }

    public void ResetPosition(Vector3 startPos)
    {
        movingBox = false;
        transform.position = startPos;
    }

    public void SetAttackParams(float knockForce, int damage, float destroyTime, bool destroyOnHit, float delayActivation)
    {
        
        damageToDeal = damage;
        this.destroyOnHit = destroyOnHit;

        if(knockForce > 0.0f)
        {
            knockbackPlayer = true;
            knockbackForce = knockForce;
        }

        if(delayActivation != 0.0f)
        {
            StartCoroutine(ActivateLate(delayActivation));
        }

        if(destroyTime != 0.0f)
        {
            Destroy(gameObject, destroyTime);
        }



    }

    private IEnumerator ActivateLate(float delayActivation)
    {
        yield return new WaitForSeconds(delayActivation);
        gameObject.GetComponent<Collider2D>().enabled = true;

    }

}
