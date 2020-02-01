using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantWorld : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] Canvas canvas;
    [SerializeField] Text text;

    [Header("DATA")]
    [SerializeField] float spawnChance;
    [Space]
    [SerializeField] LootTable table;
    [SerializeField] List<Item> itemsAvailable = new List<Item>();

    [Header("ON TRIGGER")]
    [SerializeField]
    [TextArea]
    string[] triggerSentences;

    MerchantHUD merchantHUD;

    // Use this for initialization
    void Start()
    {
        CheckIfSpawn();

        merchantHUD = HudManager.Singleton.merchantHUD;

        itemsAvailable = table.GenerateNewItemList();
        merchantHUD.SetItemsInSlots(itemsAvailable);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        player = collision.GetComponent<Player>();

        if (player != null)
        {
            text.text = GetSentence();
            canvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Player player;
        player = other.GetComponent<Player>();

        if (player != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void CheckIfSpawn()
    {
        float value;
        value = Random.Range(0, 100);

        if (value > spawnChance)
        {
            Destroy(gameObject);
        }
    }

    string GetSentence()
    {
        int id;
        id = Random.Range(0, triggerSentences.Length);

        return triggerSentences[id];
    }

    public void Interact()
    {
        HudManager.Singleton.merchantHUD.Open();
        HudManager.Singleton.merchantHUD.SetMerchantWorld(this);
        canvas.gameObject.SetActive(false);
    }

    public void RemoveItem(Item itemToRemove)
    {
        itemsAvailable.Remove(itemToRemove);
    }
}
