using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : MonoBehaviour
{
    public Collider2D explosionCollider;
    private float timer;
    public int bombDamage, bombKnock;
    public AudioClip bombSound;
    public int type = 0;
    public bool armorPiercing = false;


    public void SetBombData(float timeToExplode)
    {
        timer = timeToExplode;
        StartCoroutine(Explode());
    }

    public void SetBombDamage(int damage)
    {
        bombDamage = damage;
        bombKnock = damage + (damage / 2);
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(timer);
        AudioSource.PlayClipAtPoint(bombSound, transform.position, GameManager.instance.sfxVolume);
        if (GetComponent<Animator>().parameters.Length > 0)
        { GetComponent<Animator>().SetTrigger("isExplode"); }
        transform.localScale *= 1.75f;
        Destroy(gameObject, 1.0f);
    }

    public IEnumerator NoScaleExplode()
    {
        yield return new WaitForSeconds(timer);
        AudioSource.PlayClipAtPoint(bombSound, transform.position, GameManager.instance.sfxVolume);
        if (GetComponent<Animator>().parameters.Length > 0)
        { GetComponent<Animator>().SetTrigger("isExplode"); }
        Destroy(gameObject, 1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == 0)
        {
            if (collision.gameObject.layer == 6)
            {
                PlayerController.instance.TakeDamage(bombDamage);
                PlayerController.instance.GetKnocked((collision.transform.position - transform.position).normalized, bombKnock);
            }
            else if (collision.gameObject.layer == 8)
            {
                if (!collision.gameObject.name.Contains("Metallic"))
                {
                    collision.gameObject.GetComponent<AbstractEnemyBase>().EnemyGetKnocked(bombKnock, (collision.transform.position - transform.position).normalized);
                }
            }
        }
        else if(type == 1)
        {
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 12)
            {
                collision.GetComponentInChildren<AbstractEnemyBase>().EnemyTakeDamage(bombDamage, armorPiercing);
                collision.GetComponentInChildren<AbstractEnemyBase>().EnemyGetKnocked(bombKnock, (collision.transform.position - transform.position).normalized);
            }
            else if(collision.gameObject.layer == 11)
            {
                collision.GetComponent<AbstractDestructable>().TakeDamage(bombDamage);
            }
        }
    }
}
