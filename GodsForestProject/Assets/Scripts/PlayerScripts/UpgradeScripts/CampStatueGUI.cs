using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CampStatueGUI : MonoBehaviour
{
    public TMP_Text healthCost, healingCost, armorCost, speedCost, damageCost, favorCost;
    public List<Sprite> barImages;
    public Image maxHPBar, healBar, speedBar, armorBar, damageBar, favorBar;
    private Canvas statueCanvas;

    private void Start()
    {
        statueCanvas = GetComponentInChildren<Canvas>();
        statueCanvas.enabled = false;
        UpdateCostText();
    }
    public void HealthUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeHealth();
        UpdateCostText();
    }

    public void HealingUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeHealAmount();
        UpdateCostText();
    }

    public void SpeedUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeSpeed();
        UpdateCostText();
    }

    public void DamageUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeDamage();
        UpdateCostText();
    }

    public void ArmorUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeArmor();
        UpdateCostText();
    }

    public void FavorUpgradeButton()
    {
        PlayerStateManager.playerManager.UpgradeFavorGain();
        UpdateCostText();
    }

    internal void UpdateCostText()
    {
        if (PlayerStateManager.playerManager.maxHPLevel < 6)
        {
            healthCost.text = "Cost: " + (PlayerStateManager.playerManager.maxHPLevel * 125 + 125);
            maxHPBar.sprite = barImages[PlayerStateManager.playerManager.maxHPLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

        if (PlayerStateManager.playerManager.healAmountLevel < 6)
        {
            healingCost.text = "Cost: " + (PlayerStateManager.playerManager.healAmountLevel * 100 + 100);
            healBar.sprite = barImages[PlayerStateManager.playerManager.healAmountLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

        if (PlayerStateManager.playerManager.armorUpLevel < 6)
        {
            armorCost.text = "Cost: " + (PlayerStateManager.playerManager.armorUpLevel * 400 + 200);
            armorBar.sprite = barImages[PlayerStateManager.playerManager.armorUpLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

        if (PlayerStateManager.playerManager.speedUpLevel < 6)
        {
            speedCost.text = "Cost : " + (PlayerStateManager.playerManager.speedUpLevel * 200 + 100);
            speedBar.sprite = barImages[PlayerStateManager.playerManager.speedUpLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

        if (PlayerStateManager.playerManager.damageUpLevel < 6)
        {
            damageCost.text = "Cost: " + (PlayerStateManager.playerManager.damageUpLevel * 200 + 100);
            damageBar.sprite = barImages[PlayerStateManager.playerManager.damageUpLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

        if (PlayerStateManager.playerManager.favorUpLevel < 6)
        {
            favorCost.text = "Cost: " + (PlayerStateManager.playerManager.favorUpLevel * 225 + 125);
            favorBar.sprite = barImages[PlayerStateManager.playerManager.favorUpLevel];
        }
        else
        {
            favorCost.text = "Cost: Maxed";
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { statueCanvas.enabled = true; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { statueCanvas.enabled = false; }
        
    }
}
