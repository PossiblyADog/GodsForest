using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{

    public static TooltipManager instance;
    private TMP_Text upgradeName, description;
    private UpgradeObjectUI tempHolder;
    private bool isShowing = false;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;    
        }
        upgradeName = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        description = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        transform.GetChild(0).gameObject.SetActive(false);
    }
    private void Update()
    {
        if(EscMenuUI.instance.IsActive)
        {
            transform.position = Input.mousePosition + new Vector3(20, 0);

            if (!isShowing)
            {
                if (IsOverItemUIElement())
                {
                    ShowTooltip(tempHolder);
                }
            }
            else if (!IsOverItemUIElement())
            {
                if (isShowing)
                {
                    HideTooltip();
                }
            }
            else
            {

            }
        }
        else if (isShowing)
        {
            HideTooltip();
        }

        
    }

    private bool IsOverItemUIElement()
    {
        bool hoveringItem = false;
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position =  Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.GetComponent<UpgradeObjectUI>() != null)
            {
                hoveringItem = true;
                tempHolder = results[i].gameObject.GetComponent<UpgradeObjectUI>();
            }
        }

        return hoveringItem;
    }

    public void ShowTooltip(UpgradeObjectUI hoveredUpgrade)
    {
        isShowing = true;
        transform.GetChild(0).gameObject.SetActive(true);
        upgradeName.text = hoveredUpgrade.itemName; 
        description.text = hoveredUpgrade.itemDescription;
    }

    public void HideTooltip()
    {

        transform.GetChild(0).gameObject.SetActive(false);
        upgradeName.text = "";
        description.text = "";
        isShowing=false;
    }
}
