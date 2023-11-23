using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FavorItem : AbstractConsumable
{

    public int favorAmount;

    public override void ApplyEffect()
    {
        PlayerStateManager.playerManager.FavorTransfer(Mathf.RoundToInt(favorAmount * PlayerStateManager.playerManager.favorMultiplier));
    }
}
