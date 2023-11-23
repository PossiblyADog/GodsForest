using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostScepter : AbstractPlayerWeapon
{

    public override void Awake()
    {
        weaponSprite = LoadSprite("Frost Scepter");
        primaryProj = Resources.Load<GameObject>("Sfx/IcePick");
        secondaryProj = Resources.Load<GameObject>("Sfx/IceNova");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootIcePick");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/IceNovaShoot");
        weaponDamage = 10;
        primaryCD = 1.0f;
        secondaryCD = 4f;
        primaryKnock = 10.0f;
        primarySpeed = 6.5f;
        secondaryKnock = 40.0f;


    }
    public override string GiveName()
    {
        return "Frost Scepter";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("IcePick", "IceNova");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            PlayerController.instance.Call_LMB_Items();
            var bullet = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(primarySpeed, (weaponDamage + PlayerStateManager.playerManager.damageFlatModifier*2) , primaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, targetPos, false, 0, true, 0);
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
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            var nova = GameObject.Instantiate(secondaryProj, player.transform.position, Quaternion.identity);
            nova.GetComponent<IceNova>().SetNovaParams((5 + PlayerStateManager.playerManager.damageFlatModifier * 2) , secondaryKnock + PlayerStateManager.playerManager.offFlatKnockModifier, .75f, .02f, 3.0f, .3f);
            player.PlayPlayerSound(secondaryShootSFX, false);

        }
    }
    public override string GiveDescription()
    {
        return "Frost Scepter descript holder";
    }
}
