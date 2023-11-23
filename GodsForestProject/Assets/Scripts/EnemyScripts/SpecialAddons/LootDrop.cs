using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    private GameObject weapDrop, itemDrop, buffDrop;

    private void Awake()
    {
        itemDrop = Resources.Load<GameObject>("Lootables/RandomItem");
        weapDrop = Resources.Load<GameObject>("Lootables/RandomWeapon");
    }
    internal void SetDrop(int chance)
    {
        int weapDropRoll = Random.Range(0, 1250);
        int itemDropRoll = Random.Range(0, 1000);
        int buffDropRoll = Random.Range(0, 1000);

        if (weapDropRoll < chance && PlayerStateManager.playerManager.weaponDropList.Count > 0)
        {
             DropStaff();
        }
        else if(itemDropRoll < chance * 4 && PlayerStateManager.playerManager.itemDropList.Count > 0)
        {
            //Instantiate(weapDrop, transform.position, Quaternion.identity);
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }

        else
        {

        }

        /*if (buffDropRoll < 200 && PlayerStateManager.playerManager.weaponDropList.Count > 0)
        {

        }*/
    }

    private void DropStaff()
    {
        int selector = Random.Range(1, PlayerStateManager.playerManager.staffDrops.Count);
        bool hasNow = false;
        for (int i = 0; i < PlayerController.instance.currentWeapons.Count; i++)
        {
            if (PlayerStateManager.playerManager.staffDrops[selector].name.Contains(PlayerController.instance.currentWeapons[i].GiveName()))
            {
                hasNow = true;
                Debug.Log("Staff Flagged");
            }
        }

        if (hasNow == false)
        {
            Instantiate(PlayerStateManager.playerManager.staffDrops[selector], transform.position, Quaternion.identity);
        }
        else
        {
            DropStaff();
        }
    }
}
