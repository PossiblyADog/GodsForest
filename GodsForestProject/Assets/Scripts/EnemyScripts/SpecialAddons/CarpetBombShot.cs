using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarpetBombShot : MonoBehaviour
{
    public GameObject bombEffect;
    
    public IEnumerator StartBombing(float frequency)
    {
        var bomb = Instantiate(bombEffect, transform.position, Quaternion.identity);
        bomb.GetComponent<TimeBomb>().SetBombDamage(5 + PlayerStateManager.playerManager.damageFlatModifier);
        bomb.GetComponent<TimeBomb>().SetBombData(.01f);
        yield return new WaitForSeconds(frequency); 
        StartCoroutine(StartBombing(frequency));
    }

    public void StartPlayerBombing(float frequency)
    {
        StartCoroutine(StartBombing(frequency));
    }

}
