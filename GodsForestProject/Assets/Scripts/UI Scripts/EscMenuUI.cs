using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscMenuUI : MonoBehaviour
{
    public static EscMenuUI instance;
    private bool isActive = false;
    private PlayerInputActions playerActions;
    private GameObject escMenu;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            escMenu = transform.GetChild(1).GetComponent<RectTransform>().gameObject;
            //transform.GetChild(1).GetComponentInChildren<UpgradeUI>().InitializeUpgradeUI(28);
            playerActions = new PlayerInputActions();
            playerActions.Player.Enable();
            playerActions.Player.EscMenu.performed += ToggleUI;
        }
    }
    public void ToggleUI(InputAction.CallbackContext context)
    {
        if (this != null)
        {
            isActive = !isActive;
            Cursor.visible = isActive;
            escMenu.SetActive(isActive);

            if (!isActive)
            {
                if (GameManager.instance.SettingActive)
                {
                    GameManager.instance.ToggleSettings();
                }
            }
            else
            {
                try
                {

                    if (PlayerController.instance.items.Count >= 0)
                    {
                        UpgradeUI.instance.ResetData();

                        foreach (var itemData in PlayerController.instance.items)
                        {
                            UpgradeUI.instance.AddItemToUI(itemData);
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }

    public void ToggleUIManual()
    {
        if (this != null)
        {
            isActive = !isActive;
            escMenu.SetActive(isActive);

            if (isActive)
            {
                try
                {
                    UpgradeUI.instance.ResetData();

                    foreach (var itemData in PlayerController.instance.items)
                    {
                        UpgradeUI.instance.AddItemToUI(itemData);
                    }
                }
                catch
                {

                }
            }
        }
    }

    public bool IsActive
    {
        get { return isActive; }
    }
}
