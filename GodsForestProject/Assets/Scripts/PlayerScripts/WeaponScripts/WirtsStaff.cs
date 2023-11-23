using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirtsStaff : AbstractPlayerWeapon
{
    public override void Awake()
    {
        weaponSprite = LoadSprite("Wirt's Staff");
        primaryProj = Resources.Load<GameObject>("Sfx/LightningBall");
        secondaryProj = Resources.Load<GameObject>("Sfx/BullShot");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootElectricBall");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 5;
        primaryCD = .75f;
        secondaryCD = 3f;
        primaryKnock = 5.0f;
        secondaryKnock = 50f;
        primarySpeed = 6.5f;
        secondarySpeed = 3.0f;
        
    }

    public override void SetCDPanelData()
    {
       StaffCooldownManager.instance.SetPanelData("WirtBall", "Bull");
    }
    public override string GiveName()
    {
        return "Wirt's Staff";
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            PlayerController.instance.Call_LMB_Items();
            player.PlayPlayerSound(primaryShootSFX, false);
            StaffCooldownManager.instance.SetLMB_CD(primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            for (int i = -1; i < 2; i++)
            {
                var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
                bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, (weaponDamage + PlayerStateManager.playerManager.damageFlatModifier), primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, i, true, 3);
                BulletEffectors(bullet);
            }
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            PlayerController.instance.Call_RMB_Items();
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            var bullet = GameObject.Instantiate(secondaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(secondarySpeed, (weaponDamage + PlayerStateManager.playerManager.damageFlatModifier )* 2, secondaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, false, 0);
            player.PlayPlayerSound(secondaryShootSFX, false);

        }

    }

    public override string GiveDescription()
    {
        return "Appears to be part of a femur on a stick. Perhaps a relic from the past?";
    }
}
