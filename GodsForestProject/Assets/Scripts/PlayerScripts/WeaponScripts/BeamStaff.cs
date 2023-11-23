using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamStaff : AbstractPlayerWeapon
{
    private GameObject laserBeam;
    public override void Awake()
    {
        weaponSprite = LoadSprite("Beam Rod");

        primaryProj = Resources.Load<GameObject>("Sfx/LaserBeam");
        secondaryProj = Resources.Load<GameObject>("Sfx/RefractorCube");
        primaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalStar");
        secondaryShootSFX = Resources.Load<AudioClip>("Sounds/ShootCrystalRocket");
        weaponDamage = 2;
        primaryCD = .35f;
        secondaryCD = 10.0f;
        secondarySpeed = 6.0f;


    }
    public override void SetCDPanelData()
    {
        StaffCooldownManager.instance.SetPanelData("Beam", "Refractor");        
    }
    public override string GiveName()
    {
        return "Beam Rod";
    }


    public override void OnLMB(PlayerController player, Vector2 targetPos)
    {
        if(laserBeam == null)
        {
            laserBeam = GameObject.Instantiate(primaryProj, player.GetWeaponPosition(), Quaternion.identity);
            laserBeam.GetComponent<PlayerLaser>().SetLaser(weaponDamage + (PlayerStateManager.playerManager.damageFlatModifier / 2), primaryCD);
        }
    }

    public override void OnLMB_Release(PlayerController player, Vector2 targetPos)
    {
        if(laserBeam != null)
        {
            GameObject.Destroy(laserBeam.gameObject);
        }
    }

    public override void OnRMB(PlayerController player, Vector2 targetPos)
    {
        if (Time.time > secondaryShotTime)
        {
            PlayerController.instance.Call_RMB_Items();
            var cube = GameObject.Instantiate(secondaryProj, CursorController.instance.transform.position, Quaternion.identity);
            cube.GetComponent<Refractor>().SetRefractData(secondarySpeed);
            StaffCooldownManager.instance.SetRMB_CD(secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
            secondaryShotTime = Time.time + (secondaryCD / PlayerStateManager.playerManager.secondaryCastSpeedMultiplier);
        }

    }

    public override string GiveDescription()
    {
        return "What would a magical world be without a few beams.";
    }
}
