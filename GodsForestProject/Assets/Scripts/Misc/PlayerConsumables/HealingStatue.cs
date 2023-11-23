using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingStatue : AbstractInteractable
{
    private bool isUsed = false;
    public override void PerformInteraction()
    {
        if (!isUsed)
        {
            PlayerStateManager.playerManager.SetCurrentHP(1000);
            transform.GetComponent<Animator>().SetTrigger("isUsed");
            isUsed = true;
        }
    }
}


