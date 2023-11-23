using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public GameObject entryEffect;
    public AudioClip pickupSound;
    public virtual void Start()
    {
        var effect = Instantiate(entryEffect, transform.position, Quaternion.identity);
        Destroy(effect, .5f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (gameObject.name.Contains("Random"))
            { SendRandomItem(); }
            else
            {
                SendSpecificItem();
            }
        }
    }

    private void SendRandomItem()
    {
        if (PlayerStateManager.playerManager.itemDropList.Count > 0)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
            int selector = Random.Range(0, PlayerStateManager.playerManager.itemDropList.Count);
            var newItem = new ItemList(PlayerStateManager.playerManager.itemDropList[selector], PlayerStateManager.playerManager.itemDropList[selector].GiveName());
            PlayerController.instance.AddItem(newItem);
            PlayerStateManager.playerManager.itemDropList.RemoveAt(selector);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SendSpecificItem()
    {
        AudioSource.PlayClipAtPoint(pickupSound, transform.position, GameManager.instance.sfxVolume);
        for(int i = 0; i < GameManager.instance.allItems.Length; i++)
        {
            if (gameObject.name.Contains(GameManager.instance.allItems[i].GiveName()))
            {
                var itemToAdd = new ItemList(GameManager.instance.allItems[i], GameManager.instance.allItems[i].GiveName());
                PlayerController.instance.AddItem(itemToAdd);
                Destroy(this.gameObject);
            }
        }
    }


}
