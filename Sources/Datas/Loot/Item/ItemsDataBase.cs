using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Data/Item/DataBase")]
public class ItemsDataBase : ScriptableObject
{
    [Header("Capacities")]
    public Item_Skill fury;
    public Item_Skill shield;
    public Item_Skill sonicBoom;
    [Header("Defensives")]
    public Item_DefensiveStats armor;
    public Item_DefensiveStats hpMax;
    public Item_DefensiveStats hpRegeneration;
    public Item_DefensiveStats staminaMax;
    public Item_DefensiveStats staminaRegeneration;
    [Header("Offensives")]
    public Item_OffensiveStats normalDamages;
    public Item_OffensiveStats critChances;
    public Item_OffensiveStats critDamages;
    public Item_OffensiveStats capacitiesDamages;
    public Item_OffensiveStats cooldownReduction;
	
}
