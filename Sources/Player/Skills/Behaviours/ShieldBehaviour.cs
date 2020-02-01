using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehaviour : MonoBehaviour
{
    float currentActivationTime = 0f;
    Skill_Shield refSkill;

    Player player;

    // Update is called once per frame
    void LateUpdate()
    {
        if (refSkill != null && refSkill.IsActive)
        {
            if (player.stats.damageAbsorptionAmount <= 0f)
            {
                Disable();
            }

            if (currentActivationTime > 0f)
            {
                currentActivationTime -= Time.deltaTime;
            }
            else
            {
                Disable();
            }
        }
    }

    public void Init(Player _player, Skill_Shield _skill)
    {
        player = _player;
        refSkill = _skill;
        refSkill.SetActive(true);
        currentActivationTime = _skill.activationTime;
    }

    public void Disable()
    {
        player.stats.damageAbsorptionActive = false;
        player.stats.damageAbsorptionAmount = 0f;
        refSkill.StartCooldown(player.stats.cooldownReduction);
        refSkill.canBeUsed = false;
        refSkill.SetActive(false);

        Destroy(gameObject);
    }
}
