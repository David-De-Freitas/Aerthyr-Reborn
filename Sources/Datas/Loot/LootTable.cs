using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Table", menuName = "Data/LootTable")]
public class LootTable : ScriptableObject
{
    [Header("MONEY")]
    [SerializeField] int moneyDropChance;
    [SerializeField] int minMoneyAmount;
    [SerializeField] int maxMoneyAmount;
    [SerializeField] Sprite moneyIcon;
    [Header("ITEMS")]
    [SerializeField] int itemDropChance;
    [SerializeField] int minItemsDrop;
    [SerializeField] int maxItemsDrop;
    [SerializeField] List<TableItem> table = new List<TableItem>();

    void GenerateItemsDropRanges()
    {
        float maxValue = 100f;
        for (int i = 0; i < table.Count; i++)
        {
            table[i].CalculateDropRange(ref maxValue);
        }
    }

    void DropItems(Vector3 position)
    {
        float itemRoll;

        itemRoll = Random.Range(0f, 100f);

        if (itemRoll <= itemDropChance)
        {
            int itemNumber;

            GenerateItemsDropRanges();

            itemNumber = Random.Range(minItemsDrop, maxItemsDrop);

            for (int i = 0; i < itemNumber; i++)
            {
               itemRoll = Random.Range(0f, 100f);

                foreach (TableItem tableItem in table)
                {
                    if (itemRoll <= tableItem.maxValue && itemRoll >= tableItem.minValue)
                    {
                        InstanciateItem(tableItem.item, position);
                        break;
                    }
                }
            }
        }
    }

    public void Drop(Vector3 position)
    {
        int moneyAmount;
        float moneyRoll;

        DropItems(position);

        // MONEY

        moneyRoll = Random.Range(0f, 100f);
        if (moneyRoll <= moneyDropChance)
        {
            moneyAmount = Random.Range(minMoneyAmount, maxMoneyAmount +1);

            for (int i = 0; i < moneyAmount; i++)
            {
                InstanciateMoney(position);
            }
        }
    }

    public List<Item> GenerateNewItemList()
    {
        List<Item> items = new List<Item>();

        float itemRoll;
        int itemNumber;

        GenerateItemsDropRanges();

        itemNumber = Random.Range(minItemsDrop, maxItemsDrop);

        for (int i = 0; i < itemNumber; i++)
        {
            itemRoll = Random.Range(0f, 100f);

            foreach (TableItem tableItem in table)
            {
                if (itemRoll <= tableItem.maxValue && itemRoll >= tableItem.minValue)
                {
                    items.Add(tableItem.item.CreateNewInstance());
                    break;
                }
            }
        }

        return items;
    }

    private void InstanciateItem(Item item, Vector3 position)
    {
        GameObject itemGO = new GameObject();
        
        WorldItem worldItem = itemGO.AddComponent<WorldItem>();
        worldItem.item = item.CreateNewInstance();

        itemGO.transform.position = position;
    }

    private void InstanciateMoney(Vector3 position)
    {
        GameObject go = new GameObject();
        WorldMoney script = go.AddComponent<WorldMoney>();
        script.Init(position, moneyIcon);
    }

    [System.Serializable]
    public class TableItem
    {
        public Item item;
        public float dropChance;

        public float minValue { get; set; }
        public float maxValue { get; set; }
        
        public void CalculateDropRange(ref float currentMaxValue)
        {
            maxValue = currentMaxValue;
            minValue = maxValue - dropChance;
            currentMaxValue = minValue;
        }
    }
}
