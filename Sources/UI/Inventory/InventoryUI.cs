using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("PREFABS")]
    [SerializeField] Object slotAnchorPrefab;
    [SerializeField] Object gridLockerPrefab;
    [SerializeField] Object slotPrefab;
    [Space]
    [Header("REFERENCES")]
    [SerializeField] RectTransform grid;
    public GameObject inventorySelector;
    [SerializeField] Text moneyAmountText;
    [Space]
    [Header("HUD")]
    [SerializeField] RectTransform gearTop;
    [SerializeField] RectTransform gearBottom;
    [Space]
    [Header("SLOT LIST")]
    [SerializeField] List<InventorySlot> slots = new List<InventorySlot>();
    List<GameObject> gridLockers = new List<GameObject>();

    HudManager hudManager;
    Inventory inventory;
    InputControler inputControler;
    ItemHandler itemHandler;

    int selectedSlotIndex;
    Animator animator;

    public State state { get; set; }

    // --------------------------------------------------------------------------------------------------------------------- **
    #region NATIVES FUNCTIONS
    // ----------------------------------------------------------------------- **

    private void Start()
    {
        hudManager = HudManager.Singleton;

        inventory = GameManager.Singleton.playerInventory;
        inputControler = GameManager.Singleton.inputControler;

        animator = GetComponent<Animator>();

        InitInventory();
        state = State.Close;
    }

    // ----------------------------------------------------------------------- **

    private void Update()
    {
        UpdateInventoryUI();
    }
    #endregion

    // --------------------------------------------------------------------------------------------------------------------- **

    #region PRIVATE FUNCTIONS

    private void InitInventory()
    {
        AddSlotsLines();
        selectedSlotIndex = -1;
    }

    // ----------------------------------------------------------------------- **

    private void UpdateInventoryUI()
    {
        UpdateAnimation();
        UpdateSelector();
    }

    // ----------------------------------------------------------------------- **

    private void UpdateAnimation()
    {
        animator.SetInteger("State", (int)state);

        float speed = 500f;
        int dir = 1;

        if (state == State.Opening)
        {
            gearTop.Rotate(Vector3.forward, gearTop.rotation.z + speed * dir * Time.deltaTime);
            gearBottom.Rotate(Vector3.forward, gearTop.rotation.z + speed * dir * Time.deltaTime);
        }
        else if (state == State.Closing)
        {
            dir *= -1;
            gearTop.Rotate(Vector3.forward, gearTop.rotation.z + speed * dir * Time.deltaTime);
            gearBottom.Rotate(Vector3.forward, gearTop.rotation.z + speed * dir * Time.deltaTime);
        }
    }

    // ----------------------------------------------------------------------- **

    private void UpdateSelector()
    {
        bool hover = false;
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsHover)
            {
                inventorySelector.SetActive(true);
                SetSelectorPosition(slot.gameObject.transform);
                hover = true;
                break;
            }
        }

        if (!hover)
        {
            inventorySelector.SetActive(false);
        }
    }

    // ----------------------------------------------------------------------- **

    void AddGridLockers()
    {
        int gridLockersNeeded;
        gridLockersNeeded = (inventory.MaxItemNumber - inventory.slotsAvailable) / inventory.MinItemNumber;

        while (gridLockers.Count < gridLockersNeeded)
        {
            GameObject newLocker = (GameObject)Instantiate(gridLockerPrefab, grid) as GameObject;
            gridLockers.Add(newLocker);
        }
    }

    // ----------------------------------------------------------------------- **

    void RemoveGridLockers()
    {
        foreach (GameObject locker in gridLockers)
        {
            Destroy(locker);
        }

        gridLockers.Clear();
    }

    // ----------------------------------------------------------------------- **

    InventorySlot CreateInventorySlot(Transform parent)
    {
        InventorySlot tmp;
        GameObject newSlot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        newSlot.transform.SetParent(parent);
        newSlot.transform.localScale = Vector3.one;
        tmp = newSlot.GetComponent<InventorySlot>();
        return tmp;
    }

    // ----------------------------------------------------------------------- **

    void SetSelectorPosition(Transform transform)
    {
        inventorySelector.transform.position = transform.position;
    }

    // ----------------------------------------------------------------------- **

    void SetState(State newState)
    {
        state = newState;
        if (state == State.Close)
        {
            inputControler.ControlBlocked = false;
            AllowSlotsUpdate(false);
            CustomCursorManager.Singleton.SetState(CustomCursorManager.CursorState.Hidden);
        }
        else if (state == State.Open)
        {
            AllowSlotsUpdate(true);
            CustomCursorManager.Singleton.SetState(CustomCursorManager.CursorState.Normal);
        }
    }

    #endregion

    // --------------------------------------------------------------------------------------------------------------------- **

    #region PUBLIC FUNCTIONS

    public void UpdateMoney()
    {
        moneyAmountText.text = inventory.MoneyAmount.ToString();
    }

    /// <summary>
    /// Add new slot line(s) to the inventory UI
    /// </summary>
    public void AddSlotsLines()
    {
        int linesToAdd;
        linesToAdd = (inventory.slotsAvailable - slots.Count) / inventory.MinItemNumber;

        RemoveGridLockers();

        // Instantiate new Anchor(s)
        for (int i = 0; i < linesToAdd; i++)
        {
            int slotsAdded = 0;
            GameObject newAnchor = (GameObject)Instantiate(slotAnchorPrefab, grid) as GameObject;

            // Instantiate new Slots
            while (slotsAdded < inventory.MinItemNumber)
            {
                slots.Add(CreateInventorySlot(newAnchor.transform));
                slotsAdded++;
            }
        }

        AddGridLockers();
    }

    // ----------------------------------------------------------------------- **

    /// <summary>
    /// Add an item to the inventory HUD
    /// </summary>
    /// <param name="itemToAdd"></param>
    public void AddItem(Item itemToAdd, int index)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == null)
            {
                itemToAdd.slotIndex = i;
                slots[i].AddItem(itemToAdd, index);
                break;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    /// <summary>
    /// Add an item to the inventory HUD
    /// </summary>
    /// <param name="itemToAdd"></param>
    public void AddItem(Item itemToAdd, int index, int indexSlot)
    {
        if (slots[indexSlot].item == null)
        {
            itemToAdd.slotIndex = indexSlot;
            slots[indexSlot].AddItem(itemToAdd, index);
        }
    }

    // ----------------------------------------------------------------------- **

    /// <summary>
    /// Remove the item from the inventory HUD
    /// </summary>
    /// <param name="itemToRemove"></param>
    public void RemoveItemAt(int index)
    {

    }

    // ----------------------------------------------------------------------- **

    public void Open()
    {
        if (state == State.Close)
        {
            state = State.Opening;
        }
        else if (state == State.Closing)
        {
            state = State.Opening;
        }

        hudManager.itemStatsPanel.DisableRender();
        hudManager.itemStatsPanel.enabled = true;
    }

    // ----------------------------------------------------------------------- **

    public void Close()
    {
        if (state == State.Open)
        {
            state = State.Closing;
        }
        else if (state == State.Opening)
        {
            state = State.Closing;
        }

        DisableSelection();
        hudManager.itemStatsPanel.DisableRender();
        hudManager.itemStatsPanel.enabled = false;
        hudManager.itemHandler.ResetIcon();
    }

    // ----------------------------------------------------------------------- **

    public bool IsFocus()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(grid, Input.mousePosition);
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

    public void IconsUpdate()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                if (slot.item.isSaved)
                {
                    slot.IconUpdate();
                }
            }
        }
    }

    // ----------------------------------------------------------------------- **

    /// <summary>
    /// Get the index of the item in the selected slot in the inventory.
    /// </summary>
    /// <returns></returns>
    public int GetSelectedItemIndex()
    {
        return slots[selectedSlotIndex].itemIndex;
    }

    // ----------------------------------------------------------------------- **

    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }

    // ----------------------------------------------------------------------- **

    public Item GetSelectedItem()
    {
        Item item = null;

        if (selectedSlotIndex != -1)
        {
            item = slots[selectedSlotIndex].item;
        }

        return item;
    }

    // ----------------------------------------------------------------------- **

    public void ClearSelectedSlot()
    {
        slots[selectedSlotIndex].ClearSlot();

        selectedSlotIndex = -1;
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

    public void ClearSlotWithItemIndex(int index)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemIndex == -1)
            {
                continue;
            }
            else if (slots[i].itemIndex == index)
            {
                slots[i].ClearSlot();

                if (selectedSlotIndex == i)
                {
                    ClearSelectedSlot();
                }
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void AllowSlotsUpdate(bool state)
    {
        foreach (InventorySlot slot in slots)
        {
            slot.CanBeUpdated(state);
        }
    }

    // ----------------------------------------------------------------------- **
    // EXCHANGES FUNCTIONS
    // ----------------------------------------------------------------------- **

    public void SecondarySelectSlot()
    {
        if (selectedSlotIndex == -1)
        {
            SelectSlot(true);

            if (selectedSlotIndex != -1)
            {
                AllowSlotsUpdate(false);
                hudManager.merchantHUD.AllowSlotsUpdate(false);
                slots[selectedSlotIndex].EnableIconDisplay(true);

                hudManager.itemHandler.ResetIcon();

                hudManager.itemActionMenu.SetActive(true);
                hudManager.itemActionMenu.SetState(ItemActionMenu.MainAction.Sell);

            }
        }
    }

    // ----------------------------------------------------------------------- **

    public void SellItem()
    {
        inventory.AddMoney(slots[selectedSlotIndex].item.sellPrice);
        inventory.RemoveItemAt(slots[selectedSlotIndex].itemIndex);
        AllowSlotsUpdate(true);
        hudManager.itemActionMenu.SetActive(false);
    }

    // ----------------------------------------------------------------------- **

    public bool BuyItem(Item item)
    {
        bool hasEnoughMoney = item.buyPrice <= inventory.MoneyAmount;

        if (hasEnoughMoney)
        {
            inventory.AddItem(item);
            inventory.RemoveMoney(item.buyPrice);
        }

        return hasEnoughMoney;
    }

    #endregion

    public enum State
    {
        Opening,
        Open,
        Closing,
        Close,
    }

}
