using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceNova : MonoBehaviour
{
    int damage;
    float knock, maxRad, radInc, sfxDur, specMag;
    public CircleCollider2D effectRadius;
    public AudioClip hitSFX;
    public GameObject specialEffect;
    public void SetNovaParams(int damageToDeal, float knockback, float desiredRadius, float growRate, float specialDuration, float specMagnitude)
    {
        damage = damageToDeal;
        knock = knockback;
        maxRad = desiredRadius;
        radInc = growRate;
        sfxDur = specialDuration;
        specMag = specMagnitude;

        StartCoroutine(PerformNova());
    }

    private IEnumerator PerformNova()
    {
        if(effectRadius.radius < maxRad)
        {
            effectRadius.radius += radInc;
            yield return null;
            StartCoroutine(PerformNova());
        }
        else
        {
            Destroy(gameObject, .05f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8 || collision.gameObject.layer == 12)
        {
            if(hitSFX != null) 
            { AudioSource.PlayClipAtPoint(hitSFX, collision.transform.position); }

            try
            {
                collision.GetComponent<AbstractEnemyBase>().EnemyTakeDamage(damage, false);
                collision.GetComponent<AbstractEnemyBase>().EnemyGetKnocked(knock, collision.transform.position - transform.position);
                if(specialEffect != null)
                {
                    StartCoroutine(collision.GetComponentInChildren<EffectHandler>().ApplyMovespeedEffect(specialEffect, sfxDur, -specMag));
                }

            }
            catch
            {

            }
        }
        else if(collision.gameObject.layer == 11)
        {
            collision.GetComponent<AbstractDestructable>().TakeDamage(damage);
        }
    }
}
