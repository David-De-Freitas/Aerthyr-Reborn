using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image haloSelector;

    public Item item = null;
    public int itemIndex = -1;

    private bool canUpdate = true;
    private bool isHover = false;
    private bool isPreviouslyHover = false;

    public bool IsHover { get { return isHover; } }

    HudManager hudManager;

    private void Start()
    {
        hudManager = HudManager.Singleton;
    }

    private void Update()
    {
        if (canUpdate)
        {
            isHover = RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);

            if (isHover != isPreviouslyHover)
            {
                isPreviouslyHover = isHover;

                if (isHover)
                {
                    icon.transform.localScale = Vector3.one * 2.2f;
                    haloSelector.enabled = true;
                }
                else
                {
                    icon.transform.localScale = Vector3.one * 2f;
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
                    hudManager.itemStatsPanel.SetItemInfo(item, HUDItemStats.PriceType.Sell);
                }
            }
        }
    }

    public void AddItem(Item newItem, int index)
    {
        item = newItem;
        itemIndex = index;

        IconUpdate();
        icon.transform.localScale = Vector3.one * 2f;
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
        return (isHover && canUpdate);
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
