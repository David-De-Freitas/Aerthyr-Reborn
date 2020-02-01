using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StorageChest", menuName = "Data/Storage/StorageChest")]
public class StorageChestData : ScriptableObject
{
    public int slotPerPage { get; private set; }
    public int linePerPage;
    public int slotPerLine;
    [Space]
    public int activePageIndex = 0;
    public List<Item> items = new List<Item>();

    public void Init()
    {
        slotPerPage = linePerPage * slotPerLine;
    }

    public bool CanAddItemOnPage(int pageIndex)
    {
        if (items.Count < slotPerPage)
        {
            return true;
        }
        else if (items.ContainNullItem())
        {
            return true;
        }

        return false;
    }

    public void AddItemAtPosition(Item item, int slotIndex)
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
    }

    public void AddItemAtPosition(Item item, int slotIndex, int pageIndex)
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
    }

    public void RemoveItemAt(int index)
    {
        items[index] = null;
    }

    public Item GetItemAt(int index)
    {
        return items[index];
    }
}
