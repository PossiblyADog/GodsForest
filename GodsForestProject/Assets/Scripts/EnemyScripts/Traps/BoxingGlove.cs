using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGlove : MonoBehaviour
{
    public int damage;
    public float knock;
    bool hasDamaged = false;

    public void SetGlove(Vector2 homePos)
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, 1.75f);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.layer == 3)
            {
                Debug.Log(hits[i].ClosestPoint(transform.position));
                transform.position = hits[i].ClosestPoint(transform.position);
                //transform.position = Physics2D.Raycast(transform.position, (Vector2)transform.position - Physics2D.ClosestPoint(transform.position, hits[i]), 4.0f, 3).point;
                break;
            }
        }

        Vector2 direction = homePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if (collision.gameObject.layer == 6 && !hasDamaged)
            {
                hasDamaged = true;
                PlayerController.instance.TakeDamage(damage);
                PlayerController.instance.GetKnocked((PlayerController.instance.transform.position - transform.position).normalized, knock);
            }
            else if (collision.gameObject.layer == 8)
            {
                GetComponent<AbstractEnemyBase>().EnemyTakeDamage(damage + 5, false);
                GetComponent<AbstractEnemyBase>().EnemyGetKnocked(knock, (collision.transform.position - transform.position).normalized);
            }
        }
        catch
        {

        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
