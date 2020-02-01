using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDItemStats : MonoBehaviour
{
    [SerializeField] Text itemName;
    [Space]
    [SerializeField] Text itemType;
    [SerializeField] Text itemStatValue;
    [Space]
    [SerializeField] Text priceTypeText;
    [SerializeField] Text priceValueText;
    [Space]
    [SerializeField] Text itemDescription;
    [Space]
    [SerializeField] Transform inMerchantTransform;
    [SerializeField] Transform inInventoryTransform;

    CanvasGroup canvasGroup;

    public enum PriceType
    {
        Sell,
        Buy
    }

    // Use this for initialization
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void SetPosition(bool inMerchant)
    {
        if (inMerchant)
        {
            transform.position = inMerchantTransform.position;
        }
        else
        {
            transform.position = inInventoryTransform.position;
        }
    }

    public void SetItemInfo(Item item, PriceType priceType)
    {
        canvasGroup.alpha = 1;

        itemName.text = item.name;

        if (item is Item_DefensiveStats)
        {
            Item_DefensiveStats _item = item as Item_DefensiveStats;

            itemType.text = _item.statAlteration.TypeToString();
            itemStatValue.text = _item.statAlteration.finalValue.ToString();
            itemDescription.text = "";
        }
        else if (item is Item_OffensiveStats)
        {
            Item_OffensiveStats _item = item as Item_OffensiveStats;

            itemType.text = _item.statAlteration.TypeToString();
            itemStatValue.text = _item.statAlteration.finalValue.ToString();
            itemDescription.text = "";
        }
        else if (item is Item_Skill)
        {
            Item_Skill _item = item as Item_Skill;

            itemType.text = "";
            itemStatValue.text = "";

            itemDescription.text = _item.skill.GetDescription();
        }

        // Price display

        if (priceType == PriceType.Sell)
        {
            priceTypeText.text = "Prix de vente";
            priceValueText.text = item.sellPrice.ToString();
        }
        else
        {
            priceTypeText.text = "Prix d'achat";
            priceValueText.text = item.buyPrice.ToString();
        }

        
    }

    public void DisableRender()
    {
        canvasGroup.alpha = 0;
    }
}
