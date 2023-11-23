using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosRod : AbstractPlayerWeapon
{
    public override void Awake()
    {
        weaponSprite = LoadSprite("Rod of Chaos");
        primaryProj = Resources.Load<GameObject>("Sfx/ChaosBall");
        secondaryProj = Resources.Load<GameObject>("Sfx/ChaosOrb");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ChaosBallShoot");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ChaosOrbShoot");
        weaponDamage = 7;
        primaryCD = .3f;
        secondaryCD = 3f;
        primaryKnock = 5.0f;
        secondaryKnock = 5.0f;
        primarySpeed = 6.5f;
        secondarySpeed = 3f;

    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("ChaosBall", "ChaosCloud");
    }
    public override string GiveName()
    {
        return "Rod of Chaos";
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            PlayerController.instance.Call_LMB_Items();
            player.PlayPlayerSound(primaryShootSFX, false);
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            StaffCooldownManager.instance.SetLMB_CD(primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, weaponDamage + PlayerStateManager.playerManager.damageFlatModifier, primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, true, 0, true, 5);
            BulletEffectors(bullet);
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            PlayerController.instance.Call_RMB_Items();
            var bullet = GameObject.Instantiate(secondaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(secondarySpeed, (weaponDamage - 2) + PlayerStateManager.playerManager.damageFlatModifier, secondaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, false, 1);
            bullet.GetComponent<ChaosOrb>().Initialize(weaponDamage + PlayerStateManager.playerManager.damageFlatModifier, primarySpeed, (primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier)/2);
            player.PlayPlayerSound(secondaryShootSFX, false);
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
        }

    }

    public override string GiveDescription()
    {
        return "A wily beast, for sure.";
    }
}