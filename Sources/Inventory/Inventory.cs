using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Info")]
    const int minListItem = 5;
    const int maxListItem = 25;
    public int slotsAvailable = 10;
    [Space]
    [SerializeField] int moneyAmount;
    [Space]
    [Header("Inventory List")]
    public List<Item> items = new List<Item>();

    public int MinItemNumber { get { return minListItem; } }
    public int MaxItemNumber {  get { return maxListItem; } }

    public int MoneyAmount { get { return moneyAmount; } }

    HudManager hudManager;
    InventoryUI inventoryUI;
    Transform playerCenter;

    private void Start()
    {
        hudManager = HudManager.Singleton;
        inventoryUI = HudManager.Singleton.inventoryUI;
        playerCenter = GameManager.Singleton.Player.centerTransform;
    }

    public bool CanAddItem()
    {
        if (items.Count < slotsAvailable)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void AddItem(Item item)
    {
        int itemIndex = -1;

        if (items.Count > 0)
        {
            items.FillAdd(item, ref itemIndex);
        }
        else
        {
            items.Add(item);
            itemIndex = 0;
        }

        inventoryUI.AddItem(item, itemIndex);
    }

    public void AddItem(Item item, int slotIndex)
    {
        int itemIndex = -1;

        if (items.Count > 0)
        {
            items.FillAdd(item, ref itemIndex);
        }
        else
        {
            items.Add(item);
            itemIndex = 0;
        }

        inventoryUI.AddItem(item, itemIndex, slotIndex);
    }

    public void RemoveItemAt(int index)
    {
        inventoryUI.ClearSlotWithItemIndex(index);
        items[index] = null;
    }

    public void DropItem(int index)
    {
        GameObject itemGO = new GameObject();

        items[index].slotIndex = -1;

        WorldItem worldItem = itemGO.AddComponent<WorldItem>();
        worldItem.item = items[index];

        itemGO.transform.position = playerCenter.position;

        inventoryUI.ClearSlotWithItemIndex(index);
        items[index] = null;

        hudManager.itemHandler.ResetIcon();
    }

    public void AddMoney(int amount)
    {
        moneyAmount += amount;
        inventoryUI.UpdateMoney();
    }

    public void RemoveMoney(int amount)
    {
        moneyAmount -= amount;
        if (moneyAmount < 0)
        {
            moneyAmount = 0;
        }
        inventoryUI.UpdateMoney();
    }


    public void SaveItems()
    {
        foreach (Item item in items)
        {
            if (item != null)
            {
                item.isSaved = true;
            }
        }

        HudManager.Singleton.inventoryUI.IconsUpdate();
    }

    public void DestroyNonSavedItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                if (!items[i].isSaved)
                {             
                    RemoveItemAt(i);
                }
            }
        }
    }

    public void CHEAT_UnlockInventorySlots()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (slotsAvailable < maxListItem)
            {
                slotsAvailable += minListItem;
                inventoryUI.AddSlotsLines();
            }
        }
    }

    public void UnlockInventorySlots(int lineAmount)
    {
        for (int i = 0; i < lineAmount; i++)
        {
            if (slotsAvailable >= maxListItem)
            {
                break;
            }

            slotsAvailable += minListItem;
            inventoryUI.AddSlotsLines();
        }
    }
}

[Serializable]
public class InventorySaveData
{
    public int moneyAmount;
    public int slotAvailable;
    public ItemDefensiveStatsSaveData[] itemDefensives;
    public ItemOffensiveStatsSaveData[] itemOffensives;
    public ItemCapacitySaveData[] itemCapacities;

    public InventorySaveData(Inventory _inventory)
    {
        moneyAmount = _inventory.MoneyAmount;
        slotAvailable = _inventory.slotsAvailable;

        List<ItemDefensiveStatsSaveData> itemDefList = new List<ItemDefensiveStatsSaveData>();
        List<ItemOffensiveStatsSaveData> itemOffList = new List<ItemOffensiveStatsSaveData>();
        List<ItemCapacitySaveData> itemCapList = new List<ItemCapacitySaveData>();

        foreach (Item item in _inventory.items)
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

        itemDefensives = itemDefList.ToArray();
        itemOffensives = itemOffList.ToArray();
        itemCapacities = itemCapList.ToArray();
    }

    public void LoadToInventory(Inventory _inventory, ItemsDataBase _itemsDataBase)
    {
        _inventory.AddMoney(moneyAmount);

        int lineToAdd = ((slotAvailable - _inventory.slotsAvailable) / _inventory.MinItemNumber);
        _inventory.UnlockInventorySlots(lineToAdd);

        foreach (ItemDefensiveStatsSaveData itemData in itemDefensives)
        {
            Item toAdd = LoadDefensiveItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _inventory.AddItem(toAdd, toAdd.slotIndex);
            }        
        }

        foreach (ItemOffensiveStatsSaveData itemData in itemOffensives)
        {
            Item toAdd = LoadOffensiveItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _inventory.AddItem(toAdd, toAdd.slotIndex);
            }
        }

        foreach (ItemCapacitySaveData itemData in itemCapacities)
        {
            Item toAdd = LoadCapacityItem(itemData, _itemsDataBase);
            if (toAdd != null && toAdd.isSaved)
            {
                _inventory.AddItem(toAdd, toAdd.slotIndex);
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
