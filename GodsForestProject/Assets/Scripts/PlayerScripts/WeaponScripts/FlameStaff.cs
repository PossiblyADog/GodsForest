using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameStaff : AbstractPlayerWeapon
{
    public override void Awake()
    {
        weaponSprite = LoadSprite("Flame Staff");

        primaryProj = Resources.Load<GameObject>("Sfx/Fireball");
        secondaryProj = Resources.Load<GameObject>("Sfx/SmallFireball");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootFireball");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootSmallFireball");
        weaponDamage = 6;
        primaryCD = 1.1f;
        secondaryCD = .11f;
        primaryKnock = 20.0f;
        secondaryKnock = 0.0f;
        primarySpeed = 6.5f;
        secondarySpeed = 2.5f;

    }
    public override string GiveName()
    {
        return "Flame Staff";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("Fireball", "Flamethrower");
    }
    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier) ;
            PlayerController.instance.Call_LMB_Items();
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, weaponDamage + PlayerStateManager.playerManager.damageFlatModifier, primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, true, 0);
            bullet.GetComponent<ProjectileExpander>().SetExpansionRate(.01f, true);
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
            player.PlayPlayerSound(secondaryShootSFX, true);
            for (int i = 0; i < 3; i++)
            {
                var bullet = GameObject.Instantiate(secondaryProj, player.GetWeaponPosition(), Quaternion.identity);
                bullet.GetComponent<PlayerProjectile>().SetBulletParams(secondarySpeed + Random.value, 2 + PlayerStateManager.playerManager.damageFlatModifier / 2, secondaryKnock, targetPos, true, 0, false, 0);
                bullet.GetComponent<PlayerProjectile>().SetDestroy(1.0f);
            }


        }

    }

    public override string GiveDescription()
    {
        return "A rather odd looking staff that spits fire!";
    }
}