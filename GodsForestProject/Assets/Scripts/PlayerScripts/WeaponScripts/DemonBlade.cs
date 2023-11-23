using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBlade : AbstractPlayerWeapon
{
    private GameObject spinBlade;
    public override void Awake()
    {
        weaponSprite = LoadSprite("Demon Blade");

        primaryProj = Resources.Load<GameObject>("Sfx/SpinningBlade");
        secondaryProj = Resources.Load<GameObject>("Sfx/DemonBladeShot");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 8;
        primaryCD = .25f;
        secondaryCD = 1.0f;
        primaryKnock = 20.0f;
        secondaryKnock = 1.0f;
        primarySpeed = 10.0f;
        secondarySpeed = 6.25f;

    }
    public override string GiveName()
    {
        return "Demon Blade";
    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("SpinBlade", "DemonBlade");
    }

    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > nextShotTime)
        {

            if (spinBlade == null)
            {
                spinBlade = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
                spinBlade.GetComponent<SpinningBlade>().SetSpin(weaponDamage + PlayerStateManager.playerManager.damageFlatModifier, primaryKnock, primarySpeed);
                nextShotTime = Time.time + (primaryCD / PlayerStateManager.playerManager.primaryCastSpeedMultiplier);
            }
        }
    }

    public override void OnLMB_Release(PlayerController player, Vector2 targetPos)
    {
        if (spinBlade != null)
        {
            GameObject.Destroy(spinBlade.gameObject);
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
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(secondarySpeed * PlayerStateManager.playerManager.projectileTravelSpeedMultiplier, weaponDamage/2 + PlayerStateManager.playerManager.damageFlatModifier, secondaryKnock, targetPos, false, 0, false, 5);
            player.PlayPlayerSound(secondaryShootSFX, false);
        }

    }

    public override string GiveDescription()
    {
        return "What would a magical world be without a few beams.";
    }
}