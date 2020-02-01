using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplayer : MonoBehaviour
{
    [Header("DEFENSIVES STATS TEXTS")]
    public PlayerStatsText health;
    public PlayerStatsText healthRegen;
    public PlayerStatsText stamina;
    public PlayerStatsText staminaRegen;
    public PlayerStatsText armor;
    [Header("OFFENSIVE STATS TEXTS")]
    public PlayerStatsText normalDamage;
    public PlayerStatsText criticalChance;
    public PlayerStatsText criticalDamage;
    public PlayerStatsText capacityDamage;
    public PlayerStatsText cooldownReduc;

    Player player;
    CanvasGroup canvasGroup;

    // Use this for initialization
    void Start ()
    {
        player = GameManager.Singleton.Player;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        UpdateStats();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void UpdateStats()
    {
        health.SetStats(player.stats.healthMax, player.stats.baseHealthMax, "", "");
        healthRegen.SetStats(player.stats.healthRegen, player.stats.baseHealthRegen, "", " /sec");
        stamina.SetStats(player.stats.staminaMax, player.stats.baseStaminaMax, "", "");
        staminaRegen.SetStats(player.stats.staminaRegen * player.stats.staminaRegenMultiplier, player.stats.baseStaminaRegen, "", " /sec");
        armor.SetStats(player.stats.armor, player.stats.baseArmor, "", "");

        normalDamage.SetStats(player.stats.normalDamages * player.stats.normalDamagesMultiplier, player.stats.baseNormalDamages, "", "");
        criticalChance.SetStats(player.stats.criticalChances * player.stats.criticalChanceMultiplier, player.stats.baseCriticalChances, "", " %");
        criticalDamage.SetStats(player.stats.criticalDamagesM, player.stats.baseCriticalDamagesM, "x ", "");
        capacityDamage.SetStats(player.stats.capacitiesDamagesM, player.stats.baseCapacitiesDamagesM, "x ", "");
        cooldownReduc.SetStats(player.stats.cooldownReduction, player.stats.baseCooldownReduction, "", " %");
    }

    public void OpenClose()
    {
        canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0;
    }
}
