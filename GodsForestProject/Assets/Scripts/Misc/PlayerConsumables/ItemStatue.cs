using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemStatue : AbstractInteractable
{
    public GameObject randomItem;
    public TMP_Text costText;
    int cost = 50;
    public override void PerformInteraction()
    {
        if (PlayerStateManager.playerManager.favor >= cost)
        {
            PlayerStateManager.playerManager.FavorTransfer(-cost);
            Instantiate(randomItem, transform.position - new Vector3(0, 1), Quaternion.identity);
            cost += 50;
            costText.text = ":" + cost + "f";
            
        }
    }
}

