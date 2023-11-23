using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFireTrap : MonoBehaviour
{
    public int damage;
    public float knock, burstDelay;

    private void Start()
    {
        transform.position += new Vector3(.4f, 0, 0);
        FireBurst();
    }

    private void FireBurst()
    {
        GetComponent<Animator>().SetTrigger("isFire");
        StartCoroutine(ResetAnim());
    }

    IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(burstDelay);
        GetComponent<Animator>().SetTrigger("isReset");
        yield return new WaitForSeconds(.5f);
        FireBurst();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if (collision.gameObject.layer == 6)
            {
                PlayerController.instance.TakeDamage(damage);
                PlayerController.instance.GetKnocked((PlayerController.instance.transform.position - transform.position).normalized, knock);
            }
            else if (collision.gameObject.layer == 8)
            {
                GetComponent<AbstractEnemyBase>().EnemyTakeDamage(damage + 5, false);
                GetComponent<AbstractEnemyBase>().EnemyGetKnocked(knock*3, (collision.transform.position - transform.position).normalized);
            }
        }
        catch
        {

        }
    }

}
