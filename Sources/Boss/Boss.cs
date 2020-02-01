using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Controller2D))]
public class Boss : MonoBehaviour
{
    protected Animator animator;
    public State state;
    protected bool lookingRight = false;
    protected Controller2D controller;
    private Vector3 scale = Vector3.one;

    public EventBossCombat eventBoss;


    [SerializeField]
    protected StatsBoss stats;
    [Space]
    public Transform targetFocus;
    public Transform entityCenter;

    public StatsBoss Stats { get { return stats; } }

    protected bool BlockDamageReceived;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        targetFocus = GameManager.Singleton.Player.transform;

        controller = GetComponent<Controller2D>();
        stats.Update();
    }

    protected virtual void Update()
    {
        if (lookingRight)
        {
            scale.x = 1;
            transform.localScale = scale;
        }
        else
        {
            scale.x = -1;
            transform.localScale = scale;
        }

        stats.velocity.y += controller.Gravity * Time.deltaTime;

        if (state != State.escape)
        {

            controller.Move(stats.velocity * Time.deltaTime);

            if (controller.collisions.below || controller.collisions.above)
            {
                stats.velocity.y = 0F;
            }
        }
        else
        {
            transform.Translate(stats.velocity * Time.deltaTime);
        }

    }

    public virtual void ActivateBoss()
    {
        if (state == State.waitingPlayer)
        {
            state = State.walk;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!BlockDamageReceived)
        {
            if (stats.health > 0F)
            {
                int finalDamage = Mathf.CeilToInt(stats.ArmorDamageReduction(damage));
                stats.health -= finalDamage;

                // Feedback
                CanvasWorld.Singleton.damageTextManager.AddDamageText(finalDamage, transform);

                if (stats.health <= 0F)
                {
                    state = State.defeated;
                    stats.velocity = Vector2.zero;
                    animator.Play("Defeated");
                    GameManager.Singleton.Player.stats.EarnXP(stats.GetXpToGive());
                    GameManager.Singleton.gameData.gameProgress.bossDefeatedCount++;
                }
            }
        }
    }


    public enum State
    {
        waitingPlayer,
        idle,
        walk,
        attack,
        defeated,
        escape,
        dead
    }
}

[System.Serializable]
public class StatsBoss
{
    [Header("stats")]
    [Space]
    public Vector2 velocity;
    public float speed;
    public bool movingRight;
    public int maxHealth;
    public int health;
    public int armor;
    public int actualAttack;
    [Space]
    [SerializeField] float xpToGive = 4000f;

    public void Update()
    {
        maxHealth = Mathf.CeilToInt(maxHealth * GameManager.Singleton.gameData.GetEnemiesStatsFactor());
        health = maxHealth;

        armor = Mathf.CeilToInt(armor * GameManager.Singleton.gameData.GetEnemiesStatsFactor());
    }

    public float GetXpToGive()
    {
        return xpToGive * GameManager.Singleton.gameData.GetEnemiesStatsFactor();
    }

    public float ArmorDamageReduction(float rawDamage)
    {
        float netDamage; // The damage dealt after reduction
        float drFactor; // The damage reduction factor

        // Calculate the damage reduction factor

        drFactor = armor / (armor + 10 * rawDamage);

        // Calculate the damage after reduction

        netDamage = rawDamage - rawDamage * drFactor;

        return netDamage;
    }
}

[System.Serializable]
public class BossAttack
{
    public string name;
    public Vector2 knockBackForce;
    public float maxAttackRange;
    public float minAttackRange;
    public float stunTime;
    public float cooldown;
    public float currentCooldown;
    public int damage;
    public bool knockBack;
    public bool stun;
    public float timeToWait;

    public void Update()
    {
        damage = Mathf.CeilToInt(damage * GameManager.Singleton.gameData.GetEnemiesStatsFactor());
    }
}
