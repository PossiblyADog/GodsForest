using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordsStaff : AbstractPlayerWeapon
{
    public override void Awake()
    {
        weaponSprite = LoadSprite("Lord's Staff");

        primaryProj = Resources.Load<GameObject>("Sfx/LordBall");
        secondaryProj = Resources.Load<GameObject>("Sfx/SuckerBomb");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 5;
        primaryCD = 1.0f;
        secondaryCD = 7.0f;
        primaryKnock = 10.0f;
        primarySpeed = 4.5f;

    }
    public override string GiveName()
    {
        return "Lord's Staff";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("LordBall", "SuckerBomb");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            PlayerController.instance.Call_LMB_Items();
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, weaponDamage + PlayerStateManager.playerManager.damageFlatModifier, (primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier), targetPos, false, 0, false, 2);
            player.PlayPlayerSound(primaryShootSFX, false);
            StaffCooldownManager.instance.SetLMB_CD(primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            bullet.GetComponent<CarpetBombShot>().StartPlayerBombing(.33f);
            BulletEffectors(bullet);
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            PlayerController.instance.Call_RMB_Items();
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            var bullet = GameObject.Instantiate(secondaryProj, CursorController.instance.transform.position, Quaternion.identity);
            bullet.GetComponent<SuckerBomb>().SetBomba(4.0f);
            player.PlayPlayerSound(secondaryShootSFX, false);
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
        }

    }

    public override string GiveDescription()
    {
        return "Sometimes, it is the crown that makes the man.";
    }
}
