using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControler : MonoBehaviour
{
    GameManager gameManager;
    HudManager hudManager;

    PlayerInteract playerInteract;
    Player player;

    Inventory inventory;

    // UI REF
    InventoryUI inventoryUI;
    EquipmentUI equipmentUI;
    MerchantHUD merchantHUD;
    StorageChestUI storageChestUI;

    int horizontalInput;
    float verticalInput;

    public bool InputsBlocked { get; set; }
    public bool ControlBlocked { get; set; }
    public bool InventoryBlocked { get; set; }

    public InputState state;
    // Use this for initialization

    public enum InputState
    {
        PlayerControl,
        Equipment,
        StorageChest,
        Merchant,
        Blocked
    }

    private void Start()
    {
        gameManager = GameManager.Singleton;
        hudManager = HudManager.Singleton;

        inventoryUI = hudManager.inventoryUI;
        equipmentUI = hudManager.equipmentUI;
        merchantHUD = hudManager.merchantHUD;
        storageChestUI = hudManager.storageChestUI;

        player = gameManager.Player;
        playerInteract = player.GetComponent<PlayerInteract>();
        inventory = player.GetComponentInChildren<Inventory>();

        state = InputState.PlayerControl;
    }

    private void Update()
    {
        switch (state)
        {
            case InputState.PlayerControl:
                PlayerControl();
                break;
            case InputState.Equipment:
                EquipementNavigation();
                break;
            case InputState.StorageChest:
                StorageChestNavigation();
                break;
            case InputState.Blocked:
                break;
            case InputState.Merchant:
                MerchantNavigation();
                break;
            default:
                break;
        }
        DebugInputs();
    }

    private void PlayerControl()
    {
        if (!ControlBlocked)
        {
            horizontalInput = Mathf.FloorToInt(Input.GetAxis("Horizontal"));
            verticalInput = Mathf.FloorToInt(Input.GetAxis("Vertical"));

            if (horizontalInput != 0)
            {
                player.OnHorizontalInput(horizontalInput);
            }
            else
            {
                player.OnHorizontalInputUp();
            }

            if (verticalInput != 0f)
            {
                player.OnVerticalInput(verticalInput);
            }
            else
            {
                player.OnVerticalInputUp();
            }

            if (Input.GetButtonDown("Jump"))
            {
                //if (!playerInteract.CheckInteraction())
                //{
                //    player.OnJumpInputDown();
                //}

                player.OnJumpInputDown();
            }
            else if (Input.GetButtonDown("Dodge"))
            {
                player.OnDodgeInputDown();
            }
            else if (Input.GetButtonDown("Melee Attack"))
            {
                player.OnMeleeAttackInputDown();
            }
            else if (Input.GetButtonDown("Skill 1"))
            {
                player.OnSkillInputDown(0);
            }
            else if (Input.GetButtonDown("Skill 2"))
            {
                player.OnSkillInputDown(1);
            }

            // CHECK INTERACTIONS
            if (Input.GetButtonDown("Interact"))
            {
                playerInteract.CheckInteraction();
            }

            if (Input.GetButtonDown("Inventory"))
            {
                OpenEquipement();
            }
        }
    }

    private void EquipementNavigation()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.Close();
            equipmentUI.Close();
            hudManager.statsDisplayer.OpenClose();
            hudManager.ActivateBackground(false);
            hudManager.graphicRaycaster.enabled = false;
            state = InputState.PlayerControl;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (inventoryUI.IsFocus())
            {
                if (inventoryUI.HasSelectedSlot())
                {
                    inventoryUI.SwapItems();
                }
                else
                {
                    if (equipmentUI.HasSelectedSlot())
                    {
                        inventoryUI.SelectSlot(false);
                        equipmentUI.UnequipItem();
                    }
                    else
                    {
                        inventoryUI.SelectSlot(true);
                    }
                }
            }
            else if (equipmentUI.IsFocus())
            {
                if (equipmentUI.HasSelectedSlot())
                {
                    equipmentUI.SwapItems();
                }
                else
                {
                    if (inventoryUI.HasSelectedSlot())
                    {
                        equipmentUI.TryEquipItem(inventoryUI.GetSelectedItem(), inventoryUI.GetSelectedItemIndex(), inventoryUI.GetSelectedSlotIndex());
                    }
                    else
                    {
                        equipmentUI.TryToSelectSlot();

                    }
                }
            }
            else
            {
                if (inventoryUI.HasSelectedSlot())
                {
                    inventory.DropItem(hudManager.inventoryUI.GetSelectedItemIndex());
                }
                else if (equipmentUI.HasSelectedSlot())
                {
                    equipmentUI.DisableSelection();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (inventoryUI.HasSelectedSlot())
            {
                inventoryUI.DisableSelection();
                hudManager.itemHandler.ResetIcon();
            }
            else if (equipmentUI.HasSelectedSlot())
            {
                equipmentUI.DisableSelection();
            }
        }
    }

    private void StorageChestNavigation()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.Close();
            storageChestUI.Close();
            hudManager.ActivateBackground(false);
            hudManager.graphicRaycaster.enabled = false;
            state = InputState.PlayerControl;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (inventoryUI.IsFocus())
            {
                if (inventoryUI.HasSelectedSlot())
                {
                    inventoryUI.SwapItems();
                }
                else
                {
                    if (storageChestUI.HasSelectedSlot())
                    {
                        inventoryUI.SelectSlot(false);
                        //equipmentUI.UnequipItem();
                    }
                    else
                    {
                        inventoryUI.SelectSlot(true);
                    }
                }
            }
            else if (storageChestUI.IsFocus())
            {
                if (storageChestUI.HasSelectedSlot())
                {
                    storageChestUI.SwapItems();
                }
                else
                {
                    if (inventoryUI.HasSelectedSlot())
                    {
                        storageChestUI.SelectSlot(false);
                    }
                    else
                    {
                        storageChestUI.SelectSlot(true);
                    }
                }
            }
            else
            {
                //if (inventoryUI.HasSelectedSlot())
                //{
                //    inventory.DropItem(hudManager.inventoryUI.GetSelectedItemIndex());
                //}
                //else if (equipmentUI.HasSelectedSlot())
                //{
                //    equipmentUI.DisableSelection();
                //}
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (inventoryUI.HasSelectedSlot())
            {
                inventoryUI.DisableSelection();
                hudManager.itemHandler.ResetIcon();
            }
            else if (equipmentUI.HasSelectedSlot())
            {
                equipmentUI.DisableSelection();
            }
        }
    }

    private void MerchantNavigation()
    {
        if (Input.GetButtonDown("Inventory") || Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.Close();
            merchantHUD.Close();
            hudManager.ActivateBackground(false);
            hudManager.graphicRaycaster.enabled = false;
            state = InputState.PlayerControl;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (inventoryUI.IsFocus())
            {
                if (inventoryUI.HasSelectedSlot())
                {
                    inventoryUI.SwapItems();
                }
                else
                {
                    inventoryUI.SelectSlot(true);
                }
            }
            else if (merchantHUD.IsFocus())
            {

            }
            else
            {

            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (inventoryUI.IsFocus())
            {
                if (inventoryUI.HasSelectedSlot())
                {
                    inventoryUI.DisableSelection();
                    hudManager.itemHandler.ResetIcon();
                }
                else
                {
                    inventoryUI.SecondarySelectSlot();
                }
            }
            else if (merchantHUD.IsFocus())
            {
                merchantHUD.SelectSlot();
            }
        }
    }

    public void OpenEquipement()
    {
        inventoryUI.Open();
        equipmentUI.Open();
        hudManager.itemStatsPanel.SetPosition(false);
        hudManager.statsDisplayer.OpenClose();
        hudManager.ActivateBackground(true);

        hudManager.graphicRaycaster.enabled = true;
        state = InputState.Equipment;
        player.stats.velocity.x = 0;
    }

    public void OpenMerchant()
    {
        inventoryUI.Open();
        hudManager.itemStatsPanel.SetPosition(true);
        hudManager.ActivateBackground(true);
        hudManager.graphicRaycaster.enabled = true;
        state = InputState.Merchant;
        player.stats.velocity.x = 0;
    }

    public void OpenStorageChest()
    {
        inventoryUI.Open();
        storageChestUI.Open();
        hudManager.itemStatsPanel.SetPosition(false);
        hudManager.ActivateBackground(true);
        hudManager.graphicRaycaster.enabled = true;
        state = InputState.StorageChest;
        player.stats.velocity.x = 0;
    }

    private void DebugInputs()
    {
        inventory.CHEAT_UnlockInventorySlots();
    }
}

