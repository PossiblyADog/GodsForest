using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class AbstractPlayerWeapon
{
    
    public Sprite weaponSprite;

    protected int weaponDamage;
    public float primaryCD, secondaryCD, primarySpeed, secondarySpeed, primaryKnock, secondaryKnock, nextShotTime, secondaryShotTime;

    protected AudioClip primaryShootSFX, secondaryShootSFX;

    public abstract string GiveDescription();
    public abstract string GiveName();

    [SerializeField]
    protected GameObject primaryProj, secondaryProj;

    public virtual void Awake()
    {

    }
    public virtual void OnLMB(PlayerController player, Vector2 targetPos)
    {

    }

    public virtual void OnRMB(PlayerController player, Vector2 targetPos)
    {

    }

    public virtual void LMB_Manual(PlayerController player, Vector2 targetPos)
    {
        nextShotTime = 0;
        OnLMB(player, targetPos);
    }

    public virtual void OnLMB_Release(PlayerController player, Vector2 targetPos)
    {

    }

    public virtual void SetCDPanelData()
    {

    }

    public virtual void BulletEffectors(GameObject bullet)
    {
        if (PlayerController.instance.items.Count > 0)
        {
            foreach (var i in PlayerController.instance.items)
            {
                i.item.OnEffectProjectile(bullet, i.itemStacks);

            }
        }
    }

    public virtual Sprite LoadSprite(string textureName)
    {
        Sprite[] weaponSprites = Resources.LoadAll<Sprite>("Weapons/WeaponsSheet");
        foreach (Sprite texture in weaponSprites)
        {
            if (texture.name == textureName)
                return texture;
        }
        return null;
    }


}

