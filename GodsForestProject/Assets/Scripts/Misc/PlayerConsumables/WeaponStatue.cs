using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponStatue : AbstractInteractable
{
    public GameObject randomWeapon;
    int cost = 200;
    public TMP_Text costText;
    public override void PerformInteraction()
    {
        if (PlayerStateManager.playerManager.favor >= cost)
        {
            PlayerStateManager.playerManager.FavorTransfer(-cost);
            Instantiate(randomWeapon, transform.position - new Vector3(0, 1), Quaternion.identity);
            cost += 150;
            costText.text = ":" + cost + "f";
        }
    }
}
