using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class InstanceManager : MonoBehaviour
{
    //Inspector and public variables

    [Header("Data to Generate")]
    public int minMap = 1;
    public int maxMap = 2;
    [Range(0, 100)]
    public int runMapSpawnChance;

    [Header("Actual Instance Generate")]
    public int nbMaps = 0;
    public int actualMap = 0;
    [Space]
    public List<string> expedition = new List<string>();
    public AllScenes allScenes;

    SceneTransition transition;
    Player player;
    bool canChangeMap;
    bool canGenerateExpedition;

    private void Awake()
    {
        GameManager.Singleton.SetInstanceManager(this);
    }

    private void Start()
    {
        transition = GetComponentInChildren<SceneTransition>();
        player = GameManager.Singleton.Player;
        allScenes.Load();

        canChangeMap = true;
        canGenerateExpedition = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("ReGenerateInstance"))
        {
            StartExpeditionGeneration();
        }
        if (Input.GetButtonDown("Clear Instance"))
        {
            ClearInstance();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SwitchToNextMap();
        }
    }

    IEnumerator GenerateExpedition()
    {
        ClearInstance();
        actualMap = 0;

        List<string> tome1Maps = allScenes.tome1.GetAllMaps();
        List<string> tome2Maps = allScenes.tome2.GetAllMaps();

        // TOME 1 GENERATION
        int mapsToGets = Random.Range(minMap, maxMap + 1);
        int runPos = Random.Range(1, mapsToGets);

        bool haveRunMap = Random.Range(0, 101) <= runMapSpawnChance;
        
        for (int i = 0; i < mapsToGets; i++)
        {
            if (haveRunMap && i == runPos)
            {
                AddMapToTheExpedition(allScenes.tome1.run);
            }
            else
            {
                int mapID = Random.Range(0, tome1Maps.Count);
                AddMapToTheExpedition(tome1Maps[mapID]);
                tome1Maps.RemoveAt(mapID);
            }
        }

        // TOME 2 GENERATION

        mapsToGets = tome2Maps.Count;
        AddMapToTheExpedition(allScenes.tome2.safeZone);

        for (int i = 0; i < mapsToGets; i++)
        {
            int mapID = Random.Range(0, tome2Maps.Count);
            AddMapToTheExpedition(tome2Maps[mapID]);
            tome2Maps.RemoveAt(mapID);
        }

        // BOSS
        AddMapToTheExpedition(allScenes.boss);

        yield return new WaitForSeconds(0.1f);
    }

    void AddMapToTheExpedition(string name)
    {
        expedition.Add(name);
    }

    void ClearInstance()
    {
        actualMap = 0;
        expedition.Clear();
    }

    public void StartExpeditionGeneration()
    {
        if (canGenerateExpedition)
        {
            canGenerateExpedition = false;
            StartCoroutine(GenerateExpedition());
        }
    }

    public void SwitchToNextMap()
    {
        if (canChangeMap)
        {
            if (actualMap > 0 && expedition[actualMap-1] == "Tome2_SafeZone")
            {
                HudManager.Singleton.merchantHUD.ClearAllSlots();
            }

            transition.StartTransitionTo(expedition[actualMap]);
            actualMap++;
            canChangeMap = false;
        }
    }

    public void EndExpedition()
    {
        if (player.stats.health > 0f)
        {
            GameManager.Singleton.playerInventory.SaveItems();
            HudManager.Singleton.equipmentUI.SaveItems();
            GameManager.Singleton.gameData.gameProgress.expeditionFinishedCount++;
        }
        else
        {
            GameManager.Singleton.gameData.gameProgress.expeditionFailedCount++;
        }

        if (actualMap > 0 && expedition[actualMap - 1] == "Tome2_SafeZone")
        {
            HudManager.Singleton.merchantHUD.ClearAllSlots();
        }

        transition.StartTransitionTo(allScenes.lobby);
        ClearInstance();

        canGenerateExpedition = true;
    }

    public void SetCanChangeMap(bool state)
    {
        canChangeMap = state;
    }

    [System.Serializable]
    public class AllScenes
    {
        [System.Serializable]
        public class TomeMaps
        {
            public int mapCount;

            public string safeZone;

            public string run;
            public string[] explorations;
            public string[] camps;
            public string[] pirateAttacks;

            public void Load(int tome)
            {
                safeZone = "Tome" + tome + "_SafeZone";
                run = "Tome" + tome + "_Run";

                // Load explos    
                for (int i = 0; i < explorations.Length; i++)
                {
                    explorations[i] = "Tome" + tome + "_Explo" + (i + 1);
                }

                // Load camps
                for (int i = 0; i < camps.Length; i++)
                {
                    camps[i] = "Tome" + tome + "_Camp" + (i + 1);
                }

                // Load pirateAttacks
                for (int i = 0; i < pirateAttacks.Length; i++)
                {
                    pirateAttacks[i] = "Tome" + tome + "_Pirate" + (i + 1);
                }

                mapCount = explorations.Length + camps.Length + pirateAttacks.Length;
            }

            public List<string> GetAllMaps()
            {
                List<string> allMaps = new List<string>();

                for (int i = 0; i < explorations.Length; i++)
                {
                    allMaps.Add(explorations[i]);
                }
                for (int i = 0; i < camps.Length; i++)
                {
                    allMaps.Add(camps[i]);
                }
                for (int i = 0; i < pirateAttacks.Length; i++)
                {
                    allMaps.Add(pirateAttacks[i]);
                }
                return allMaps;
            }
        }

        public string lobby;
        public TomeMaps tome1;
        public TomeMaps tome2;
        public string boss;

        public void Load()
        {
            lobby = "Lobby";
            tome1.Load(1);
            tome2.Load(2);
            boss = "Boss";
        }
    }
}