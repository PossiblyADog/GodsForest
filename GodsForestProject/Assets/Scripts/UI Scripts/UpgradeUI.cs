using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{

    [SerializeField]
    private GameObject upgradePrefab;

    [SerializeField]
    private RectTransform contentPanel;


    //List<GameObject> upgradeList = new List<GameObject> ();

    public static UpgradeUI instance;

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
    public void AddItemToUI(ItemList itemToAdd)
    {
        var itemUIObject = Instantiate(upgradePrefab);
        itemUIObject.transform.SetParent (contentPanel);
        itemUIObject.GetComponent<UpgradeObjectUI>().SetItemData(itemToAdd.name, itemToAdd.item.GiveDescription(), itemToAdd.item.ItemImage(), itemToAdd.itemStacks);

    }



    public void ResetData()
    {
        if (contentPanel.childCount > 0)
        {
            for (int i = 0; i < contentPanel.childCount; i++)
            {
                Destroy(contentPanel.GetChild(i).gameObject);
            }
        }       
    }
}
