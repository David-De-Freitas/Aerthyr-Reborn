using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill Item", menuName = "Data/Item/Skill")]
public class Item_Skill : Item
{
    public enum SkillType
    {
        Fury, Shield, SonicBoom
    }

    public SkillType skillType;
    [Space]
    public Skills skill;
    public Skills GetSkill { get { return skill; } }
    [Space]

    [SerializeField]
    ItemRarity[] rarities = new ItemRarity[4]
    {
        new ItemRarity("Commun", 86.7f),
         new ItemRarity("Rare", 10f),
         new ItemRarity("Epique", 3f),
        new ItemRarity("Legendaire", 0.3f)
    };

    private int rarityIndex = -1;

    public override Item CreateNewInstance()
    {
        Item_Skill instance;

        instance = Instantiate(this) as Item_Skill;

        UpdateRarity(ref instance);
        return instance;
    }

    void UpdateRarity(ref Item_Skill item)
    {
        float maxValue = 100;
        foreach (ItemRarity currentRarity in item.rarities)
        {
            currentRarity.SetRarityBounds(ref maxValue);
        }
        item.rarityIndex = 0;
        float rarityRoll = Random.Range(0, 10001) / 100f;
        foreach (ItemRarity currentRarity in item.rarities)
        {
            if (rarityRoll >= currentRarity.minValue && rarityRoll <= currentRarity.maxValue)
            {
                item.name += " (" + currentRarity.rarityName + ")";

                item.skill = Instantiate(currentRarity.skill) as Skills;
                SetItemPrices(ref item, currentRarity);
                break;
            }
            item.rarityIndex++;
        }
    }

    public void SetItemPrices(ref Item_Skill item, ItemRarity rarity)
    {
        item.sellPrice = rarity.sellPrice;
        item.buyPrice = rarity.buyPrice;
    }

    #region SAVE

    /// <summary>
    /// Copy the current item to a saveable item.
    /// </summary>
    /// <returns></returns>
    public ItemCapacitySaveData ToSaveData()
    {
        ItemCapacitySaveData data;
        data = new ItemCapacitySaveData();

        data.name = name;
        data.skillType = skillType;
        data.rarityIndex = rarityIndex;

        data.itemType = (int)type;

        data.isSaved = isSaved ? 1 : 0;
        data.isEquiped = isEquiped ? 1 : 0;

        data.slotIndex = slotIndex;

        data.sellPrice = sellPrice;
        data.buyPrice = buyPrice;

        return data;
    }

    /// <summary>
    /// Crete an item based on the datas of a saved item.
    /// </summary>
    /// <param name="_data"></param>
    public Item FromSaveData(ItemCapacitySaveData _data)
    {
        Item_Skill instance;
        instance = Instantiate(this) as Item_Skill;

        instance.name = _data.name;
        instance.skillType = _data.skillType;
        instance.rarityIndex = _data.rarityIndex;

        instance.isSaved = _data.isSaved == 1;
        instance.isEquiped = _data.isEquiped == 1;

        instance.slotIndex = _data.slotIndex;

        instance.skill = Instantiate(rarities[_data.rarityIndex].skill) as Skills;

        instance.sellPrice = _data.sellPrice;
        instance.buyPrice = _data.buyPrice;

        return instance;
    }

    #endregion

    [System.Serializable]
    public class ItemRarity
    {
        public string rarityName;
        public float rarityValue;
        [Space]
        public Skills skill;
        [Header("_________")]
        [Space]
        public int sellPrice;
        public int buyPrice;

        public float minValue { get; set; }
        public float maxValue { get; set; }

        public ItemRarity(string name, float value)
        {
            rarityName = name;
            rarityValue = value;
        }

        public void SetRarityBounds(ref float currentMaxValue)
        {
            maxValue = currentMaxValue;
            minValue = maxValue - rarityValue;
            currentMaxValue = minValue;
        }
    }
}

[System.Serializable]
public class ItemCapacitySaveData
{
    public string name;

    public Item_Skill.SkillType skillType;
    public int rarityIndex;

    public int itemType;

    public int isSaved;
    public int isEquiped;

    public int slotIndex;

    public int sellPrice;
    public int buyPrice;

    public override string ToString()
    {
        string str = "";

        str += "Name : " + name + "\n";

        str += "isSaved : " + (isSaved == 1 ? "true" : "false") + "\n";
        str += "isEquiped : " + (isEquiped == 1 ? "true" : "false") + "\n";

        str += "sell price : " + sellPrice + "\n";
        str += "buy price : " + buyPrice + "\n";
        return str;
    }
}