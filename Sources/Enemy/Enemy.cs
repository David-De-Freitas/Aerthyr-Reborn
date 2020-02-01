using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Controller2D))]
public class Enemy : MonoBehaviour
{
    protected delegate void OnDeath();
    protected event OnDeath onDeath;

    public enum Species
    {
        Loulpe, Castureuil, Cerfbourse, Taupard
    }

    public enum Type
    {
        Wild, Pirate
    }

    protected Animator animator;
    protected Controller2D controller;
    protected GameManager GM;
    protected Player player;

    protected float healthFactor;
    protected float damageFactor;

    new protected SpriteRenderer renderer;

    public Species species;
    public Type type;
    [Space]
    [SerializeField] protected LootTable lootTable;
    public BoxCollider2D attackBox;
    public Transform DamageEmit;
    public Transform centerEntity;
    public Transform deadSpriteParent;

    [SerializeField]
    public StatsEnemy stats;
    [Space]
    [Header("ground detection")]
    public Transform groundDetection;
    public Transform groundDetection2;
    public Transform blockDetection;
    public float rayGroundLenght = 1.2F;
    public bool needToReturn = false;
    public bool blockReturn = false;
    [Space]
    [Header("detection target")]
    public Transform eyes;
    public Player targetFocus;
    public Transform targetCenter;
    public LayerMask layerMask;
    public float rayAgrooLenght;
    public float angleVision;
    [Space]
    [Header("waiting")]
    public float minTimeToWait = 1F;
    public float maxTimeToWait = 4F;

    public float minTimeBeforeWait = 1F;
    public float maxTimeBeforeWait = 4F;

    protected float timerWaiting = 0F;
    protected float timeToWait = 0F;
    protected float timerBeforeWaiting = 0F;
    protected float timeBeforeWait = 0F;

    protected bool timeToWaitSet = false;
    protected bool timeBeforeWaitSet = false;

    protected float deadFactor;
    private Vector3 scale;

    private Rect CameraLimit;
    // Use this for initialization
    protected virtual void Start()
    {
        GM = GameManager.Singleton;
        player = GM.Player;
        targetFocus = player;
        targetCenter = targetFocus.centerTransform;
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller2D>();
        renderer = GetComponent<SpriteRenderer>();
        CameraLimit = Camera.main.GetComponent<Camera2D>().cameraLimits;

        healthFactor = 1.05f;
        damageFactor = 1f;

        scale = Vector3.one;

        stats.Update(healthFactor);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (stats.lifeState == StatsEnemy.LifeState.alive)
        {
            stats.velocity.y += controller.Gravity * Time.deltaTime;

            if (stats.movingRight)
            {
                scale.x = 1;
                transform.localScale = scale;
            }
            else
            {
                scale.x = -1;
                transform.localScale = scale;
            }

            controller.Move(stats.velocity * Time.deltaTime);

            if (controller.collisions.below || controller.collisions.above)
            {
                stats.velocity.y = 0F;
            }
        }
        else if (stats.lifeState == StatsEnemy.LifeState.dying)
        {
            stats.velocity.y += controller.Gravity * Time.deltaTime;
            transform.Translate(stats.velocity * Time.deltaTime);
     
            deadSpriteParent.Rotate(Vector3.forward, Time.deltaTime * 800 * deadFactor);
                    
        }
        if (!CameraLimit.Contains(transform.position) && transform.position.y < CameraLimit.yMax)
        {
            Destroy(gameObject);
        }
    }

    protected AgroInfo CheckAgro()
    {
        AgroInfo agroInfo;

        agroInfo.rayLenght = Vector2.Distance(eyes.position, targetCenter.position);
        agroInfo.directionRay = targetCenter.position - eyes.position;

        agroInfo.directionRay = agroInfo.directionRay.normalized;

        agroInfo.raycast = Physics2D.Raycast(eyes.position, agroInfo.directionRay, agroInfo.rayLenght, layerMask);
        agroInfo.angle = Mathf.Abs(Vector3.Angle(Vector3.right, agroInfo.directionRay));
        agroInfo.hit = agroInfo.raycast;

        if (!agroInfo.hit && Mathf.Abs(Vector3.Angle(Vector3.right * transform.localScale.x, agroInfo.directionRay)) < angleVision)
        {
            Debug.DrawRay(eyes.position, agroInfo.directionRay * agroInfo.rayLenght, Color.red);
            agroInfo.hit = false;
        }
        return agroInfo;
    }

    #region GroundDetection

    protected RaycastHit2D GroundDetection()
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = groundDetection.position;
        position.x += (stats.speed * factorVel) * Time.deltaTime;
        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        return hit;
    }

    protected RaycastHit2D GroundDetection(float actualSpeed)
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = groundDetection.position;
        position.x += ((actualSpeed / 100) * Time.deltaTime * factorVel);
        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        return hit;
    }

    protected RaycastHit2D GroundDetection(Transform originRay)
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = originRay.position;
        position.x += ((stats.speed / 100) * Time.deltaTime * factorVel);
        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        return hit;
    }

    protected RaycastHit2D GroundDetection(bool debug)
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = groundDetection.position;
        position.x += ((stats.speed / 128) * Time.deltaTime * factorVel);
        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        if (debug)
        {
            Debug.DrawRay(position, Vector2.down);
        }
        return hit;
    }

    protected RaycastHit2D GroundDetection(float actualSpeed, bool debug)
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = groundDetection.position;
        position.x += ((actualSpeed / 100) * Time.deltaTime * factorVel);
        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        if (debug)
        {
            Debug.DrawRay(position, Vector2.down);
        }

        return hit;
    }

    protected RaycastHit2D GroundDetection(Transform originRay, float actualSpeed, bool debug)
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = originRay.position;
        position.x += ((actualSpeed / 100) * Time.deltaTime * factorVel);

        hit = Physics2D.Raycast(position, Vector2.down, rayGroundLenght, layerMask);

        if (debug)
        {
            Debug.DrawRay(position, Vector2.down);
        }

        return hit;
    }
    #endregion

    protected RaycastHit2D BlockDetection()
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        Vector3 position = blockDetection.position;
        Vector2 dir = new Vector2(factorVel,0);
        hit = Physics2D.Raycast(position, dir,1f, layerMask);

        return hit;
    }

    protected void UpdateScale()
    {
        if (stats.movingRight)
        {
            scale.x = 1;
            transform.localScale = scale;
        }
        else
        {
            scale.x = -1;
            transform.localScale = scale;
        }
    }

    virtual public void TakeDamage(int damage)
    {
        if (stats.movingRight != (transform.position.x < targetFocus.transform.position.x))
        {
            stats.movingRight = !stats.movingRight;
            UpdateScale();
        }
        if (stats.health > 0F)
        {
            stats.health -= damage;
            stats.velocity.x = 0f;
            //Feedback
            CanvasWorld.Singleton.damageTextManager.AddDamageText(damage, centerEntity);
                 
            if (stats.health <= 0F) // DEATH
            {
                SpriteRenderer deadSprite = deadSpriteParent.GetComponentInChildren<SpriteRenderer>();
                stats.lifeState = StatsEnemy.LifeState.dying;
                GetComponent<SpriteRenderer>().enabled = false;

                deadSprite.enabled = true;
                deadFactor = -1;

                animator.Play("TakeDamage");

                if (targetCenter.position.x >= transform.position.x)
                {
                    deadFactor = 1;
                }

                if (deadFactor == 1)
                {
                    scale.x = 1;
                    transform.localScale = scale;
                }
                else
                {
                    scale.x = -1;
                    transform.localScale = scale;
                }

                deadSprite.sortingLayerName = "Front";
                deadSprite.sortingOrder = 80;

                // Drop item(s) from the loot table.
                if (lootTable != null)
                {
                    lootTable.Drop(transform.position + Vector3.up * 1.5f);
                }

                player.stats.EarnXP(stats.GetXpToGive()); // Give Xp to the player
                GM.gameData.gameProgress.enemiesDefeatedCount++;

                if (type == Type.Pirate)
                {
                    GM.gameData.gameProgress.haveEncounterPirates = true;
                }

                onDeath.Invoke();

                stats.velocity.x = 8F * deadFactor * -1;
                stats.velocity.y = 15F;
            }
            else
            {
                animator.Play("TakeDamage");
                stats.velocity = Vector2.zero;
            }
        }
    }

    public struct AgroInfo
    {
        public bool hit;
        public RaycastHit2D raycast;
        public float angle;
        public float rayLenght;
        public Vector2 directionRay;
    }

}

[System.Serializable]
public class StatsEnemy
{
    [Header("stats")]
    [Space]

    public Vector2 velocity;
    public float speed;
    public bool movingRight;
    public int maxHealth;
    public int health;
    public int actualAttack;
    public LifeState lifeState;
    [Space]
    [SerializeField] float xpToGive;

    public enum LifeState
    {
        alive,
        dying,
        dyingOneShot,
        dead
    }

    /// <summary>
    ///Update the health according to the healthFactor and the actual gameDifficulty.
    /// </summary>
    /// <param name="healthFactor"></param>
    public void Update(float healthFactor)
    {
        maxHealth = Mathf.CeilToInt(maxHealth * GameManager.Singleton.gameData.GetEnemiesStatsFactor() * healthFactor);
        health = maxHealth;
    }

    public float GetXpToGive()
    {
        return xpToGive * GameManager.Singleton.gameData.GetEnemiesStatsFactor();
    }
}

[System.Serializable]

public class StatsAttack
{
    public string name;
    public Vector2 knockBackForce;
    public Vector2 moveSpeed;
    public bool moveEnable;
    public float attackRange;
    public float stunTime;
    public float cooldown;
    public float currentCooldown;
    public int damage;
    public bool knockBack;
    public bool stun;

    /// <summary>
    /// Update the damage according to the damageFactor and the actual gameDifficulty.
    /// </summary>
    /// <param name="damageFactor"></param>
    public void Update(float damageFactor)
    {
        damage = Mathf.CeilToInt(damage * GameManager.Singleton.gameData.GetEnemiesStatsFactor() * damageFactor);
    }
}