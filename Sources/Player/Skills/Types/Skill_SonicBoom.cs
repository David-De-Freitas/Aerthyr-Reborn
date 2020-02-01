using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Player Skills/SonicBoom")]
public class Skill_SonicBoom : Skills
{
    [SerializeField] Object prefab;
    [SerializeField] string animationToLaunch;
    [Space]
    [Header("DATA")]
    [SerializeField] float damages;

    private SonicBoomBehaviour behaviour;

    public override void Use(Player player)
    {
        if (CanBeUsed)
        {
            UpdateCooldownTime(player.stats.cooldownReduction);

            GameObject go = (GameObject)Instantiate(prefab) as GameObject;
            go.transform.position = player.centerTransform.position;
       
            behaviour = go.GetComponent<SonicBoomBehaviour>();
            behaviour.Init(player, damages);

            canBeUsed = false;
            currentCooldown = cooldown;
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
        description = "Projete une boule d'energie qui inflige " + damages + " points de degats a l'impact puis explose.\n" +
            "Inflige des degats supplementaires aux ennemis dans la zone d'explosion.\n" +
            "Passe a travers le terrain.";
        return description;
    }
}
