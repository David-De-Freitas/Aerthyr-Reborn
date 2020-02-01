using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    new public string name = "New Item";

    public ItemType type;
    public ItemRarity rarity;

    public bool isSaved = false;
    public bool isEquiped = false;
    [Space]
    public Sprite icon = null;
    public Sprite savedIcon = null;
    [Space]
    public int sellPrice;
    public int buyPrice;
    [Space]
    public int slotIndex = -1;

    public abstract Item CreateNewInstance();

    [Serializable]
    public class RaritiesPerGameDifficuty
    {
        public RarityData[] rarities;

        [Serializable]
        public class RarityData
        {
            public ItemRarity itemRarity;
            public float dropChance;

            public float minValue { get; private set; }
            public float maxValue { get; private set; }

            public void SetRarityBounds(ref float currentMaxValue)
            {
                maxValue = currentMaxValue;
                minValue = maxValue - dropChance;
                currentMaxValue = minValue;
            }
        }
    }
}

public enum ItemType
{
    Stats_Defensive,
    Stats_Offensive,
    Capacity,
    Edible,
    CraftItem
}

public enum ItemRarity
{
    Common, Rare, Epic, Legendary, Mythical, _COUNT
}
