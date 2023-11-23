using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItem : AbstractConsumable
{
    public override void ApplyEffect()
    {
        try
        {
            PlayerStateManager.playerManager.SetCurrentHP(PlayerStateManager.playerManager.healAmount);
        }
        catch
        {
            PlayerStateManager.playerManager.SetCurrentHP(10);
        }
    }
}
