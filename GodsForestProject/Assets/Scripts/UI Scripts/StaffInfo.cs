using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffInfo : MonoBehaviour
{
    public Image current, next, prev;

    public static StaffInfo instance;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetStaffInfo(int index)
    {
        current.sprite = PlayerController.instance.currentWeapons[index].weaponSprite;

        if (PlayerController.instance.currentWeapons.Count > 1)
        {
            try
            {
                next.sprite = PlayerController.instance.currentWeapons[index + 1].weaponSprite;
            }
            catch
            {
                next.sprite = PlayerController.instance.currentWeapons[0].weaponSprite;
            }

            try
            {
                prev.sprite = PlayerController.instance.currentWeapons[index - 1].weaponSprite;
            }
            catch
            {
                prev.sprite = PlayerController.instance.currentWeapons[PlayerController.instance.currentWeapons.Count-1].weaponSprite;
            }
        }
        else
        {
            prev.sprite = next.sprite = PlayerController.instance.currentWeapons[0].weaponSprite;
            next.sprite = next.sprite = PlayerController.instance.currentWeapons[0].weaponSprite;
        }
    }
}
