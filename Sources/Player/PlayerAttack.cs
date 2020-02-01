using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboAttack
{
    [Header("GENERAL COMBO DATAS")]
    public string name;
    public int hitCount;
    [Space]
    public List<Attack> attacks;
}

[System.Serializable]
public class Attack
{
    [Header("INFORMATIONS")]
    public string name = "Attack";
    public string animationName = "";
  
    [Header("PLAYER ALTERATION")]
    public bool ignoreGravity;
    [Space]
    public Vector2 velocity;
    public bool applyVelocity;

    [Header("OFFENSIVES STATS")]
    [Range(0.5f, 3f)]
    public float damagesMultiplier = 1f;
    public Vector2 RepulseForce;

    [Header("FEEDBACKS")]
    public List<AudioClip> soundsToPlay;
}
