using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageChestUI : MonoBehaviour
{
    [Header("PREFABS")]
    [SerializeField] Object slotAnchorPrefab;
    [SerializeField] Object slotPrefab;
    [Space]
    [Header("REFERENCES")]
    [SerializeField] RectTransform gridAnchor;
    public GameObject selector;
    [SerializeField] RectTransform page;
    [SerializeField] List<InventorySlot> slots;
    [Header("INFORMATIONS")]
    public int activePageIndex;
    StorageChestData chestData;
    HudManager hudManager;
    CanvasGroup canvasGroup;

    int selectedSlotIndex;

    public enum State
    {
        Opening,
        Open,
        Closing,
        Close,
    }

    public State state { get; private set; }

    private void Awake()
    {
        if (chestData == null)
        {
            chestData = Resources.Load<StorageChestData>("Data/Storage/StorageChest");
        }
    }

    private void Start()
    {
        selectedSlotIndex = -1;
        state = State.Close;

        page = gridAnchor.GetComponentOnlyInChildren<RectTransform>();
        slots = new List<InventorySlot>();
        chestData = Resources.Load<StorageChestData>("Data/Storage/StorageChest");

        hudManager = HudManager.Singleton;
        hudManager.storageChestUI = this;
        canvasGroup = GetComponent<CanvasGroup>();
        AddSlotsLines();
    }

    private void Update()
    {
        UpdateSelector();
    }

    /// <summary>
    /// Add new slot line(s) to the inventory UI
    /// </summary>
    public void AddSlotsLines()
    {
        // Instantiate new Anchor(s)
        for (int i = 0; i < chestData.linePerPage; i++)
        {
            int slotsAddedCount = 0;
            GameObject newAnchor = (GameObject)Instantiate(slotAnchorPrefab, page) as GameObject;

            // Instantiate new Slots
            while (slotsAddedCount < chestData.slotPerLine)
            {
                slots.Add(CreateSlot(newAnchor.transform));
                slotsAddedCount++;
            }
        }

    }

    // ----------------------------------------------------------------------- **

    public void Open()
    {
        if (state == State.Close)
        {
            canvasGroup.alpha = 1;
            state = State.Open;
        }


        hudManager.itemStatsPanel.DisableRender();
        hudManager.itemStatsPanel.enabled = true;
    }

    // ----------------------------------------------------------------------- **

    public void Close()
    {
        if (state == State.Open)
        {
            canvasGroup.alpha = 0;
            state = State.Close;
        }

        DisableSelection();
        hudManager.itemStatsPanel.DisableRender();
        hudManager.itemStatsPanel.enabled = false;
        hudManager.itemHandler.ResetIcon();
    }

    // ----------------------------------------------------------------------- **

    public void DisableSelection()
    {
        if (selectedSlotIndex != -1)
        {
            slots[selectedSlotIndex].EnableIconDisplay(true);

            selectedSlotIndex = -1;
        }
    }

    // ----------------------------------------------------------------------- **

    public bool IsFocus()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(page, Input.mousePosition);
    }

    // ----------------------------------------------------------------------- **

    public bool HasSelectedSlot()
    {
        return selectedSlotIndex != -1;
    }

    // ----------------------------------------------------------------------- **

    public void SelectSlot(bool checkIfNull)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsPointerHover())
            {
                if ((checkIfNull && slots[i].item != null) || !checkIfNull)
                {
                    selectedSlotIndex = i;

                    if (checkIfNull)
                    {
                        hudManager.itemHandler.SetIcon(slots[i].GetIcon());
                    }

                    slots[i].EnableIconDisplay(false);
                }
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void SwapItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsPointerHover())
            {
                if (slots[i] == slots[selectedSlotIndex])
                {
                    selectedSlotIndex = -1;
                    hudManager.itemHandler.ResetIcon();
                    slots[i].EnableIconDisplay(true);
                }
                else
                {
                    Item savedItem;
                    int savedItemIndex;

                    savedItem = slots[i].item;
                    savedItemIndex = slots[i].itemIndex;

                    slots[i].UpdateItem(slots[selectedSlotIndex].item, slots[selectedSlotIndex].itemIndex);
                    slots[selectedSlotIndex].UpdateItem(savedItem, savedItemIndex);

                    // Update slot index for each items.
                    if (slots[i].item != null)
                    {
                        slots[i].item.slotIndex = i;
                    }
                    if (slots[selectedSlotIndex].item != null)
                    {
                        slots[selectedSlotIndex].item.slotIndex = selectedSlotIndex;
                    }

                    hudManager.itemHandler.ResetIcon();

                    selectedSlotIndex = -1;
                }

                break;
            }
        }
    }
    // ----------------------------------------------------------------------- **
    private InventorySlot CreateSlot(Transform parent)
    {
        InventorySlot tmp;
        GameObject newSlot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newSlot.transform.SetParent(parent);
        newSlot.transform.localScale = Vector3.one;
        tmp = newSlot.GetComponent<InventorySlot>();
        return tmp;
    }

    // ----------------------------------------------------------------------- **
    private void UpdateSelector()
    {
        bool hover = false;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsHover)
            {
                selector.SetActive(true);
                selector.transform.position = slot.transform.position;
                hover = true;
                break;
            }
        }

        if (!hover)
        {
            selector.SetActive(false);
        }
    }
}
