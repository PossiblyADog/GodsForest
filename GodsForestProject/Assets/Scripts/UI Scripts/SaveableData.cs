using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveableData 
{
    public int[] upgradeLevels = new int[6];
    public bool[] expansionChecks = new bool[8];
    public int[] volumeData = new int[4];
    public string[] lvlOneQuests = new string[4];
    public string[] lvlTwoQuests = new string[4];
    public string[] lvlThreeQuests = new string[4];
    public int currentFavor;
    public float[] savedCursorData = new float[5];
    public SaveableData(PlayerStateManager playerManager, GameManager gameM)
    {
        upgradeLevels[0] = playerManager.maxHPLevel;
        upgradeLevels[1] = playerManager.healAmountLevel;
        upgradeLevels[2] = playerManager.armorUpLevel;
        upgradeLevels[3] = playerManager.damageUpLevel;
        upgradeLevels[4] = playerManager.speedUpLevel;
        upgradeLevels[5] = playerManager.favorUpLevel;

        currentFavor = playerManager.favor;

        expansionChecks[0] = gameM.expandUnlocked[0];
        expansionChecks[1] = gameM.expandUnlocked[1];
        expansionChecks[2] = gameM.expandUnlocked[2];
        expansionChecks[3] = gameM.expandUnlocked[3];
        expansionChecks[4] = gameM.expandUnlocked[4];
        expansionChecks[5] = gameM.expandUnlocked[5];
        expansionChecks[6] = gameM.expandUnlocked[6];
        expansionChecks[7] = gameM.expandUnlocked[7];

        volumeData[0] = Mathf.RoundToInt(gameM.masterVol.value);
        volumeData[1] = Mathf.RoundToInt(gameM.playerVol.value);
        volumeData[2] = Mathf.RoundToInt(gameM.enemyVol.value);
        volumeData[3] = Mathf.RoundToInt(gameM.musicVol.value);

        savedCursorData = gameM.cursorInfo;

        if (GameManager.instance.levelOneQuests.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.levelOneQuests.Count; i++)
            {
                if (GameManager.instance.levelOneQuests[i] != null)
                {
                    lvlOneQuests[i] = GameManager.instance.levelOneQuests[i].name;
                }
            }
        }


        if (GameManager.instance.levelTwoQuests.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.levelTwoQuests.Count; i++)
            {
                if (GameManager.instance.levelTwoQuests[i] != null)
                {
                    lvlTwoQuests[i] = GameManager.instance.levelTwoQuests[i].name;
                }
            }
        }


        if (GameManager.instance.levelThreeQuests.Count > 0)
        {
            for (int i = 0; i < GameManager.instance.levelThreeQuests.Count; i++)
            {
                if (GameManager.instance.levelThreeQuests[i] != null)
                {
                    lvlThreeQuests[i] = GameManager.instance.levelThreeQuests[i].name;
                }
            }
        }
    }
}
