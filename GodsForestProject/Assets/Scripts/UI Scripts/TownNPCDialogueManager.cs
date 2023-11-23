using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownNPCDialogueManager : MonoBehaviour
{
    public Canvas dialogueCanvas;
    public GameObject homePanel, panelOne, panelTwo, panelThree, panelFour;

    public void Back(GameObject panel)
    {
        panel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void OptionOne()
    {
        panelOne.SetActive(true);
        homePanel.SetActive(false);
    }

    public void OptionTwo()
    {
        panelTwo.SetActive(true);
        homePanel.SetActive(false);
    }

    public void OptionThree()
    {
        panelThree.SetActive(true);
        homePanel.SetActive(false);
    }

    public void OptionFour()
    {
        panelFour.SetActive(true);
        homePanel.SetActive(false);
    }

    public void BuyItem(int amount)
    {
        if(amount == 1)
        {
            if(PlayerStateManager.playerManager.favor >= 75)
            {
                PlayerStateManager.playerManager.FavorTransfer(-75);
                int selector = Random.Range(0, PlayerStateManager.playerManager.itemDropList.Count);
                var newItem = new ItemList(PlayerStateManager.playerManager.itemDropList[selector], PlayerStateManager.playerManager.itemDropList[selector].GiveName());
                PlayerController.instance.AddItem(newItem);
                PlayerStateManager.playerManager.itemDropList.RemoveAt(selector);
            }
        }
        if(amount == 5)
        {
            if(PlayerStateManager.playerManager.favor >= 300)
            {
                for (int i = 0; i < 5; i++)
                {
                    PlayerStateManager.playerManager.FavorTransfer(-300);
                    int selector = Random.Range(0, PlayerStateManager.playerManager.itemDropList.Count);
                    var newItem = new ItemList(PlayerStateManager.playerManager.itemDropList[selector], PlayerStateManager.playerManager.itemDropList[selector].GiveName());
                    PlayerController.instance.AddItem(newItem);
                    PlayerStateManager.playerManager.itemDropList.RemoveAt(selector);
                }
            }
        }
    }

    public void BuyStaff()
    {
        if(PlayerStateManager.playerManager.favor >= 225)
        {
            PlayerStateManager.playerManager.FavorTransfer(-225);
            int selector = Random.Range(0, PlayerStateManager.playerManager.weaponDropList.Count);
            Instantiate(PlayerStateManager.playerManager.staffDrops[selector], transform.position + new Vector3(1, -1), Quaternion.identity);
        }
    }

    public void RepairPendant()
    {
        if (PlayerStateManager.playerManager.favor >= 100 && PlayerController.instance.items.Find(item => item.item.GiveName() == "Old Woman's Pendant") == null)
        {
            PlayerStateManager.playerManager.FavorTransfer(-100);
            Instantiate(Resources.Load<GameObject>("Lootables/Old Woman's Pendant"), transform.position + new Vector3(1, -1), Quaternion.identity);
        }
        else
        {
            var neckActive = Instantiate(Resources.Load<GameObject>("Images/DamageTextPrefab"), PlayerController.instance.transform.position + new Vector3(0, .25f), Quaternion.identity);
            neckActive.GetComponentInChildren<TMPro.TMP_Text>().color = Color.white;
            neckActive.GetComponentInChildren<TMPro.TMP_Text>().text = "Pendant already active";
            Destroy(neckActive, 1.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { dialogueCanvas.enabled = true; }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        { dialogueCanvas.enabled = false; }

    }
}
