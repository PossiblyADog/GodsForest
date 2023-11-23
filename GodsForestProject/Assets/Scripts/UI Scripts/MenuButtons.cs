using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public GameObject creditsUI, tutorialUI, endRunPanel;
    public void SaveAndExit()
    {
        GameManager.instance.SaveGame();
        GameManager.instance.MainMenuLevelLoad();
    }

    public void LoadSave()
    {
        GameManager.instance.LoadGame();
    }

    public void NewGame()
    {
        GameManager.instance.CampLevelLoad();
        PlayerStateManager.playerManager.FavorTransfer(-PlayerStateManager.playerManager.favor);
        try
        {
            PlayerStateManager.playerManager.ResetUpgrades();
            PlayerController.instance.SummonTheReaper();
        }
        catch
        {

        }
    }

    public void ToggleCredits()
    {
        creditsUI.SetActive(!creditsUI.activeSelf);
    }

    public void ToggleTutorial()
    {
        tutorialUI.SetActive(!tutorialUI.activeSelf);
    }

    public void ToggleSettingsPanel()
    {
        GameManager.instance.ToggleSettings();
    }
    public void CloseGame()
    {
        GameManager.instance.CloseApplication();
    }

    public void ToggleEndRunPanel()
    {
        endRunPanel.SetActive(!endRunPanel.activeSelf);
    }

    public void EndRun()
    {
        PlayerController.instance.SummonTheReaper();
    }
}
