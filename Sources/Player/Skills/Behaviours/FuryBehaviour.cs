using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuryBehaviour : MonoBehaviour
{
    float currentActivationTime = 0f;
    Skill_Fury refSkill;

    Player player;

    // Update is called once per frame
    void LateUpdate()
    {
        if (refSkill != null && refSkill.IsActive)
        {          
            if (currentActivationTime > 0f)
            {
                currentActivationTime -= Time.deltaTime;

                HudManager.Singleton.statsDisplayer.normalDamage.temporyAffected = true;
                HudManager.Singleton.statsDisplayer.criticalChance.temporyAffected = true;
                HudManager.Singleton.statsDisplayer.staminaRegen.temporyAffected = true;

                HudManager.Singleton.statsDisplayer.UpdateStats();
            }
            else
            {
                Disable();
            }
        }
    }

    public void Init(Player _player, Skill_Fury _skill)
    {
        player = _player;
        refSkill = _skill;
        refSkill.SetActive(true);
        currentActivationTime = _skill.activationTime;
    }

    public void Disable()
    {
        player.stats.normalDamagesMultiplier -= refSkill.DamageMultiplier - 1;
        player.stats.criticalChanceMultiplier -= refSkill.CritChanceMultiplier - 1;
        player.stats.staminaRegenMultiplier -= refSkill.StaminaRegenMultiplier - 1;

        HudManager.Singleton.statsDisplayer.normalDamage.affectedByFury = false;
        HudManager.Singleton.statsDisplayer.criticalChance.affectedByFury = false;
        HudManager.Singleton.statsDisplayer.staminaRegen.affectedByFury = false;

        HudManager.Singleton.statsDisplayer.normalDamage.temporyAffected = false;
        HudManager.Singleton.statsDisplayer.criticalChance.temporyAffected = false;
        HudManager.Singleton.statsDisplayer.staminaRegen.temporyAffected = false;

        HudManager.Singleton.statsDisplayer.UpdateStats();

        refSkill.StartCooldown(player.stats.cooldownReduction);
        refSkill.SetActive(false);
        refSkill.canBeUsed = false;

        Destroy(gameObject);
    }

}
