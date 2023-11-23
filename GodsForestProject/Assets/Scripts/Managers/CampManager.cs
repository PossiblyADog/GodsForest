using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampManager : MonoBehaviour
{
    public List<GameObject> expansionTiles, expansionObjects;

    void Start()
    {
        for(int i = 0; i < GameManager.instance.expandUnlocked.Length; i++)
        {
            if (GameManager.instance.expandUnlocked[i] == true)
            {
                expansionTiles[i].SetActive(true);
                expansionObjects[i].SetActive(true);
            }
        }
    }

}
