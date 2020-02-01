using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] Transform gear;

    public Item item;
    public int itemIndex;
    public TypeOfSlot type;

    private bool isHover = false;
    private bool isPreviousHover = false;

    public bool IsHover { get { return isHover; } }

    HudManager hudManager;

    private void Start()
    {
        hudManager = HudManager.Singleton;

        item = null;
        itemIndex = -1;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public enum TypeOfSlot
    {
        Defensive,
        Offensive,
        Capacity
    }

    private void Update()
    {
        if (hudManager.equipmentUI.state == EquipmentUI.EquipmentState.Open)
        {
            isHover = IsPointerHover();

            if (isHover != isPreviousHover)
            {
                isPreviousHover = isHover;

                if (!isHover)
                {
                    hudManager.itemStatsPanel.DisableRender();
                }
            }

            if (isHover)
            {
                gear.Rotate(Vector3.forward * -100 * Time.deltaTime);

                if (item != null)
                {
                    hudManager.itemStatsPanel.SetItemInfo(item, HUDItemStats.PriceType.Sell);
                }
            }
        }
        else
        {
            if (isHover)
            {
                isHover = false;
            }
        }
    }

    public void AddItem(Item newItem, int index)
    {
        item = newItem;
        itemIndex = index;

        IconUpdate();
        itemIcon.transform.localScale = Vector3.one * 2f;
        itemIcon.enabled = true;
        itemIcon.preserveAspect = true;
    }

    public Sprite GetIcon()
    {
        return itemIcon.sprite;
    }

    public void ClearSlot()
    {
        item = null;
        itemIndex = -1;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public bool IsPointerHover()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);
    }

    public void UpdateItem(Item newItem, int index)
    {
        item = newItem;
        itemIndex = index;

        if (item != null)
        {
            IconUpdate();

            itemIcon.enabled = true;
            itemIcon.preserveAspect = true;

            itemIcon.transform.localScale = Vector3.one * 2f;
        }
        else
        {
            itemIcon.enabled = false;
            itemIcon.sprite = null;
        }
    }

    public void IconUpdate()
    {
        if (item.isSaved)
        {
            itemIcon.sprite = item.savedIcon;
        }
        else
        {
            itemIcon.sprite = item.icon;
        }
    }

    public void EnableIconDisplay(bool state)
    {
        itemIcon.enabled = state;
    }
}
