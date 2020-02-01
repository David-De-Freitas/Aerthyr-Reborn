using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Player Skills/Fury")]
public class Skill_Fury : Skills
{
    [SerializeField] Object prefab;
    [Space]
    [Header("DATA")]
    [SerializeField] float damageMultiplier;
    [SerializeField] float critChanceMultiplier;
    [SerializeField] float staminaRegenMultiplier;

    public float activationTime;

    FuryBehaviour behaviour;

    public float DamageMultiplier { get { return damageMultiplier; } }
    public float CritChanceMultiplier { get { return critChanceMultiplier; } }
    public float StaminaRegenMultiplier { get { return staminaRegenMultiplier; } }


    public override void Use(Player player)
    {
        if (canBeUsed)
        {
            if (!isActive)
            {
                player.stats.normalDamagesMultiplier += damageMultiplier - 1;
                player.stats.criticalChanceMultiplier += critChanceMultiplier - 1;
                player.stats.staminaRegenMultiplier += staminaRegenMultiplier - 1;

                HudManager.Singleton.statsDisplayer.normalDamage.affectedByFury = true;
                HudManager.Singleton.statsDisplayer.criticalChance.affectedByFury = true;
                HudManager.Singleton.statsDisplayer.staminaRegen.affectedByFury = true;

                HudManager.Singleton.statsDisplayer.UpdateStats();

                GameObject go = (GameObject)Instantiate(prefab, player.transform) as GameObject;
                go.transform.localPosition = Vector3.zero;
                behaviour = go.GetComponent<FuryBehaviour>();

                behaviour.Init(player, this);

                isActive = true;
            }
            else
            {
                if (behaviour != null)
                {
                    behaviour.Disable();
                }
            }
        }
    }

    public override void UpdateCoolDown()
    {
        if (!canBeUsed)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
            else
            {
                canBeUsed = true;
            }
        }
    }

    public override string GetDescription()
    {
        string description;

        description = "Libere une rage destructrice pendant " + activationTime + " secondes.\n" +
            "Degats normaux : " + DamageMultiplier + " fois\n" +
            "Chance de coup critique : " + critChanceMultiplier + " fois\n" +
            "Regeneration d'energie : " + staminaRegenMultiplier + " fois"; 

        return description;
    }

    public void StartCooldown(float cooldownReductionValue)
    {
        canBeUsed = false;
        UpdateCooldownTime(cooldownReductionValue);
        currentCooldown = cooldown;
    }
}
