using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoStaff : AbstractPlayerWeapon
{
    private GameObject stormPoint;
    public override void Awake()
    {
        weaponSprite = LoadSprite("Tornado Staff");

        primaryProj = Resources.Load<GameObject>("Sfx/LightningStrike");
        secondaryProj = Resources.Load<GameObject>("Sfx/Tornado");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 5;
        primaryCD = .5f;
        secondaryCD = 1.25f;
        primaryKnock = 15.0f;
        secondaryKnock = 20.0f;
        secondarySpeed = 5.0f;

    }
    public override string GiveName()
    {
        return "Tornado Staff";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("Lightning", "Tornado");
    }


    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            if (stormPoint == null)
            {
                stormPoint = GameObject.Instantiate(primaryProj, CursorController.instance.transform.position, Quaternion.identity);
                stormPoint.GetComponent<ChargeShot>().StartCharging(.01f);//scale increase per frame
            }
        }
    }

    public override void OnLMB_Release(PlayerController player, Vector2 targetPos)
    {
        if (stormPoint != null)
        {
            stormPoint.GetComponent<ChargeShot>().StopChargingStatic(weaponDamage + (PlayerStateManager.playerManager.damageFlatModifier/2), primaryKnock, .725f);
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            PlayerController.instance.Call_RMB_Items();
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            var bullet = GameObject.Instantiate(secondaryProj, (Vector2)PlayerController.instance.transform.position + new Vector2(.5f, 0f), PlayerController.instance.transform.rotation);
            bullet.GetComponent<Tornado>().SetTornado(5.0f, targetPos, secondaryKnock, 4.0f, secondarySpeed);
            player.PlayPlayerSound(secondaryShootSFX, false);
        }

    }

    public override string GiveDescription()
    {
        return "The government isn't the only one controlling the weather!";
    }
}
