using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantHUD : MonoBehaviour
{
    [SerializeField] RectTransform slotsAnchor;
    [SerializeField] GameObject selector;
    [Space]
    [SerializeField] Image hoverSlotBubble;
    [SerializeField] Text hoverSlotText;
    [TextArea]
    [SerializeField] List<string> hoverSlotTextStr = new List<string>();
    List<MerchantItemSlot> slots = new List<MerchantItemSlot>();

    const string ITEM_NAME = "[ITEM_NAME]";
    const string ITEM_PRICE = "[ITEM_PRICE]";

    CanvasGroup canvasGroup;
    StateAnimation stateAnim;

    int selectedSlotIndex;
    float hoverSlotTime;
    HudManager hudManager;
    MerchantWorld merchantWorld;

    // Use this for initialization
    private void Awake()
    {
        selectedSlotIndex = -1;
        GetAllSlots();
    }

    // ----------------------------------------------------------------------- **

    private void Start()
    {
        hudManager = HudManager.Singleton;
        stateAnim = StateAnimation.close;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        hoverSlotTime = 0f;
    }

    // ----------------------------------------------------------------------- **

    private void Update()
    {
        UpdateAnim();
        UpdateSelector();
        UpdateSlotInfo();
    }

    // ----------------------------------------------------------------------- **

    private void GetAllSlots()
    {
        for (int i = 0; i < slotsAnchor.childCount; i++)
        {
            MerchantItemSlot slot = slotsAnchor.GetChild(i).GetComponent<MerchantItemSlot>();

            if (slot != null)
            {
                slot.OnHoverEnter += DisplayRandomInfoText;
                slots.Add(slot);
            }
        }
    }

    // ----------------------------------------------------------------------- **

    private void DisplayRandomInfoText(Item _item)
    {
        int _hasToDisplay = Random.Range(0, 100); // Define if the text will be displayed.

        if (_item != null && _hasToDisplay <= 35)
        {
            StopAllCoroutines();

            hoverSlotTime = Random.Range(3.8f, 4.2f);
            hoverSlotBubble.enabled = true;
            hoverSlotText.enabled = true;
            hoverSlotText.text = "";

            StartCoroutine(HoverSlotTextUpdate(GetRandomInfoText(_item)));
        }
    }

    private string GetRandomInfoText(Item _item)
    {
        string str = "";
        int i = Random.Range(0, hoverSlotTextStr.Count);

        str = hoverSlotTextStr[i];

        if (str.Contains(ITEM_NAME))
        {
            string[] splitStr = str.Split(new string[] { ITEM_NAME },System.StringSplitOptions.None);
          
            str = splitStr[0] + _item.name + splitStr[1];
        }

        if (str.Contains(ITEM_PRICE))
        {
            string[] splitStr = str.Split(new string[] { ITEM_PRICE }, System.StringSplitOptions.None);

            str = splitStr[0] + _item.buyPrice + " Crokille" + splitStr[1];
        }

        return str;
    }

    private IEnumerator HoverSlotTextUpdate(string _str = "")
    {
        char[] _chrArray = _str.ToCharArray();

        float _updateRate = Mathf.Min(1.5f / Mathf.Max(_chrArray.Length, 1), 0.02f);

        for (int i = 0; i < _chrArray.Length; i++)
        {
            hoverSlotText.text += _chrArray[i];
            yield return new WaitForSeconds(_updateRate);
        }

        yield return 0;
    }

    private void UpdateSlotInfo()
    {
        if (hoverSlotTime > 0)
        {
            hoverSlotTime -= Time.deltaTime;
        }
        else if (hoverSlotBubble.enabled)
        {
            hoverSlotBubble.enabled = false;
            hoverSlotText.enabled = false;
        }
    }

    // ----------------------------------------------------------------------- **

    private void UpdateAnim()
    {
        if (stateAnim == StateAnimation.opening)
        {
            canvasGroup.alpha += Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                stateAnim = StateAnimation.open;
                AllowSlotsUpdate(true);
            }
        }
        else if (stateAnim == StateAnimation.closing)
        {
            canvasGroup.alpha -= Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                stateAnim = StateAnimation.close;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    private void UpdateSelector()
    {
        bool over = false;
        foreach (MerchantItemSlot slot in slots)
        {
            if (slot.IsHover)
            {
                selector.SetActive(true);
                selector.transform.position = slot.transform.position;
                over = true;
                break;
            }
        }

        if (!over)
        {
            selector.SetActive(false);
        }
    }

    // ----------------------------------------------------------------------- **

    public void ClearAllSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ClearSlot();
        }
    }

    // ----------------------------------------------------------------------- **

    public void SetMerchantWorld(MerchantWorld _merchantWorld)
    {
        merchantWorld = _merchantWorld;
    }

    // ----------------------------------------------------------------------- **

    public bool IsFocus()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsHover)
            {
                return true;
            }
        }
        return false;
    }

    // ----------------------------------------------------------------------- **

    public void SetItemsInSlots(List<Item> items)
    {
        List<int> slotsAvailableID = new List<int>();

        for (int i = 0; i < slots.Count; i++)
        {
            slotsAvailableID.Add(i);
        }

        for (int i = 0; i < items.Count; i++)
        {
            int slotID = slotsAvailableID[Random.Range(0, slotsAvailableID.Count)];
            slots[slotID].AddItem(items[i], i);
            slotsAvailableID.Remove(slotID);
        }
    }

    // ----------------------------------------------------------------------- **

    public void AllowSlotsUpdate(bool state)
    {
        foreach (MerchantItemSlot slot in slots)
        {
            slot.CanBeUpdated(state);
        }
    }

    // ----------------------------------------------------------------------- **

    public void SelectSlot()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsHover)
            {
                selectedSlotIndex = i;
                AllowSlotsUpdate(false);
                hudManager.inventoryUI.AllowSlotsUpdate(false);
                hudManager.itemActionMenu.SetActive(true);
                hudManager.itemActionMenu.SetState(ItemActionMenu.MainAction.Buy);
                break;
            }
        }
    }

    // ----------------------------------------------------------------------- **

    public Item GetSelectedItem()
    {
        Item selectedItem = null;

        if (selectedSlotIndex != -1)
        {
            selectedItem = slots[selectedSlotIndex].item;
        }

        return selectedItem;
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

    public void RemoveSelectedItem()
    {
        merchantWorld.RemoveItem(slots[selectedSlotIndex].item);

        slots[selectedSlotIndex].ClearSlot();
        selectedSlotIndex = -1;

        AllowSlotsUpdate(true);
    }

    // ----------------------------------------------------------------------- **

    public void Open()
    {
        stateAnim = StateAnimation.opening;
        GameManager.Singleton.inputControler.OpenMerchant();

    }

    // ----------------------------------------------------------------------- **

    public void Close()
    {
        stateAnim = StateAnimation.closing;
        AllowSlotsUpdate(false);
    }

    // ----------------------------------------------------------------------- **

    enum StateAnimation
    {
        opening,
        open,
        closing,
        close
    }
}
