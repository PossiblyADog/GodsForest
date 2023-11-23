using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : AbstractDestructable
{
    public GameObject explodeCollider;
    private void Start()
    {
        hp = 30;
        deathTimer = 1f;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 3)
        {
            collision.gameObject.GetComponent<AbstractEnemyBase>().EnemyTakeDamage(30, false);
        }
        else if(collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(20);           
        }
        else
        {

        }


    }*/

    public override void DeathSequence()
    {
        base.DeathSequence();
        StartCoroutine(EnableCollider());
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(.65f);

        transform.localScale *= 2.0f;
        Collider2D[] nearbyChars = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 2.0f);
        for (int i = 0; i < nearbyChars.Length; i++)
        {
            if (nearbyChars[i].gameObject.layer == 6)
            {
                PlayerController.instance.TakeDamage(20);
                
            }
            else if(nearbyChars[i].gameObject.layer == 3)
            {
                try
                {
                    nearbyChars[i].gameObject.GetComponent<AbstractEnemyBase>().EnemyTakeDamage(30, false);
                }
                catch
                {

                }
            }
            else
            {

            }
        }
        explodeCollider.SetActive(true);

        yield return new WaitForSeconds(.25f);
        explodeCollider.SetActive(false);
    }
}
