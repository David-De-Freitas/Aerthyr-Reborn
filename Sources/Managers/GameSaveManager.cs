using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameSaveManager
{
    private static readonly string SAVE_PATH = Application.persistentDataPath + "/Saved/";
    private static readonly string INVENTORY_FILE = "inv.ini";
    private static readonly string EQUIPMENT_FILE = "equi.ini";
    private static readonly string GAMEPROGRESS_FILE = "gprgrss.ini";
    private static readonly string PLAYERPROGRESS_FILE = "pprgrss.ini";

    public static void SaveInventory(Inventory _inventory)
    {
        int moneyAmount = _inventory.MoneyAmount;

        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(SAVE_PATH + INVENTORY_FILE, FileMode.Create);

        InventorySaveData invData = new InventorySaveData(_inventory);

        bf.Serialize(file, invData);
        file.Close();
    }

    public static void LoadInventory(Inventory _inventory, ItemsDataBase _itemsDataBase)
    {
       
        if (File.Exists(SAVE_PATH + INVENTORY_FILE))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(SAVE_PATH + INVENTORY_FILE, FileMode.Open);

            InventorySaveData invData = bf.Deserialize(file) as InventorySaveData;

            invData.LoadToInventory(_inventory, _itemsDataBase);

            file.Close();
        }

    }

    public static void SaveEquipement(EquipmentUI _equipment)
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(SAVE_PATH + EQUIPMENT_FILE, FileMode.Create);

        EquipmentSaveData equipData = new EquipmentSaveData(_equipment);

        bf.Serialize(file, equipData);
        file.Close();
    }

    public static void LoadEquipment(EquipmentUI _equipment, ItemsDataBase _itemsDataBase)
    {       
        if (File.Exists(SAVE_PATH + EQUIPMENT_FILE))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(SAVE_PATH + EQUIPMENT_FILE, FileMode.Open);

            EquipmentSaveData equipData = bf.Deserialize(file) as EquipmentSaveData;

            equipData.LoadToEquipment(_equipment, _itemsDataBase);

            file.Close();
        }
    }

    public static void SaveGameProgress(GameProgress _gameProgress)
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(SAVE_PATH + GAMEPROGRESS_FILE, FileMode.Create);

        bf.Serialize(file, _gameProgress);
        file.Close();
    }

    public static void LoadGameProgress(out GameProgress _gameProgress)
    {
        _gameProgress = new GameProgress();

        if (File.Exists(SAVE_PATH + GAMEPROGRESS_FILE))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(SAVE_PATH + GAMEPROGRESS_FILE, FileMode.Open);

            _gameProgress = bf.Deserialize(file) as GameProgress;

            file.Close();
        }
    }

    public static void SavePlayerProgress(PlayerStats _playerProgress)
    {
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(SAVE_PATH + PLAYERPROGRESS_FILE, FileMode.Create);
        PlayerStats.SavedDatas savedDatas = new PlayerStats.SavedDatas();
        savedDatas.FromPlayerStats(_playerProgress);

        bf.Serialize(file, savedDatas);
        file.Close();
    }

    public static void LoadPlayerProgress(PlayerStats _playerProgress)
    {
        if (File.Exists(SAVE_PATH + PLAYERPROGRESS_FILE))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(SAVE_PATH + PLAYERPROGRESS_FILE, FileMode.Open);

            PlayerStats.SavedDatas savedDatas;

            savedDatas = bf.Deserialize(file) as PlayerStats.SavedDatas;
            savedDatas.ToPlayerStats(_playerProgress);

            file.Close();
        }
    }
}
