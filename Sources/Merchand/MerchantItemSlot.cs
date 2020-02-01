using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantItemSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image haloSelector;
    public Item item;
    public int itemIndex;

    private bool canUpdate;
    private bool isHover = false;
    private bool isPreviousHover = false;

    public bool IsHover { get { return isHover; } }

    public delegate void DelegateOnHoverSlot(Item _item);
    public DelegateOnHoverSlot OnHoverEnter;

    HudManager hudManager;

    private void Awake()
    {
        item = null;
        itemIndex = -1;
    }

    private void Start()
    {
        hudManager = HudManager.Singleton;
    }

    private void Update()
    {
        if (canUpdate)
        {
            isHover = IsPointerHover();

            if (isHover != isPreviousHover)
            {
                isPreviousHover = isHover;

                if (isHover)
                {
                    icon.transform.localScale = Vector3.one * 1.2f;
                    haloSelector.enabled = true;

                    OnHoverEnter(item);
                }
                else
                {
                    icon.transform.localScale = Vector3.one;
                    haloSelector.enabled = false;

                    hudManager.itemStatsPanel.DisableRender();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (canUpdate)
        {
            if (isHover)
            {
                if (item != null)
                {
                    hudManager.itemStatsPanel.SetItemInfo(item, HUDItemStats.PriceType.Buy);
                }
            }
        }
    }

    public void AddItem(Item newItem, int index)
    {
        item = newItem;
        itemIndex = index;

        IconUpdate();

        icon.enabled = true;
        icon.preserveAspect = true;
    }

    public Sprite GetIcon()
    {
        return icon.sprite;
    }

    public void ClearSlot()
    {
        item = null;
        itemIndex = -1;

        icon.sprite = null;
        icon.enabled = false;
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

            icon.enabled = true;
            icon.preserveAspect = true;

            icon.transform.localScale = Vector3.one * 2f;
        }
        else
        {
            icon.enabled = false;
            icon.sprite = null;
        }
    }

    public void IconUpdate()
    {
        if (item.isSaved)
        {
            icon.sprite = item.savedIcon;
        }
        else
        {
            icon.sprite = item.icon;
        }
    }

    public void EnableIconDisplay(bool state)
    {
        icon.enabled = state;
    }

    public void CanBeUpdated(bool state)
    {
        canUpdate = state;
    }
}
