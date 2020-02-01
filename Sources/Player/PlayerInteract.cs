using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] LayerMask layerToCheck;

    Inventory inventory;
    new BoxCollider2D collider2D;

    private void Awake()
    {
        inventory = GetComponentInChildren<Inventory>();
    }

    private void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
    }

    public bool CheckInteraction()
    {
        bool isInteract;

        Collider2D[] collidersHit = Physics2D.OverlapBoxAll(collider2D.bounds.center, collider2D.size, 0f, layerToCheck);

        isInteract = collidersHit != null;

        for (int i = 0; i < collidersHit.Length; i++)
        {
            IInteractive interactive = collidersHit[i].GetComponent<IInteractive>();

            if (collidersHit[i].CompareTag("Item"))
            {
                WorldItem item = collidersHit[i].GetComponent<WorldItem>();
                item.PickUp(ref inventory);
                break;
            }
            else if (interactive.ToMonoBehaviour() != null && interactive.CanInteract(null))
            {
                interactive.Interact(null);
                break;
            }
            else if (collidersHit[i].CompareTag("Merchant"))
            {
                MerchantWorld merchant = collidersHit[i].GetComponentInParent<MerchantWorld>();
                merchant.Interact();
               
                break;
            }
            else if (collidersHit[i].CompareTag("EndMapEvent"))
            {
                EventEndScene nextLevel = collidersHit[i].GetComponent<EventEndScene>();
                nextLevel.Activate();
                break;
            }
            else if (collidersHit[i].CompareTag("WhaleDialogue"))
            {
                dialogue dialogue = collidersHit[i].GetComponent<dialogue>();
                dialogue.InitDialog();
                break;
            }
        }

        return isInteract;
    }
}
