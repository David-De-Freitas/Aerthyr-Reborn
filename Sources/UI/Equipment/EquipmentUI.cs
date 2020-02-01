using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] Transform weel;
    public List<EquipmentSlot> slots = new List<EquipmentSlot>();
    public EquipmentState state { get; set; }

    HudManager hudManager;

    InventoryUI inventoryUI;
    Inventory inventory;

    Player player;
    PlayerFight playerFight;

    CanvasGroup canvasGroup;
    Animator animator;

    int selectedSlotIndex;

    // ----------------------------------------------------------------------- **

    // Use this for initialization
    private void Start()
    {
        hudManager = HudManager.Singleton;
        inventoryUI = HudManager.Singleton.inventoryUI;
        inventory = GameManager.Singleton.playerInventory;

        player = GameManager.Singleton.Player;
        playerFight = player.GetComponent<PlayerFight>();

        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        canvasGroup.alpha = 0;

        state = EquipmentState.Close;
        InitWeelAndSlots();
        selectedSlotIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }

    // ----------------------------------------------------------------------- **

    void InitWeelAndSlots()
    {
        weel = transform.GetChild(0);

        for (int i = 0; i < weel.childCount; i++)
        {
            EquipmentSlot slot;
            slot = weel.GetChild(i).GetComponent<EquipmentSlot>();

            if (slot != null)
            {
                slots.Add(slot);
            }
        }
    }

    // ----------------------------------------------------------------------- **

    private void UpdateAnimation()
    {
        //state = (EquipmentState)inventoryUI.state;
        animator.SetInteger("State", (int)state);
    }

    // ----------------------------------------------------------------------- **

    void SetState(EquipmentState newState)
    {
        state = newState;
    }

    // ----------------------------------------------------------------------- **

    void UpdatePlayerStats(Item item, bool equip)
    {
        if (item is Item_DefensiveStats)
        {
            Item_DefensiveStats itemDef = item as Item_DefensiveStats;

            if (equip)
            {
                switch (itemDef.statAlteration.type)
                {
                    case DefensiveStatsAlteration.ModificationType.HealthMax:
                        player.stats.healthMax += itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.HealthRegen:
                        player.stats.healthRegen += itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.StaminaMax:
                        player.stats.staminaMax += itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.StaminaRegen:
                        player.stats.staminaRegen += itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.Armor:
                        player.stats.armor += itemDef.statAlteration.finalValue;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (itemDef.statAlteration.type)
                {
                    case DefensiveStatsAlteration.ModificationType.HealthMax:
                        player.stats.healthMax -= itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.HealthRegen:
                        player.stats.healthRegen -= itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.StaminaMax:
                        player.stats.staminaMax -= itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.StaminaRegen:
                        player.stats.staminaRegen -= itemDef.statAlteration.finalValue;
                        break;
                    case DefensiveStatsAlteration.ModificationType.Armor:
                        player.stats.armor -= itemDef.statAlteration.finalValue;
                        break;
                    default:
                        break;
                }

            }

            hudManager.statsDisplayer.UpdateStats();
        }
        else if (item is Item_OffensiveStats)
        {
            Item_OffensiveStats itemOff = item as Item_OffensiveStats;

            if (equip)
            {
                switch (itemOff.statAlteration.type)
                {
                    case OffensiveStatsAlteration.ModificationType.NormalDamage:
                        player.stats.normalDamages += itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CapacityDamage:
                        player.stats.capacitiesDamagesM += itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CooldownReduction:
                        player.stats.cooldownReduction += itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CriticalDamage:
                        player.stats.criticalDamagesM += itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CriticalChance:
                        player.stats.criticalChances += itemOff.statAlteration.finalValue;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (itemOff.statAlteration.type)
                {
                    case OffensiveStatsAlteration.ModificationType.NormalDamage:
                        player.stats.normalDamages -= itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CapacityDamage:
                        player.stats.capacitiesDamagesM -= itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CooldownReduction:
                        player.stats.cooldownReduction -= itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CriticalDamage:
                        player.stats.criticalDamagesM -= itemOff.statAlteration.finalValue;
                        break;
                    case OffensiveStatsAlteration.ModificationType.CriticalChance:
                        player.stats.criticalChances -= itemOff.statAlteration.finalValue;
                        break;
                    default:
                        break;
                }

            }

            hudManager.statsDisplayer.UpdateStats();
        }
    }

    // ----------------------------------------------------------------------- **

    public void Open()
    {
        if (state == EquipmentState.Close)
        {
            state = EquipmentState.Opening;
        }
        else if (state == EquipmentState.Closing)
        {
            state = EquipmentState.Opening;
        }
    }

    // ----------------------------------------------------------------------- **

    public void Close()
    {
        if (state == EquipmentState.Open)
        {
            state = EquipmentState.Closing;
        }
        else if (state == EquipmentState.Opening)
        {
            state = EquipmentState.Closing;
        }
    }

    // ----------------------------------------------------------------------- **

    public bool HasSelectedSlot()
    {
        return selectedSlotIndex != -1;
    }

    // ----------------------------------------------------------------------- **

    public bool IsFocus()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsHover)
            {
                return true;
            }
        }
        return false;
    }

    // ----------------------------------------------------------------------- **

    public void TryToSelectSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsHover && slots[i].item != null)
            {
                selectedSlotIndex = i;
                slots[i].EnableIconDisplay(false);
                hudManager.itemHandler.SetIcon(slots[i].GetIcon());
                break;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void EquipItemTo(Item _item, int slotIndex)
    {
        slots[slotIndex].AddItem(_item, slotIndex);
        UpdatePlayerStats(_item, true);
    }

    // ----------------------------------------------------------------------- **

    public void TryEquipItem(Item item, int itemIndex, int inventorySlotIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsHover)
            {
                if (item is Item_DefensiveStats && slots[i].type == EquipmentSlot.TypeOfSlot.Defensive)
                {
                    inventory.RemoveItemAt(itemIndex);

                    if (slots[i].item != null)
                    {
                        slots[i].item.isEquiped = false;
                        inventory.AddItem(slots[i].item, inventorySlotIndex);
                        UpdatePlayerStats(slots[i].item, false);
                    }

                    slots[i].AddItem(item, i);
                    slots[i].item.isEquiped = true;
                    slots[i].item.slotIndex = i;

                    UpdatePlayerStats(slots[i].item, true);
                    HudManager.Singleton.itemHandler.ResetIcon();
                }
                else if (item is Item_OffensiveStats && slots[i].type == EquipmentSlot.TypeOfSlot.Offensive)
                {

                    inventory.RemoveItemAt(itemIndex);

                    if (slots[i].item != null)
                    {
                        slots[i].item.isEquiped = false;
                        inventory.AddItem(slots[i].item, inventorySlotIndex);
                        UpdatePlayerStats(slots[i].item, false);
                    }

                    slots[i].AddItem(item, i);
                    slots[i].item.isEquiped = true;
                    slots[i].item.slotIndex = i;

                    UpdatePlayerStats(slots[i].item, true);
                    HudManager.Singleton.itemHandler.ResetIcon();

                }
                else if (item is Item_Skill && slots[i].type == EquipmentSlot.TypeOfSlot.Capacity)
                {
                    Item_Skill capacity = item as Item_Skill;

                    inventory.RemoveItemAt(itemIndex);

                    if (slots[i].item != null)
                    {
                        slots[i].item.isEquiped = false;
                        inventory.AddItem(slots[i].item, inventorySlotIndex);
                    }
                    // Set the skill
                    playerFight.skills[i] = capacity.GetSkill;

                    slots[i].AddItem(item, i);
                    slots[i].item.isEquiped = true;
                    slots[i].item.slotIndex = i;

                    HudManager.Singleton.itemHandler.ResetIcon();
                }

                break;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void UnequipItem()
    {
        Item selectedItem = inventoryUI.GetSelectedItem();

        if (selectedItem == null)
        {
            // Set the skill
            if (slots[selectedSlotIndex].item is Item_Skill)
            {
                playerFight.skills[selectedSlotIndex] = null;
            }

            slots[selectedSlotIndex].item.isEquiped = false;
            inventory.AddItem(slots[selectedSlotIndex].item, inventoryUI.GetSelectedSlotIndex());
            UpdatePlayerStats(slots[selectedSlotIndex].item, false);

            slots[selectedSlotIndex].ClearSlot();

            hudManager.itemHandler.ResetIcon();
            selectedSlotIndex = -1;
        }
        else
        {
            if (slots[selectedSlotIndex].item is Item_DefensiveStats)
            {
                if (selectedItem is Item_DefensiveStats)
                {
                    int inventorySlotIndex = inventoryUI.GetSelectedSlotIndex();

                    inventory.RemoveItemAt(inventoryUI.GetSelectedItemIndex());

                    UpdatePlayerStats(slots[selectedSlotIndex].item, false);
                    UpdatePlayerStats(selectedItem, true);

                    slots[selectedSlotIndex].item.isEquiped = false;
                    selectedItem.isEquiped = true;

                    inventory.AddItem(slots[selectedSlotIndex].item, inventorySlotIndex);
                    slots[selectedSlotIndex].AddItem(selectedItem, selectedSlotIndex);

                    selectedItem.slotIndex = selectedSlotIndex;

                    hudManager.itemHandler.ResetIcon();
                    selectedSlotIndex = -1;
                }
            }
            else if (slots[selectedSlotIndex].item is Item_OffensiveStats)
            {
                if (selectedItem is Item_OffensiveStats)
                {
                    int inventorySlotIndex = inventoryUI.GetSelectedSlotIndex();

                    inventory.RemoveItemAt(inventoryUI.GetSelectedItemIndex());

                    UpdatePlayerStats(slots[selectedSlotIndex].item, false);
                    UpdatePlayerStats(selectedItem, true);

                    slots[selectedSlotIndex].item.isEquiped = false;
                    selectedItem.isEquiped = true;

                    inventory.AddItem(slots[selectedSlotIndex].item, inventorySlotIndex);
                    slots[selectedSlotIndex].AddItem(selectedItem, selectedSlotIndex);

                    selectedItem.slotIndex = selectedSlotIndex;

                    hudManager.itemHandler.ResetIcon();
                    selectedSlotIndex = -1;
                }
            }
            else if (slots[selectedSlotIndex].item is Item_Skill)
            {
                if (selectedItem is Item_Skill)
                {
                    Item_Skill capacity = selectedItem as Item_Skill;

                    int inventorySlotIndex = inventoryUI.GetSelectedSlotIndex();

                    inventory.RemoveItemAt(inventoryUI.GetSelectedItemIndex());

                    slots[selectedSlotIndex].item.isEquiped = false;
                    selectedItem.isEquiped = true;

                    inventory.AddItem(slots[selectedSlotIndex].item, inventorySlotIndex);
                    slots[selectedSlotIndex].AddItem(selectedItem, selectedSlotIndex);

                    selectedItem.slotIndex = selectedSlotIndex;

                    // Set the skill
                    playerFight.skills[selectedSlotIndex] = capacity.GetSkill;

                    hudManager.itemHandler.ResetIcon();
                    selectedSlotIndex = -1;
                }
            }
        }

        inventoryUI.DisableSelection();
    }

    // ----------------------------------------------------------------------- **

    public void SwapItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsPointerHover())
            {
                if (slots[i] == slots[selectedSlotIndex])
                {
                    selectedSlotIndex = -1;
                    hudManager.itemHandler.ResetIcon();
                    slots[i].EnableIconDisplay(true);
                }
                else if (slots[i].type == slots[selectedSlotIndex].type)
                {
                    Item savedItem;
                    int savedItemIndex;

                    savedItem = slots[i].item;
                    savedItemIndex = slots[i].itemIndex;

                    slots[i].UpdateItem(slots[selectedSlotIndex].item, slots[selectedSlotIndex].itemIndex);
                    slots[selectedSlotIndex].UpdateItem(savedItem, savedItemIndex);

                    // Update the slot index in each items.
                    if (slots[i].item != null)
                    {
                        slots[i].item.slotIndex = i;
                    }
                    if (slots[selectedSlotIndex].item != null)
                    {
                        slots[selectedSlotIndex].item.slotIndex = selectedSlotIndex;
                    }

                    hudManager.itemHandler.ResetIcon();

                    selectedSlotIndex = -1;
                }

                break;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void DisableSelection()
    {
        if (selectedSlotIndex != -1)
        {
            hudManager.itemHandler.ResetIcon();
            slots[selectedSlotIndex].EnableIconDisplay(true);

            selectedSlotIndex = -1;
        }
    }

    // ----------------------------------------------------------------------- **

    public void SaveItems()
    {
        foreach (EquipmentSlot slot in slots)
        {
            if (slot.item != null)
            {
                slot.item.isSaved = true;
                slot.IconUpdate();
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void DestroyNonSavedItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item != null)
            {
                if (!slots[i].item.isSaved)
                {
                    UpdatePlayerStats(slots[i].item, false);
                    slots[i].ClearSlot();
                }
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public enum EquipmentState
    {
        Opening,
        Open,
        Closing,
        Close,
    }
}

[Serializable]
public class EquipmentSaveData
{
    public ItemDefensiveStatsSaveData[] itemDefensives;
    public ItemOffensiveStatsSaveData[] itemOffensives;
    public ItemCapacitySaveData[] itemCapacities;

    public EquipmentSaveData(EquipmentUI _equipment)
    {
        List<ItemDefensiveStatsSaveData> itemDefList = new List<ItemDefensiveStatsSaveData>();
        List<ItemOffensiveStatsSaveData> itemOffList = new List<ItemOffensiveStatsSaveData>();
        List<ItemCapacitySaveData> itemCapList = new List<ItemCapacitySaveData>();

        foreach (EquipmentSlot slot in _equipment.slots)
        {
            Item item = slot.item;
            if (item != null)
            {
                if (item is Item_DefensiveStats)
                {
                    Item_DefensiveStats itemDef = item as Item_DefensiveStats;
                    itemDefList.Add(itemDef.ToSaveData());
                }
                else if (item is Item_OffensiveStats)
                {
                    Item_OffensiveStats itemOff = item as Item_OffensiveStats;
                    itemOffList.Add(itemOff.ToSaveData());
                }
                else if (item is Item_Skill)
                {
                    Item_Skill itemOff = item as Item_Skill;
                    itemCapList.Add(itemOff.ToSaveData());
                }
            }
        }

        itemDefensives = itemDefList.ToArray();
        itemOffensives = itemOffList.ToArray();
        itemCapacities = itemCapList.ToArray();
    }

    public void LoadToEquipment(EquipmentUI _equipment, ItemsDataBase _itemsDataBase)
    {

        foreach (ItemDefensiveStatsSaveData itemData in itemDefensives)
        {
            Item toAdd = LoadDefensiveItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _equipment.EquipItemTo(toAdd, toAdd.slotIndex);
            }
        }

        foreach (ItemOffensiveStatsSaveData itemData in itemOffensives)
        {
            Item toAdd = LoadOffensiveItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _equipment.EquipItemTo(toAdd, toAdd.slotIndex);
            }
        }

        foreach (ItemCapacitySaveData itemData in itemCapacities)
        {
            Item toAdd = LoadCapacityItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _equipment.EquipItemTo(toAdd, toAdd.slotIndex);
            }
        }
    }

    private Item LoadDefensiveItem(ItemDefensiveStatsSaveData _data, ItemsDataBase _itemsDataBase)
    {
        DefensiveStatsAlteration.ModificationType statType = (DefensiveStatsAlteration.ModificationType)_data.statType;
        Item defItem = null;

        switch (statType)
        {
            case DefensiveStatsAlteration.ModificationType.HealthMax:
                defItem = _itemsDataBase.hpMax.FromSaveData(_data);
                break;
            case DefensiveStatsAlteration.ModificationType.HealthRegen:
                defItem = _itemsDataBase.hpRegeneration.FromSaveData(_data);
                break;
            case DefensiveStatsAlteration.ModificationType.StaminaMax:
                defItem = _itemsDataBase.staminaMax.FromSaveData(_data);
                break;
            case DefensiveStatsAlteration.ModificationType.StaminaRegen:
                defItem = _itemsDataBase.staminaRegeneration.FromSaveData(_data);
                break;
            case DefensiveStatsAlteration.ModificationType.Armor:
                defItem = _itemsDataBase.armor.FromSaveData(_data);
                break;
            default:
                break;
        }

        return defItem;
    }

    private Item LoadOffensiveItem(ItemOffensiveStatsSaveData _data, ItemsDataBase _itemsDataBase)
    {
        OffensiveStatsAlteration.ModificationType statType = (OffensiveStatsAlteration.ModificationType)_data.statType;
        Item offItem = null;

        switch (statType)
        {
            case OffensiveStatsAlteration.ModificationType.NormalDamage:
                offItem = _itemsDataBase.normalDamages.FromSaveData(_data);
                break;
            case OffensiveStatsAlteration.ModificationType.CapacityDamage:
                offItem = _itemsDataBase.capacitiesDamages.FromSaveData(_data);
                break;
            case OffensiveStatsAlteration.ModificationType.CooldownReduction:
                offItem = _itemsDataBase.cooldownReduction.FromSaveData(_data);
                break;
            case OffensiveStatsAlteration.ModificationType.CriticalDamage:
                offItem = _itemsDataBase.critDamages.FromSaveData(_data);
                break;
            case OffensiveStatsAlteration.ModificationType.CriticalChance:
                offItem = _itemsDataBase.critChances.FromSaveData(_data);
                break;
            default:
                break;
        }

        return offItem;
    }

    private Item LoadCapacityItem(ItemCapacitySaveData _data, ItemsDataBase _itemsDataBase)
    {
        Item skillItem = null;

        switch (_data.skillType)
        {
            case Item_Skill.SkillType.Fury:
                skillItem = _itemsDataBase.fury.FromSaveData(_data);
                break;
            case Item_Skill.SkillType.Shield:
                skillItem = _itemsDataBase.shield.FromSaveData(_data);
                break;
            case Item_Skill.SkillType.SonicBoom:
                skillItem = _itemsDataBase.sonicBoom.FromSaveData(_data);
                break;
            default:
                break;
        }

        return skillItem;
    }
}