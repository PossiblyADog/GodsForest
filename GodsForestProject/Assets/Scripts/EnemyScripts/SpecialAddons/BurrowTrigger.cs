using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowTrigger : MonoBehaviour
{
    public BoneyPlantAI boneAI;
    public EliteBoneyPlantAI elitePlantAI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerBullet")
        {
            if (boneAI != null)
            { boneAI.Burrow(); }

            if(elitePlantAI != null)
            {
                elitePlantAI.Burrow();
            }
        }
    }
}
