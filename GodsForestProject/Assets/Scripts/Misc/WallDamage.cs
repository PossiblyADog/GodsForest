using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDamage : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BasicEnemy")
        {
            GameObject currentHit = collision.gameObject;
            if (currentHit.GetComponent<AbstractEnemyBase>() != null)
            {
                if (currentHit.GetComponentInChildren<Rigidbody2D>().velocity.magnitude > 5)
                {
                    Debug.Log((int)currentHit.GetComponent<Rigidbody2D>().velocity.magnitude - 5);
                    currentHit.GetComponent<AbstractEnemyBase>().EnemyTakeDamage((int)(currentHit.GetComponent<Rigidbody2D>().velocity.magnitude - 5) * 5, true);
                }
            }
        }
    }
}
