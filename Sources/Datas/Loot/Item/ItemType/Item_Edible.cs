using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Data/Item/Edible")]
public class Item_Edible : Item {

    public override Item CreateNewInstance()
    {
        Item_Edible instance;
        instance = ScriptableObject.CreateInstance<Item_Edible>();

        return instance;
    }
}
