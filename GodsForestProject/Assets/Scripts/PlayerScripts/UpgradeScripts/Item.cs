using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();
    public abstract string GiveDescription();

    public abstract int ItemRarity();
    public abstract Sprite ItemImage();
    public virtual void Update(PlayerController player, int stacks)
    {

    }

    public virtual void OnPlayerHit(PlayerController player, int stacks) //When player hits an enemy
    {

    }

    public virtual void OnPickUp()//When the item is acquired
    {

    }

    public virtual void OnPlayerRoll(PlayerController player, int stacks) // On Rolling
    {

    }

    public virtual void OnPlayerDamaged(PlayerController player, int stacks) // On taking damage
    {

    }

    public virtual void OnLMB(PlayerController player, Vector2 direction, int stacks)// On successful LMB cast
    {

    }

    public virtual void OnRMB(PlayerController player, Vector2 direction, int stacks) // On successful RMB cast
    {

    }

    public virtual void OnIncomingProjectile(PlayerController player, int stacks) //Used when a projectile enters a small radius around the player
    {

    }

    public virtual void OnEffectProjectile(GameObject bullet, int stacks) //Used to edit individual bullets
    {

    }

    public virtual void OnEnemyDeath(int stacks)//when any enemy dies
    {

    }

    public virtual void OnRemove(int stacks)
    {

    }

    public virtual Sprite LoadSprite(string textureName)
    {
        Sprite[] itemSprites = Resources.LoadAll<Sprite>("Images/IconSet");
        foreach (Sprite texture in itemSprites)
        {
            if (texture.name == textureName)
                return texture;
        }
        return null;
    }


}

public class SteelHelm : Item
{
    public int hpChangeValue;

    public override Sprite ItemImage()
    {
        return LoadSprite("SteelHelm");
    }
    public override string GiveName()
    {
        return "Steel Helm";
    }

    public override int ItemRarity()
    {
        return 1;
    }

    public override string GiveDescription()
    {
        return "Common piece of protection, increases max HP.";
    }

    public override void OnPickUp()
    {
        if(hpChangeValue == 0)
        {
            hpChangeValue = 15;
        }

        PlayerStateManager.playerManager.ChangeMaxHP(hpChangeValue);
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.ChangeMaxHP(-hpChangeValue * stacks);
    }
}

public class PocketCrossbow : Item
{

    int shotChance = 10;
    
    public GameObject projectile = Resources.Load<GameObject>("Sfx/PocketArrow");
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("PocketCrossbow");
    }

    public override string GiveName()
    {
        return "Pocket Crossbow";
    }
    public override string GiveDescription()
    {
        return "Magic crossbow! Simply place in your pocket and fire away.";
    }

    public override void OnLMB(PlayerController player, Vector2 direction, int stacks)
    {
        int roll = Random.Range(0, 100);

        if( roll < shotChance*stacks)
        {
            var bullet = GameObject.Instantiate(projectile, player.GetWeaponPosition(), Quaternion.identity);
            bullet.GetComponent<PlayerProjectile>().SetBulletParams(6.0f, 4, 0, direction, false, 0, true, 2);
        }
    }
    public override void OnRemove(int stacks)
    {
        shotChance = 0;
    }
}

public class RearviewMirror : Item
{
    float miniCD = .08f, miniTimer;
    int shotChance = 10;
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("RearviewMirror");
    }

    public override string GiveName()
    {
        return "Rearview Mirror";
    }
    public override string GiveDescription()
    {
        return "Doesn't help you see, but certainly not useless!";
    }

    public override void OnLMB(PlayerController player, Vector2 direction, int stacks)
    {
        int roll = Random.Range(0, 100);
        if (roll < shotChance * stacks && Time.time > miniTimer)
        {
            miniTimer = Time.time + miniCD;
            player.Fire_LMB_Rear();

        }
    }
    public override void OnRemove(int stacks)
    {
        shotChance = 0;
    }
}

public class SteelTippedBoots : Item
{
    public override Sprite ItemImage()
    {
        return LoadSprite("SteelTippedBoots");
    }
    public override int ItemRarity()
    {
        return 3;
    }
    public override string GiveName()
    {
        return "Steel-tipped Boots";
    }
    public override string GiveDescription()
    {
        return "Armor where it counts; gain small window of invulnerability during roll.";
    }

    public override void OnPlayerRoll(PlayerController player, int stacks)
    {
        PlayerStateManager.playerManager.SetInvulnerableWindow(.5f);
    }
}

public class HolyDeflector : Item
{

    GameObject effect = Resources.Load<GameObject>("Sfx/HolyDeflector");
    private float cooldown = 11.0f, nextTriggerTime = 0f;
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("HolyDeflector");
    }
    public override string GiveName()
    {
        return "Holy Deflector";
    }

    public override string GiveDescription()
    {
        return "Reflects incoming projectiles every so often.";
    }

    public override void OnPickUp()
    {
        if (cooldown > 1)
        { cooldown -= 1; }
    
    }

    public override void OnIncomingProjectile(PlayerController player, int stacks)
    {
        if (Time.time > nextTriggerTime)
        {
            var holyDef = GameObject.Instantiate(effect, player.transform.position, Quaternion.identity);
            holyDef.transform.parent = player.transform;
            GameObject.Destroy(holyDef, .4f);
            nextTriggerTime = Time.time + cooldown;
        }
    }

    public override void OnRemove(int stacks)
    {
        cooldown = 11.0f;
    }
}

public class MagicAcorn : Item
{
    public override Sprite ItemImage()
    {
        return LoadSprite("MagicAcorn");
    }
    public override int ItemRarity()
    {
        return 1;
    }
    public override string GiveName()
    {
        return "Magic Acorn";
    }

    public override string GiveDescription()
    {
        return "Eating this acorn has made you very energetic...";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.ChangeSpeedMultiplier(.08f);
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.ChangeSpeedMultiplier(-.08f * stacks);
    }
}

public class ArcaneEnhancement : Item
{
    public override Sprite ItemImage()
    {
        return LoadSprite("ArcaneEnhancement");
    }
    public override int ItemRarity()
    {
        return 1;
    }
    public override string GiveName()
    {
        return "Arcane Enhancement";
    }
    public override string GiveDescription()
    {
        return "Lightly increases damage with most weapons.";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.damageFlatModifier += 1;
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.damageFlatModifier -= 1*stacks;
    }
}

public class HammerShot : Item
{
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("HammerShot");
    }
    public override string GiveName()
    {
        return "Hammer Shot";
    }
    public override string GiveDescription()
    {
        return "Essence of smack; increases your force against enemies.";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.offFlatKnockModifier += 6;
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.offFlatKnockModifier -= 6 * stacks;
    }
}

public class TriggerFinger : Item
{
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("TriggerFinger");
    }
    public override string GiveName()
    {
        return "Trigger Finger";
    }

    public override string GiveDescription()
    {
        return "Unknown origin, but it seems to scare yours into working faster!";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.primaryCastSpeedMultiplier += .04f;
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.primaryCastSpeedMultiplier -= .04f * stacks;
    }
}

public class PileOfRocks : Item
{
    public override int ItemRarity()
    {
        return 2;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("PileOfRocks");
    }
    public override string GiveName()
    {
        return "Pile of Rocks";
    }

    public override string GiveDescription()
    {
        return "Tired of getting knocked around? Put some rocks in your pocket!";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.defKnockMultiplier += .1f;
    }

    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.defKnockMultiplier -= .1f * stacks;
    }
}
public class Hourglass : Item
{
    public override int ItemRarity()
    {
        return 2;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("Hourglass");
    }
    public override string GiveName()
    {
        return "Hourglass";
    }

    public override string GiveDescription()
    {
        return "Recover faster with the power of Time!";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.rollCooldown -= .2f;
    }
    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.rollCooldown += .2f * stacks;
    }
}

public class KinemagicBooster : Item
{
    public override int ItemRarity()
    {
        return 1;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("KinemagicBooster");
    }
    public override string GiveName()
    {
        return "Kinemagic Booster";
    }

    public override string GiveDescription()
    {
        return "Time arrows like a banana.\nFruit flies.";
    }

    public override void OnPickUp()
    {
        PlayerStateManager.playerManager.projectileTravelSpeedMultiplier += .08f;
    }
    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.projectileTravelSpeedMultiplier -= .08f * stacks;
    }
}

public class StrangeOrb : Item
{
    public GameObject laserBotPrefab = Resources.Load<GameObject>("Sfx/LaserBot"), currentBot;

    public override int ItemRarity()
    {
        return 2;
    }
    public override Sprite ItemImage()
    {
        return LoadSprite("StrangeOrb");
    }
    public override string GiveName()
    {
        return "Strange Orb";
    }

    public override string GiveDescription()
    {
        return "A friendly little ball that shoots lasers.";
    }

    public override void OnPickUp()
    {
        if (PlayerController.instance.transform.GetComponentInChildren<LaserBotAI>() == null)
        {

            currentBot = GameObject.Instantiate(laserBotPrefab, PlayerController.instance.transform);
            currentBot.GetComponentInChildren<LaserBotAI>().InitializeBot();
        }
        else
        {
            PlayerController.instance.transform.GetComponentInChildren<LaserBotAI>().UpgradeBot();
        }
    }
    public override void OnRemove(int stacks)
    {
        GameObject.Destroy(currentBot.gameObject);
    }

}

public class AnkleLordsCrown : Item
{
    int shotCounter = 0;
    public override Sprite ItemImage()
    {
            return LoadSprite("AnkleCrown");
    }
    public override string GiveName()
    {
            return "Ankle Lord's Crown";
    }
    public override string GiveDescription()
    {
            return "To slay the King, is to become the King.";
    }
    public override int ItemRarity()
    {
        return 4;
    }

    public override void OnEffectProjectile(GameObject bullet, int stacks)
    {
        shotCounter++;
        if(shotCounter > 3)
        {
            bullet.transform.localScale *= 2;
            shotCounter = 0;
        }
    }
}
public class GodsHamstrings : Item
    {
    public override int ItemRarity()
    {
        return 4;
    }
    public override Sprite ItemImage()
        {
            return LoadSprite("GoldenString");
        }
        public override string GiveName()
        {
            return "God's Hamstrings";
        }
        public override string GiveDescription()
        {
            return "It wasn't easy replacing yours, now go forth and roll";
        }

        public override void OnPickUp()
        {
            PlayerStateManager.playerManager.rollCooldown /= 5;
            PlayerStateManager.playerManager.speedMultiplier += 1.0f;
        }
    public override void OnRemove(int stacks)
    {
        PlayerStateManager.playerManager.rollCooldown *= 5;
        PlayerStateManager.playerManager.speedMultiplier -= 1.0f;
    }
}

public class VampireBlood : Item
{
    public override int ItemRarity()
    {
        return 3;
    }

    public override Sprite ItemImage()
    {
        return LoadSprite("VampireBlood");
    }
    public override string GiveName()
    {
        return "Vampire Blood";
    }
    public override string GiveDescription()
    {
        return "The blood that just keeps on giving!";
    }

    public override void OnEnemyDeath(int stacks)
    {
        if(Random.Range(0, 1000)%3 == 0)
        {
            PlayerStateManager.playerManager.ChangeMaxHP(2);
        }
    }

}

public class OldWomansPendant : Item
{

    public override Sprite ItemImage()
    {
        return LoadSprite("OldWomansPendant");
    }
    public override string GiveName()
    {
        return "Old Woman's Pendant";
    }

    public override int ItemRarity()
    {
        return 6;
    }

    public override string GiveDescription()
    {
        return "A memory that gives the strength to keep going.";
    }

}

