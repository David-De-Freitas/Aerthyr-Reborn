using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skills : ScriptableObject
{
    public bool canBeUsed = true;
    [SerializeField] protected bool allowMultiUse = true;
    [SerializeField] protected bool isActive = false;
    public bool CanBeUsed { get { return canBeUsed; } }
    public bool AllowMultiUse { get { return allowMultiUse; } }
    public bool IsActive { get { return isActive; } }

    [SerializeField] protected float baseCooldown;
    protected float cooldown;
    protected float currentCooldown; 

    public abstract void Use(Player player);

    public abstract void UpdateCoolDown();

    public abstract string GetDescription();

    public void SetActive(bool state)
    {
        isActive = state;
    }

    protected void UpdateCooldownTime(float cooldownReductionValue)
    {
        cooldown = baseCooldown * 1f - cooldownReductionValue / 100f;
    }
}
