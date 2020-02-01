using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item/Craft Resource")]
public class Item_CraftResource : Item {

    public override Item CreateNewInstance()
    {
        Item_CraftResource instance;
        instance = ScriptableObject.CreateInstance<Item_CraftResource>();

        return instance;
    }
}
