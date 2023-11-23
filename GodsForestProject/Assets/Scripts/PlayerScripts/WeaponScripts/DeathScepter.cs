using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScepter : AbstractPlayerWeapon
{

    public override void Awake()
    {
        weaponSprite = LoadSprite("Death Scepter");
        primaryProj = Resources.Load<GameObject>("Sfx/FlyingSkull");
        secondaryProj = Resources.Load<GameObject>("Sfx/FelCloud");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 7;
        primaryCD = .5f;
        secondaryCD = 7f;
        primaryKnock = 15.0f;
        primarySpeed = 7.0f;

    }
    public override string GiveName()
    {
        return "Death Scepter";
    }

    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("FlyingSkull", "FelCloud");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
       if(Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
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
            var vortex = GameObject.Instantiate(secondaryProj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            vortex.GetComponent<FelVortex>().SetVortex(6.0f, 8.0f, 50.0f);
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            player.PlayPlayerSound(secondaryShootSFX, false);

        }
    }
    public override string GiveDescription()
    {
        return "You heard of a man who was banished for tinkering with the Dark Arts, hopefully he has long perished.";
    }
}
