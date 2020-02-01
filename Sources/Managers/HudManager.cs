using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    // Singleton set up
    private static HudManager singleton = null;
    public static HudManager Singleton
    {
        get
        {
            return singleton;
        }
    }

    public GraphicRaycaster graphicRaycaster;
    [Space]
    public PlayerInfoUI playerInfoUI;
    public InventoryUI inventoryUI;
    public EquipmentUI equipmentUI;
    public StorageChestUI storageChestUI;
    [Space]
    public ItemHandler itemHandler;
    public ItemActionMenu itemActionMenu;
    [Space]
    public HUDItemStats itemStatsPanel;
    public PlayerStatsDisplayer statsDisplayer;
    [Space]
    public MerchantHUD merchantHUD;
    [Space]
    public Image backgroundHUD;

    Animator backgroundHUD_Animator;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this);
        }
        else if(singleton != this)
        {
            Destroy(gameObject);
        }

        backgroundHUD_Animator = backgroundHUD.GetComponent<Animator>();

        if (graphicRaycaster == null)
        {
            graphicRaycaster = GetComponentInChildren<GraphicRaycaster>();
        }
    }

    public void ActivateBackground(bool _state)
    {
        string animStr = _state ? "open" : "close";
        backgroundHUD_Animator.Play(animStr);
    }
}
