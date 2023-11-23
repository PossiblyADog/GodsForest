using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRod : AbstractPlayerWeapon
{
    public override void Awake()
    {
        weaponSprite = LoadSprite("Crystal Rod");

        primaryProj = Resources.Load<GameObject>("Sfx/CrystalStar");
        secondaryProj = Resources.Load<GameObject>("Sfx/CrystalRocket");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 3;
        primaryCD = .17f;
        secondaryCD = 3f;
        primaryKnock = 1.0f;
        secondaryKnock = 60f;
        primarySpeed = 6.5f;
        secondarySpeed = 7.0f;

    }
    public override string GiveName()
    {
        return "Crystal Rod";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("CrystalStar", "CrystalRocket");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier); 
            PlayerController.instance.Call_LMB_Items();
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, weaponDamage + (PlayerStateManager.playerManager.damageFlatModifier/2), (primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier/3), targetPos, false, 0, true, 2);
            player.PlayPlayerSound(primaryShootSFX, false);
            StaffCooldownManager.instance.SetLMB_CD(primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            BulletEffectors(bullet);
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            PlayerController.instance.Call_RMB_Items();
            var bullet = GameObject.Instantiate(secondaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetRocket();
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(secondarySpeed, weaponDamage + 8 + (PlayerStateManager.playerManager.damageFlatModifier * 3), secondaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, true, 0);
            bullet.GetComponent<ProjectileExpander>().SetExpansionRate(.01f, true);
            player.PlayPlayerSound(secondaryShootSFX, false);
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
        }

    }

    public override string GiveDescription()
    {
        return "Really puts the rock in rocket.";
    }
}
