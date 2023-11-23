using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCruncherController : MonoBehaviour
{
    public GameObject cruncherCanvas;
    public GameObject itemButton;
    public GameObject layoutPanel;
    public ChildDestroyer killer;
    public int charges;

    private void Awake()
    {
        if (charges == 0)
        { charges = 2; }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && charges > 0)
        {
            cruncherCanvas.SetActive(true);
            PopulateButtonList();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && cruncherCanvas.activeSelf == true)
        {
            killer.KillTheChildren();
            cruncherCanvas.SetActive(false); 
        }
    }

    public void ChargesUsed()
    {
        cruncherCanvas.SetActive(false);
    }

    private void PopulateButtonList()
    {
        for (int i = 0; i < GameManager.instance.allItems.Length; i++)
        {
            var newButton = Instantiate(itemButton, layoutPanel.transform);
            newButton.GetComponent<ItemCruncherButton>().SetButtonData(GameManager.instance.allItems[i], this);
        }
    }

    public void RefreshOnClick()
    {
        killer.KillTheChildren();
        PopulateButtonList();
    }
}
