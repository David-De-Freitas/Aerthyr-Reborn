using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemActionMenu : MonoBehaviour
{
    [SerializeField] Transform inMerchantTransform;
    [SerializeField] Transform inInventoryTransform;
    [Space]
    [SerializeField] Text mainButtonText;
    bool canUpdate;
    MainAction mainAction;
    CanvasGroup canvasGroup;

    HudManager hudManager;
    InventoryUI inventoryUI;
    MerchantHUD merchantHUD;

    public enum MainAction
    {
        Sell,
        Buy
    }

    private void Start()
    {
        hudManager = HudManager.Singleton;

        inventoryUI = hudManager.inventoryUI;
        merchantHUD = hudManager.merchantHUD;

        canvasGroup = GetComponent<CanvasGroup>();

        SetActive(false);
    }

    public void OnSellBuyButtonClic()
    {
        if (mainAction == MainAction.Sell)
        {
            inventoryUI.SellItem();
            merchantHUD.AllowSlotsUpdate(true);
        }
        else
        {
            if (!inventoryUI.BuyItem(merchantHUD.GetSelectedItem()))
            {

            }
            else
            {
                merchantHUD.RemoveSelectedItem();
            }

            SetActive(false);
            merchantHUD.AllowSlotsUpdate(true);
            inventoryUI.AllowSlotsUpdate(true);
        }
    }

    public void OnCancelButtonClic()
    {
        if (mainAction == MainAction.Sell)
        {
            inventoryUI.DisableSelection();          
        }
        else
        {
            merchantHUD.DisableSelection();
        }

        inventoryUI.AllowSlotsUpdate(true);
        merchantHUD.AllowSlotsUpdate(true);
        SetActive(false);
    }

    public void SetActive(bool state)
    {
        if (state)
        {
            canvasGroup.alpha = 1;
            canUpdate = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canUpdate = false;
        }
    }

    public void SetState(MainAction action)
    {
        mainAction = action;

        if (action == MainAction.Buy)
        {
            transform.position = inMerchantTransform.position;
            mainButtonText.text = "ACHETER";
        }
        else
        {
            transform.position = inInventoryTransform.position;
            mainButtonText.text = "VENDRE";
        }
    }

}
