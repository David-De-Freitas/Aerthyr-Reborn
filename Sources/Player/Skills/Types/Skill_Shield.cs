using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Player Skills/Shield")]
public class Skill_Shield : Skills
{
    [SerializeField] Object prefab;
    [Space]
    [Header("DATA")]
    [SerializeField] float healthFactor;
    public float activationTime;
   
    ShieldBehaviour behaviour;

    public override void Use(Player player)
    {
        if (canBeUsed)
        {
            if (!isActive)
            {
                player.stats.damageAbsorptionActive = true;
                player.stats.damageAbsorptionAmount = Mathf.CeilToInt(player.stats.healthMax * healthFactor);

                GameObject shieldGO = (GameObject)Instantiate(prefab, player.centerTransform) as GameObject;
                shieldGO.transform.localPosition = Vector3.zero + Vector3.up * 0.3f;
                behaviour = shieldGO.GetComponent<ShieldBehaviour>();

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

        description = "Cree un bouclier protecteur qui absorbe les degats a hauteur de " + healthFactor + " fois la sante maximum pendant " + activationTime + " secondes.";

        return description;
    }

    public void StartCooldown(float cooldownReductionValue)
    {
        canBeUsed = false;
        UpdateCooldownTime(cooldownReductionValue);
        currentCooldown = cooldown;
    }
}
