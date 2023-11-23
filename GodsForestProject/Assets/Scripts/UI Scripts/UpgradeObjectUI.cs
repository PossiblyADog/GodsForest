
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


[System.Serializable]
public class UpgradeObjectUI : MonoBehaviour
{
    public Image itemImage;
    public string itemName, itemDescription;
    public TMP_Text stackText;


    private void Awake()
    {
        itemImage = GetComponent<Image>();
    }
    public void SetItemData(string name, string description, Sprite sprite, int stacks)
    {
        itemName = name;
        itemDescription = description;
        itemImage.sprite = sprite;
        stackText.text = stacks.ToString();
           
        
    }

}
