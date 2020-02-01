using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class GameManager : MonoBehaviour
{
    // Singleton set up
    private static GameManager singleton;
    public static GameManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                GameObject tmpGO = new GameObject("GameManager");
                tmpGO.AddComponent<GameManager>();
            }

            return singleton;
        }
    }

    private Player player;
    public Player Player { get { return player; } }

    private InstanceManager instanceManager;
    public InstanceManager InstanceManager { get { return instanceManager; } }

    public GameData gameData;
    [Space]
    public Inventory playerInventory;
    public InputControler inputControler;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 200;

        inputControler = gameObject.AddComponent<InputControler>();
        gameData = new GameData();
        gameData.Init();
    }

    private void Start()
    {
        StartCoroutine(LoadGameSave());
    }

    /// <summary>
    /// Set the Player.
    /// The gameObject pass will be destroy if the Player is already set.
    /// </summary>
    /// <param name="gameObj"></param>
    public void SetPlayer(Player p, Inventory inventory)
    {
        if (player != null)
        {
            player.Init(p.transform);
            Destroy(p.gameObject);
        }
        else
        {
            player = p;
            playerInventory = inventory;
            DontDestroyOnLoad(player.gameObject);
        }
    }

    /// <summary>
    /// Set the Player.
    /// The gameObject pass will be destroy if the Player is already set.
    /// </summary>
    /// <param name="instanceM"></param>
    public void SetInstanceManager(InstanceManager instanceM)
    {
        if (instanceManager != null)
        {
            Destroy(instanceM.gameObject);
            return;
        }

        if (instanceM != null)
        {
            instanceManager = instanceM;
            DontDestroyOnLoad(instanceManager);
        }
        else
        {
            Debug.LogError("!! ERROR !! " + instanceM + " is not the player.");
        }
    }

    private IEnumerator LoadGameSave()
    {
        yield return new WaitForEndOfFrame();

        GameSaveManager.LoadInventory(playerInventory, gameData.itemsDataBase);
        GameSaveManager.LoadEquipment(HudManager.Singleton.equipmentUI, gameData.itemsDataBase);
        GameSaveManager.LoadPlayerProgress(player.stats);
    }

    private void OnApplicationQuit()
    {
        GameSaveManager.SaveInventory(playerInventory);
        GameSaveManager.SaveEquipement(HudManager.Singleton.equipmentUI);
        GameSaveManager.SaveGameProgress(gameData.gameProgress);
        GameSaveManager.SavePlayerProgress(player.stats);
    }
}

[System.Serializable]
public class GameData
{
    public ItemsDataBase itemsDataBase;
    public DifficultyDataBase difficultyDataBase;
    [Header("____________________________________________")]
    [Header("PROGRESS")]
    [Space(5)]
    public GameProgress gameProgress;

    //Constructor
    public GameData()
    {

    }

    public void Init()
    {
        GameSaveManager.LoadGameProgress(out gameProgress);
        LoadDataBases();
    }

    public void LoadDataBases()
    {
        itemsDataBase = Resources.Load<ItemsDataBase>("Data/Items/ItemDataBase");
        difficultyDataBase = Resources.Load<DifficultyDataBase>("Data/DifficultyData");
    }

    public GameDifficulty GetGameDifficulty()
    {
        return gameProgress.currentGameDifficulty;
    }

    public void IncreaseGameDifficulty()
    {
        if ((int)gameProgress.currentGameDifficulty < (int)GameDifficulty._COUNT - 1)
        {
            gameProgress.currentGameDifficulty = (GameDifficulty)((int)gameProgress.currentGameDifficulty + 1);
        }
    }

    public float GetEnemiesStatsFactor()
    {
        return difficultyDataBase.dataModifiers[(int)gameProgress.currentGameDifficulty].enemiesStatsFactor;
    }

    public GameProgress.ProgressPerDifficulty GetCurrentGameDifficultyProgress()
    {
        return gameProgress.perDifficulties[(int)GetGameDifficulty()];
    }
}

[System.Serializable]
public class GameProgress
{
    public GameDifficulty currentGameDifficulty;
    [Space]
    public int expeditionStartedCount;
    public int expeditionFinishedCount;
    public int expeditionFailedCount;
    [Space]
    public int bossDefeatedCount;
    public int enemiesDefeatedCount;
    [Space]
    public bool haveEncounterPirates;
    public bool haveFinishedPirateWhaleAttack;
    [Space]
    public ProgressPerDifficulty[] perDifficulties = new ProgressPerDifficulty[(int)GameDifficulty._COUNT];

    [System.Serializable]
    public class ProgressPerDifficulty
    {
        public int expeditionStartedCount = 0;
        public int expeditionFinishedCount = 0;
        public int expeditionFailedCount = 0;
        public int bossDefeatedCount = 0;
        [Space]
        public int loulpeBasicDefeatedCount = 0;
        public int cerfbourseBasicDefeatedCount = 0;
        public int castureuilBasicDefeatedCount = 0;
        [Space]
        public int loulpePirateDefeatedCount = 0;
        public int cerfboursePirateDefeatedCount = 0;
        public int castureuilPirateDefeatedCount = 0;
    }
}
