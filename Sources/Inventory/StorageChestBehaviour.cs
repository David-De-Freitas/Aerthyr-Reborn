using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageChestBehaviour : MonoBehaviour, IInteractive
{
    public StorageChestData data;
    public static bool dataInitialized = false;

    private void Start()
    {
        data = Resources.Load<StorageChestData>("Data/Storage/StorageChest");
        if (!dataInitialized)
        {
            data.Init();
            dataInitialized = true;
        }
    }

    public void Interact(Player pController)
    {
        GameManager.Singleton.inputControler.OpenStorageChest();
    }

    public void StopInteraction()
    {
        
    }

    public void Begin()
    {
        
    }

    public void End()
    {
        
    }

    public bool CanInteract(Player pController)
    {
        return true;
    }

    public void CancelInteraction()
    {
        
    }
}
