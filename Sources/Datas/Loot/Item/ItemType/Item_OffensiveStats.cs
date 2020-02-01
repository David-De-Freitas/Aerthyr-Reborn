using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item/Stats/Offensive")]
public class Item_OffensiveStats : Item
{
    [Space]
    public string rarityStr;
    public OffensiveStatsAlteration statAlteration;
    [Space]
    [Header("RARITIES")]
    public RaritiesPerGameDifficuty[] raritiesPerDifficulty = new RaritiesPerGameDifficuty[(int)GameDifficulty._COUNT];
    public ItemRarityData[] raritiesDatas = new ItemRarityData[(int)ItemRarity._COUNT];

    public override Item CreateNewInstance()
    {
        Item_OffensiveStats instance;
        instance = Instantiate(this) as Item_OffensiveStats;

        UpdateRarity(ref instance);

        return instance;
    }

    void UpdateRarity(ref Item_OffensiveStats item)
    {
        GameDifficulty _gameDifficulty = GameManager.Singleton.gameData.GetGameDifficulty();

        float maxValue = 100;
        foreach (RaritiesPerGameDifficuty.RarityData currentRarity in item.raritiesPerDifficulty[(int)_gameDifficulty].rarities)
        {
            currentRarity.SetRarityBounds(ref maxValue);
        }

        float rarityRoll = Random.Range(0, 10001) / 100f;

        foreach (RaritiesPerGameDifficuty.RarityData currentRarity in item.raritiesPerDifficulty[(int)_gameDifficulty].rarities)
        {
            if (rarityRoll >= currentRarity.minValue && rarityRoll <= currentRarity.maxValue)
            {
                ItemRarityData rarityData = raritiesDatas[(int)currentRarity.itemRarity];

                item.rarityStr = rarityData.name;
                item.name += " (" + item.rarityStr + ")";

                rarityData.statAlteration.GenerateFinalValue();

                item.statAlteration = rarityData.statAlteration;
                SetItemPrices(ref item, rarityData);
                break;
            }
        }
    }

    public void SetItemPrices(ref Item_OffensiveStats item, ItemRarityData rarity)
    {
        float sellPriceGap;
        float buyPriceGap;

        sellPriceGap = rarity.maxSellPrice - rarity.minSellPrice;
        buyPriceGap = rarity.maxBuyPrice - rarity.minBuyPrice;

        item.sellPrice = Mathf.FloorToInt(rarity.minSellPrice + (sellPriceGap * item.statAlteration.finalValue) / item.statAlteration.maxValue);
        item.buyPrice = Mathf.CeilToInt(rarity.minBuyPrice + (buyPriceGap * item.statAlteration.finalValue) / item.statAlteration.maxValue);
    }

    #region SAVE

    /// <summary>
    /// Copy the current item to a saveable item.
    /// </summary>
    /// <returns></returns>
    public ItemOffensiveStatsSaveData ToSaveData()
    {
        ItemOffensiveStatsSaveData data;
        data = new ItemOffensiveStatsSaveData();

        data.name = name;
        data.rarity = rarityStr;

        data.itemType = (int)type;

        data.isSaved = isSaved ? 1 : 0;
        data.isEquiped = isEquiped ? 1 : 0;

        data.slotIndex = slotIndex;

        data.statType = (int)statAlteration.type;
        data.statsValue = statAlteration.finalValue;

        data.sellPrice = sellPrice;
        data.buyPrice = buyPrice;

        return data;
    }

    /// <summary>
    /// Crete an item based on the datas of a saved item.
    /// </summary>
    /// <param name="_data"></param>
    public Item FromSaveData(ItemOffensiveStatsSaveData _data)
    {
        Item_OffensiveStats instance;
        OffensiveStatsAlteration _statsAlteration;
        instance = Instantiate(this) as Item_OffensiveStats;

        instance.name = _data.name;
        instance.rarityStr = _data.rarity;

        instance.isSaved = _data.isSaved == 1;
        instance.isEquiped = _data.isEquiped == 1;

        instance.slotIndex = _data.slotIndex;

        _statsAlteration = new OffensiveStatsAlteration();
        _statsAlteration.type = (OffensiveStatsAlteration.ModificationType)_data.statType;
        _statsAlteration.finalValue = _data.statsValue;

        instance.statAlteration = _statsAlteration;

        instance.sellPrice = _data.sellPrice;
        instance.buyPrice = _data.buyPrice;

        return instance;
    }

    #endregion

    [System.Serializable]
    public class ItemRarityData
    {
        public string name = "Commun Rare Epique Legendaire";

        [Header("_________")]
        [Space]
        public int minSellPrice;
        public int maxSellPrice;
        [Space]
        public int minBuyPrice;
        public int maxBuyPrice;
        [Header("_________")]
        [Space]
        public  OffensiveStatsAlteration statAlteration;
    }
}

[System.Serializable]
public class OffensiveStatsAlteration
{
    public ModificationType type;

    public float minValue, maxValue;
    public float finalValue { get; set; }

    public void GenerateFinalValue()
    {
        int minValueInt = Mathf.CeilToInt(minValue * 10);
        int maxValueInt = Mathf.CeilToInt(maxValue * 10);

        finalValue = Random.Range(minValueInt, maxValueInt + 1) / 10f;
    }

    public enum ModificationType
    {
        NormalDamage,
        CapacityDamage,
        CooldownReduction,
        CriticalDamage,
        CriticalChance
    }

    public string TypeToString()
    {
        string name = "Error type name";
        switch (type)
        {
            case ModificationType.NormalDamage:
                name = "Degats normaux";
                break;
            case ModificationType.CapacityDamage:
                name = "Degats de competence";
                break;
            case ModificationType.CooldownReduction:
                name = "Temps de recharge competence";
                break;
            case ModificationType.CriticalDamage:
                name = "Degats critique";
                break;
            case ModificationType.CriticalChance:
                name = "Chance critique";
                break;
            default:
                break;
        }
        return name;
    }
}

[System.Serializable]
public class ItemOffensiveStatsSaveData
{
    public string name;
    public string rarity;

    public int itemType;

    public int isSaved;
    public int isEquiped;

    public int slotIndex;

    public int statType;
    public float statsValue;

    public int sellPrice;
    public int buyPrice;

    public override string ToString()
    {
        string str = "";

        str += "Name : " + name + "\n";
        str += "rarity : " + rarity + "\n";

        str += "isSaved : " + (isSaved == 1 ? "true" : "false") + "\n";
        str += "isEquiped : " + (isEquiped == 1 ? "true" : "false") + "\n";

        str += "type : " + ((OffensiveStatsAlteration.ModificationType)statType).ToString() + "\n";
        str += "value : " + statsValue + "\n";
        str += "sell price : " + sellPrice + "\n";
        str += "buy price : " + buyPrice + "\n";
        return str;
    }
}
