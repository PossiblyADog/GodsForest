using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private Slider masterVol, playerVol, enemyVol, musicVol;

    public void ApplySettings()
    {
        GameManager.instance.sfxVolume = Mathf.RoundToInt(playerVol.value * masterVol.value / 100f);
        GameManager.instance.enemyVolume = Mathf.RoundToInt(enemyVol.value * masterVol.value / 100f);
        GameManager.instance.bgmVolume = Mathf.RoundToInt(musicVol.value * masterVol.value / 100f);
    }
    
}
