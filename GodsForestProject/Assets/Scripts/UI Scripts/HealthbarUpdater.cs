using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthbarUpdater : MonoBehaviour
{
    public static HealthbarUpdater instance;

    private Slider barSlider;

    private TMP_Text hpText;

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

        barSlider = GetComponentInChildren<Slider>();
        hpText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        ResetHP();
        PlayerStateManager.playerManager.FavorTransfer(0);
    }

    internal void SetMaxHP(int maxHP)
    {
        barSlider.maxValue = maxHP;
        SetHPText();
    }

    internal void SetCurrentHP(int hp)
    {
        barSlider.value = hp;
        SetHPText();
    }

    public void SetHPText()
    {
        hpText.text = (PlayerStateManager.playerManager.currentHP + "/" + PlayerStateManager.playerManager.maxHP);
    }

    public void ResetHP()
    {
        barSlider.maxValue = PlayerStateManager.playerManager.maxHP;
        barSlider.value = PlayerStateManager.playerManager.currentHP;
        hpText.text = (PlayerStateManager.playerManager.currentHP + "/" + PlayerStateManager.playerManager.maxHP);
    }

}
