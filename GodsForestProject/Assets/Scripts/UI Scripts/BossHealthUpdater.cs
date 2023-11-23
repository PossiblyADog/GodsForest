using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BossHealthUpdater : MonoBehaviour
{
    public static BossHealthUpdater instance;

    public Slider barSlider;

    public TMP_Text currentName;

    public GameObject bossBar;

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

    public void SetBossBar(string bossName, int maxHP)
    {
        SetMaxHP(maxHP);
        SetCurrentHP(maxHP);
        currentName.text = bossName;    
    }

    internal void SetMaxHP(int maxHP)
    {
        barSlider.maxValue = maxHP;
    }

    public void SetCurrentHP(int hp)
    {
        barSlider.value = hp;
    }

    public void Switch()
    {
        bossBar.SetActive(!bossBar.activeSelf);
    }

}