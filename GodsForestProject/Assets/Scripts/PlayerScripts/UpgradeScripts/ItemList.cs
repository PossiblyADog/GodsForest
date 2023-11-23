using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemList
{
    public Item item;
    public string name;
    public int itemStacks = 1;

    public ItemList(Item item, string name)
    {
        this.item = item;
        this.name = name;
    }
}
