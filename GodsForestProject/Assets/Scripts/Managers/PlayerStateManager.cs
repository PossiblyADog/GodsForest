using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager playerManager;

    public int maxHP, currentHP, offFlatKnockModifier, defFlatKnockModifier, favor, damageFlatModifier, flatArmorModifier = 0;

    public float currentSpeed, currentRollForce, rollCooldown = 1.2f, rollMultiplier = 1.0f, hpMultiplier = 1.0f, speedMultiplier = 1.0f, offKnockMultiplier = 1.0f, defKnockMultiplier = 1.0f
        , favorMultiplier = 1.0f, primaryCastSpeedMultiplier = 1.0f, secondaryCastSpeedMultiplier = 1.0f, projectileTravelSpeedMultiplier = 1.0f, dotDurationMultiplier = 1.0f;

    private const int BASEHP = 50;
    public const float BASESPEED = 1.25f, BASEROLLFORCE = 18f;

    public int maxHPLevel = 0, healAmount, healAmountLevel = 0, favorUpLevel = 0, speedUpLevel = 0, armorUpLevel = 0, damageUpLevel = 0; 

    public bool invulnerable;

    public TMP_Text favorText;

    public List<AbstractPlayerWeapon> weaponDropList = new List<AbstractPlayerWeapon>();
    public List<Item> itemDropList = new List<Item>();

    public List<GameObject> staffDrops, bossDrops;


    public void Awake()
    {   
        if (playerManager == null)
        {
            playerManager = this;
            DontDestroyOnLoad(this);
            favor = 0;
            //StartCoroutine(StartUp());
            Initialize();
        }
        else
        {
            Destroy(this);
        }

    }

    /*IEnumerator StartUp()
    {
       yield return new WaitForSeconds(.5f);
       Initialize();

    }*/

    public void Initialize()
    {
        PopulateItemDropList();
        PopulateWeaponDropList();
        maxHP = BASEHP + (maxHPLevel * 10);
        healAmount = 10 + (healAmountLevel * 5);
        currentHP = maxHP;
        ResetModifiers();

        if (HealthbarUpdater.instance != null)
        {
            HealthbarUpdater.instance.SetMaxHP(maxHP);
            HealthbarUpdater.instance.SetCurrentHP(maxHP);
            FavorTransfer(0);
        }
        

        invulnerable = false;

    }


    private void ResetModifiers()
    {
        primaryCastSpeedMultiplier = 1.0f;
        secondaryCastSpeedMultiplier = 1.0f;
        flatArmorModifier = 0 + armorUpLevel;
        currentSpeed = BASESPEED + (speedUpLevel * .1f);
        currentRollForce = BASEROLLFORCE;
        damageFlatModifier = 0 + damageUpLevel;
        rollMultiplier = 1.0f;
        rollCooldown = 1.5f;
        hpMultiplier = 1.0f;
        speedMultiplier = 1.0f;
        offKnockMultiplier = 1.0f; 
        defKnockMultiplier = 1.0f;
        offFlatKnockModifier = 0;
        defFlatKnockModifier = 0;
        projectileTravelSpeedMultiplier = 1.0f;
        dotDurationMultiplier = 1.0f;
        favorMultiplier = 1.0f + (favorUpLevel * .2f);
    }

    public void ResetUpgrades()
    {
        maxHPLevel = 0;
        speedUpLevel = 0;
        favorUpLevel = 0;
        healAmountLevel = 0;
        armorUpLevel = 0;
        damageUpLevel = 0;
    }

    private void PopulateWeaponDropList()
    {
        weaponDropList = new List<AbstractPlayerWeapon>();
        weaponDropList.Add(new WirtsStaff());
        weaponDropList.Add(new DeathScepter());
        weaponDropList.Add(new CrystalRod());
        weaponDropList.Add(new FlameStaff());
        weaponDropList.Add(new FrostScepter());
        weaponDropList.Add(new ChaosRod());
        weaponDropList.Add(new BeamStaff());
        weaponDropList.Add(new DemonBlade());
        weaponDropList.Add(new TornadoStaff()); 
        weaponDropList.Add(new LordsStaff());
    }

    private void PopulateItemDropList()
    {
        itemDropList = new List<Item>();

        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new SteelHelm());
        }
        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new PocketCrossbow());
        }
        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new HolyDeflector());
        }
        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new MagicAcorn());
        }
        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new ArcaneEnhancement());
        }
        for (int i = 0; i < 10; i++)
        {
         itemDropList.Add(new HammerShot());
        }
        for (int i = 0; i < 10; i++)
        {
            itemDropList.Add(new RearviewMirror());
        }
        for (int i = 0; i < 10; i++)
        {
            itemDropList.Add(new TriggerFinger());
        }
        for (int i = 0; i < 5; i++)
        {
            itemDropList.Add(new StrangeOrb());
        }
        for (int i = 0; i < 5; i++)
        {
            itemDropList.Add(new PileOfRocks());
        }
        for (int i = 0; i < 10; i++)
        {
            itemDropList.Add(new KinemagicBooster());
        }
        for (int i = 0; i < 5; i++)
        {
            itemDropList.Add(new Hourglass());
        }

        itemDropList.Add(new SteelTippedBoots());
        itemDropList.Add(new VampireBlood());
    }

    public void UpgradeHealth()
    {
        if (maxHPLevel < 5 && favor >= (maxHPLevel* 125 + 125))
        {
            FavorTransfer(-(maxHPLevel*125 + 125));
            ChangeMaxHP(10);
            maxHPLevel++;
        }
    }

    public void UpgradeHealAmount()
    {
        if (healAmountLevel < 5 && favor >= healAmountLevel*100 + 100)
        {
            FavorTransfer(-(healAmountLevel* 100 + 100));
            healAmount += 5;
            healAmountLevel++;
        }
    }

    public void UpgradeDamage()
    {
        if(damageUpLevel < 5 && favor >= damageUpLevel * 200 + 100)
        {
            FavorTransfer(-(damageUpLevel * 200 + 100));
            damageFlatModifier++;
            damageUpLevel++;
        }
    }

    public void UpgradeSpeed()
    {
       if(speedUpLevel < 5 && favor >= speedUpLevel * 200 + 100)
        {
            FavorTransfer(-(speedUpLevel * 200 + 100));
            speedUpLevel++;
            currentSpeed += .1f;
        }
    }

    public void UpgradeFavorGain()
    {
        if(favorUpLevel < 5 && favor >= favorUpLevel * 250 + 125)
        {
            FavorTransfer(-(favorUpLevel * 250 + 125));
            favorUpLevel++;
            favorMultiplier += .2f;
        }
    }

    public void UpgradeArmor()
    {
        if(armorUpLevel < 5 && favor >= armorUpLevel * 400 + 200)
        {
            FavorTransfer(-(armorUpLevel * 400 + 200));
            armorUpLevel++;
            flatArmorModifier++;
        }
    }


    internal void ChangeMaxHP(int hpChange)
    {
        if (currentHP + hpChange > 1)
        {
            maxHP += hpChange;
        }
        else
        {
            maxHP = 1;
        }

        try
        {
            HealthbarUpdater.instance.SetMaxHP(maxHP);
        }
        catch
        {

        }

        if (hpChange >= 1)
        {
            SetCurrentHP(hpChange);
        }

        SetCurrentHP(0);
    }

    internal void SetCurrentHP(int hpChange)
    {
        if (!invulnerable)
        {
            if (currentHP + hpChange > maxHP)
            {
                currentHP = maxHP;
            }
            else
            {
                currentHP += hpChange;
            }


            if (currentHP < 0)
            {
                PlayerController.instance.SummonTheReaper();
            }
        }
        else if(invulnerable && hpChange > 0)
        {
            if (currentHP + hpChange <= maxHP)
            { currentHP += hpChange; }
            else { currentHP = maxHP; }
        }
        else
        {

        }

        try
        {
            HealthbarUpdater.instance.SetCurrentHP(currentHP);
        }
        catch
        {

        }
    }

    internal void ChangeSpeedMultiplier(float speedChange)
    {
        speedMultiplier += speedChange;
        currentSpeed = BASESPEED * speedMultiplier;
    }

    internal void ChangeRollMultiplier(float rollChange)
    {
        rollMultiplier += rollChange;
        currentRollForce = BASEROLLFORCE * rollMultiplier;

    }

    internal void ChangeOffKnock(float offChange)
    {
        offKnockMultiplier += offChange;

    }

    internal void ChangeDefKnock(float defChange)
    {
        defKnockMultiplier += defChange;
    }

    internal void FavorTransfer(int transferAmount)
    {
        try
        {
            if (favorText == null)
            {
                favorText = PlayerController.instance.fText;
            }
            favor += transferAmount;
            favorText.text = ":" + favor;
        }
        catch
        {

        }
    }

    internal void SetInvulnerableWindow(float duration)
    {
        StartCoroutine(Invulnerable(duration));
    }

    internal IEnumerator Invulnerable(float duration)
    {
        invulnerable = true;
        yield return new WaitForSeconds(duration);
        invulnerable = false;
    }

}
