using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCruncherButton : MonoBehaviour
{
    private ItemList thisItem;
    public ItemCruncherController controller;
    public AudioClip pickupSound;
 
    public void OnClick()
    {

        if (thisItem.itemStacks <= 0)
        {
           
        }
        else
        {
            int itemCount = thisItem.itemStacks;
            thisItem.item.OnRemove(thisItem.itemStacks);
            PlayerController.instance.items.Remove(thisItem);
            

            for (int i = 0; i < itemCount; i++)
            {
                if (PlayerStateManager.playerManager.itemDropList.Count > 0)
                {

                    int selector = Random.Range(0, PlayerStateManager.playerManager.itemDropList.Count);
                    var newItem = new ItemList(PlayerStateManager.playerManager.itemDropList[selector], PlayerStateManager.playerManager.itemDropList[selector].GiveName());
                    PlayerController.instance.AddItem(newItem);
                }
            }

            controller.charges -= 1;
            if(controller.charges <= 0)
            {
                controller.ChargesUsed();
            }

            controller.RefreshOnClick();
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
        }
    }

    public void SetButtonData(Item buttonData, ItemCruncherController current)
    {
        controller = current;
        this.name = buttonData.GiveName();
        GetComponent<Image>().sprite = buttonData.ItemImage();

        if (PlayerController.instance.items.Exists(targetItem => targetItem.name.Contains(this.name)))
        {
            thisItem = PlayerController.instance.items.Find(targetItem => targetItem.name.Contains(this.name));
            GetComponentInChildren<TMP_Text>().text = thisItem.itemStacks.ToString();
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
