using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerBomb : MonoBehaviour
{
    float shotsConsumed = 0;
    public GameObject explodeEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            Destroy(collision.gameObject);
            shotsConsumed++;
        }
    }


    public void SetBomba(float duration)
    {
        StartCoroutine(Bomba(duration));
    }
    private IEnumerator Bomba(float duration)
    {
        yield return new WaitForSeconds(duration);
        GetComponent<Animator>().SetTrigger("isExplode");
        SetExplosion();

    }

    private void SetExplosion()
    {
        var explosion = Instantiate(explodeEffect, transform.position, Quaternion.identity);
        explosion.transform.localScale += new Vector3(1.5f,1.5f,0) * (1.0f + (shotsConsumed / 8.0f));
        int damMod = Mathf.RoundToInt((8.0f + PlayerStateManager.playerManager.damageFlatModifier) * (shotsConsumed / 8.0f));
        explosion.GetComponent<TimeBomb>().SetBombDamage(8 + damMod);
        explosion.GetComponent<TimeBomb>().NoScaleExplode();
        Debug.Log(damMod);
        Destroy(gameObject, .35f);
        Destroy(explosion, 1.0f);
    }
}
