using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    [SerializeField] private Collider2D AttackBoxCollider;
    [SerializeField] private ContactFilter2D hitFilter;
    [Header("COMBOS ATTACKS")]
    [Space]
    public int comboID;
    public int comboAttackID;
    public int clickCount;
    [Space]
    public float timeToSwitchAttack;
    float currentTimeToSwitch = 0f;
    public float attackCooldown;
    float currentAttackCoolDown;
    [Space]
    public bool canAttack = true;
    public bool comboActive = false;
    public bool checkHit = false;
    [Space]
    public List<ComboAttack> meleeCombos;
    public Skills[] skills = new Skills[2]; 

    Player player;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    public void UpdateFight()
    {
        CheckHit();
        ComboUpdate();
        UpdateSkills();
    }

    void ComboUpdate()
    {
        if (comboActive && canAttack)
        {
            if (currentTimeToSwitch < timeToSwitchAttack)
            {
                currentTimeToSwitch += Time.deltaTime;
            }
            else
            {
                EndCombo();
            }
        }
        else if (!comboActive && !canAttack)
        {
            if (currentAttackCoolDown < attackCooldown)
            {
                currentAttackCoolDown += Time.deltaTime;
            }
            else
            {
                currentAttackCoolDown = 0f;
                canAttack = true;
            }
        }
    }

    void UpdateSkills()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null)
            {
                skills[i].UpdateCoolDown();
            } 
        }
    }
    void ComboCanBeUsed()
    {
        canAttack = true;
    }

    void StopComboVelocity()
    {
        if (comboAttackID > -1 && comboID > -1)
        {
            Attack actualAttack = meleeCombos[comboID].attacks[comboAttackID];
            actualAttack.applyVelocity = false;
        }
    }

    void ApplyComboVelocityY()
    {
        Attack actualAttack = meleeCombos[comboID].attacks[comboAttackID];
        player.stats.velocity.y = actualAttack.velocity.y;
    }

    void PlayComboAttackSound()
    {

    }

    bool CalculateCritDamages(ref int damages)
    {
        bool isCrit;
        isCrit = Random.Range(0, 100) <= player.stats.criticalChances;

        if (isCrit)
        {
            damages = Mathf.CeilToInt(damages * player.stats.criticalDamagesM);
        }

        return isCrit;
    }

    // Verify if the Attack collider is collide with something
    void CheckHit()
    {
        if (checkHit)
        {
            Collider2D[] collider2Ds = new Collider2D[200];

            Physics2D.OverlapCollider(AttackBoxCollider, hitFilter, collider2Ds);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider != null)
                {
                    Enemy enemyHit = collider.GetComponent<Enemy>();
                    Boss bossHit = collider.GetComponent<Boss>();
                    ChestAndBox chestAndBoxHit = collider.GetComponent<ChestAndBox>();

                    // Enemy Hit
                    if (enemyHit != null)
                    {
                        Vector2 playerCenter = (Vector2)player.centerTransform.position;
                        Vector2 enemyPos = enemyHit.transform.position;
                        Vector2 vector = enemyPos - playerCenter;
                        Vector2 dir = vector.normalized;
                        float dist = vector.magnitude;

                        RaycastHit2D hit = Physics2D.Raycast(player.centerTransform.position, dir, dist, LayerMask.NameToLayer("mapColliders"));
                        if (!hit)
                        {
                            int damages = Mathf.CeilToInt(player.stats.normalDamages * meleeCombos[comboID].attacks[comboAttackID].damagesMultiplier * player.stats.normalDamagesMultiplier);
                            bool isCrit = CalculateCritDamages(ref damages);

                            if (isCrit)
                            {
                                CanvasWorld.Singleton.damageTextManager.GenerateCritFeedback(enemyHit.centerEntity);
                            }

                            // FeedBacks
                            enemyHit.TakeDamage(damages);

                            StopComboVelocity();
                            player.stats.velocity = Vector2.zero;
                        }
                    }

                    if (bossHit != null)
                    {
                        Vector2 playerCenter = (Vector2)player.centerTransform.position;
                        Vector2 bossPos = bossHit.transform.position;
                        Vector2 vector = bossPos - playerCenter;
                        Vector2 dir = vector.normalized;
                        float dist = vector.magnitude;

                        RaycastHit2D hit = Physics2D.Raycast(player.centerTransform.position, dir, dist, LayerMask.NameToLayer("mapColliders"));

                        if (!hit)
                        {
                            int damages = Mathf.CeilToInt(player.stats.normalDamages * meleeCombos[comboID].attacks[comboAttackID].damagesMultiplier * player.stats.normalDamagesMultiplier);
                            bool isCrit = CalculateCritDamages(ref damages);

                            if (isCrit)
                            {
                                CanvasWorld.Singleton.damageTextManager.GenerateCritFeedback(bossHit.entityCenter);
                            }

                            // FeedBacks
                            bossHit.TakeDamage(damages);

                            StopComboVelocity();
                            player.stats.velocity = Vector2.zero;
                        }
                    }

                    if (chestAndBoxHit != null)
                    {
                        chestAndBoxHit.Hit();
                    }
                }
            }

            checkHit = false;
        }
    }

    void HasToCheckHit()
    {
        checkHit = true;
    }

    public void FightVelocityUpdate()
    {
        if (comboActive)
        {
            Attack actualAttack = meleeCombos[comboID].attacks[comboAttackID];
            player.stats.velocity.x = 0f;

            if (actualAttack.applyVelocity)
            {
                player.stats.velocity.x = actualAttack.velocity.x * (player.stats.isFacingRight ? 1 : -1);
            }

        }
    }

    public bool IsComboIgnoringGravity()
    {
        bool isIgnoringGravity = false;

        if (comboActive)
        {
            isIgnoringGravity = meleeCombos[comboID].attacks[comboAttackID].ignoreGravity;
        }

        return isIgnoringGravity;
    }

    public void EndCombo()
    {
        CancelCombo();

        if (player.AnimatorRef.GetBool("OnGround"))
        {
            if (player.AnimatorRef.velocity.x <= 1f)
            {
                player.AnimatorRef.Play("Idle", 0);
            }
            else
            {
                player.AnimatorRef.Play("Run", 0);
            }
        }
        else
        {
            player.AnimatorRef.Play("Jump_Fall", 0);
        }
    }

    public void CancelCombo()
    {
        comboActive = false;
        clickCount = 0;
        comboAttackID = -1;
        currentTimeToSwitch = 0f;
    }

    public void UseCombo()
    {
        if (canAttack)
        {
            if (!comboActive)
            {
                comboActive = true;
                if (player.AnimatorRef.GetBool("OnGround"))
                {
                    comboID = 0;
                }
                else
                {
                    comboID = 1;
                }

                player.AnimatorRef.Play(meleeCombos[comboID].attacks[0].animationName);
            }

            canAttack = false;
            currentTimeToSwitch = 0f;
            clickCount++;
            comboAttackID++;
            meleeCombos[comboID].attacks[comboAttackID].applyVelocity = true;
            player.AnimatorRef.SetInteger("ComboAttack ID", comboAttackID + 1);
        }
    }
}
