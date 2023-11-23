using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterStaff : AbstractPlayerWeapon
{

    public override void Awake()
    {
        weaponSprite = LoadSprite("Starter Staff");
        primaryProj = Resources.Load<GameObject>("Sfx/StarterStaffPrimary");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootStarterShot");
        nextShotTime = Time.time;
        weaponDamage = 7;
        primaryCD = .45f;
        primarySpeed = 7.0f;
        primaryKnock = 5.0f;

    }
    public override string GiveName()
    {
        return "Starter Staff";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("StarterShot", "Empty");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            PlayerController.instance.Call_LMB_Items();
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, (weaponDamage + PlayerStateManager.playerManager.damageFlatModifier), primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, true, 1);
            player.PlayPlayerSound(primaryShootSFX, true);
            BulletEffectors(bullet);
            StaffCooldownManager.instance.SetLMB_CD(primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
        }
    }

    public override string GiveDescription()
    {
        return "It ain't much, but it's better than a fist!";
    }

}

