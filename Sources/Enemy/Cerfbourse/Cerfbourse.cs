using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerfbourse : Enemy
{
    [Header("Info Cerfbourse")]
    public State state = State.fall;
    public State prevState = State.fall;

    [Space]
    [Header("Combat info")]
    public Object taupard;
    public float rangeAgro;
    public float speedFactor;
    public bool checkHit;
    public StatsAttack[] statsAttack;
    private ContactFilter2D hitFilter;
    bool taupardBlock;
    bool wallBlock;

    protected override void Start()
    {
        base.Start();

        species = Species.Cerfbourse;
        onDeath += OnDeathInvoke;

        for (int i = 0; i < statsAttack.Length; i++)
        {
            statsAttack[i].Update(damageFactor);
        }
    }

    protected override void Update()
    {
        if (stats.lifeState == StatsEnemy.LifeState.alive)
        {
            StateMachine();
            UpdateAnimation();
        }
        else
        {
            base.Update();
        }
    }

    private void OnDeathInvoke()
    {
        GameDifficulty actualDiff = GM.gameData.GetGameDifficulty();

        switch (type)
        {
            case Type.Wild:
                GM.gameData.GetCurrentGameDifficultyProgress().cerfbourseBasicDefeatedCount++;
                break;
            case Type.Pirate:
                GM.gameData.GetCurrentGameDifficultyProgress().cerfboursePirateDefeatedCount++;
                break;
            default:
                break;
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(stats.velocity.x));
        animator.SetBool("OnGround", controller.collisions.below);
        animator.SetBool("SeeFocus", state == State.seeFocus);
    }

    void UpdateAiMove()
    {
        RaycastHit2D hit;

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        hit = GroundDetection();


        if (!hit)
        {
            stats.speed = 4;
            SwitchToState(State.waiting);
            needToReturn = true;
        }
        else if (controller.collisions.right || controller.collisions.left)
        {
            stats.speed = 4;
            SwitchToState(State.waiting);
            if (stats.movingRight == true)
            {
                stats.movingRight = false;
            }
            else
            {
                stats.movingRight = true;
            }
        }
        else
        {
            stats.velocity.x = stats.speed * factorVel;
        }
    }

    void UpdateSeeFocus()
    {
        taupardBlock = false;
        RaycastHit2D hit;
        RaycastHit2D hitBlock;
        float distance = Vector2.Distance(eyes.position, targetCenter.position);

        if (targetFocus.transform.position.x >= transform.position.x)
        {
            if (stats.movingRight == false)
            {
                stats.movingRight = true;
            }
        }
        else
        {
            if (stats.movingRight == true)
            {
                stats.movingRight = false;
            }
        }

        UpdateScale();

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        hit = GroundDetection();

        hitBlock = BlockDetection();

        if (!hit)
        {
            taupardBlock = true;
            stats.speed = 4;
            SwitchToState(State.waiting);
            needToReturn = true;
        }
        else if (controller.collisions.right || controller.collisions.left || hitBlock)
        {
            taupardBlock = true;
            stats.speed = 4;
            //SwitchToState(State.waiting);
            //needToReturn = true;
            wallBlock = true;
            stats.velocity.x = 0;
        }
        else
        {
            float distanceX = Mathf.Abs(targetCenter.position.x - transform.position.x);
            if (distanceX >= statsAttack[0].attackRange - 0.3f)
            {
                stats.velocity.x = stats.speed * factorVel * speedFactor;
            }
            else if (distanceX <= statsAttack[0].attackRange - 1f)
            {
                stats.velocity.x = 0;
            }

        }
    }

    void UpdateCoolDown()
    {
        foreach (StatsAttack attack in statsAttack)
        {
            if (attack.currentCooldown > 0)
            {
                attack.currentCooldown -= Time.deltaTime;
            }
        }
    }

    void UpdateAttackCac()
    {
        CheckHit();
    }

    void UpdateAgro()
    {
        float distance = Vector2.Distance(centerEntity.position, targetFocus.transform.position);

        if (distance <= rayAgrooLenght)
        {
            AgroInfo agroInfo = CheckAgro();
            if (!agroInfo.hit)
            {
                bool lookingTarget = false;
                if ((targetFocus.transform.position.x > centerEntity.position.x) == stats.movingRight )
                {
                    lookingTarget = true;
                }

                if (distance <= statsAttack[0].attackRange && statsAttack[0].currentCooldown <= 0 && lookingTarget)
                {
                    statsAttack[0].currentCooldown = statsAttack[0].cooldown;
                    statsAttack[1].currentCooldown += 1.2f;
                    animator.Play("AttackCac");
                    stats.actualAttack = 0;
                    SwitchToState(State.attackCac);
                }
                else if (distance >= statsAttack[0].attackRange + 2f && distance <= statsAttack[1].attackRange && statsAttack[1].currentCooldown <= 0
                    && targetFocus.GetCollisions().below && !taupardBlock && lookingTarget)
                {
                    statsAttack[1].currentCooldown = statsAttack[1].cooldown;
                    animator.Play("AttackDist");
                    stats.actualAttack = 1;
                    SwitchToState(State.attackDist);
                }
                else
                {
                    if (distance <= rangeAgro && state != State.seeFocus && lookingTarget)
                    {
                        animator.Play("Idle");
                        SwitchToState(State.seeFocus);
                    }
                }
            }
            else
            {
                if (state == State.seeFocus)
                {
                    animator.Play("Idle");
                    SwitchToState(State.walking);
                }
            }
        }
        else if (state == State.seeFocus)
        {
            animator.Play("Idle");
            SwitchToState(State.walking);
        }
    }

    void UpdateWaiting()
    {
        stats.speed = 0;
        if (!timeToWaitSet)
        {
            timeToWait = Random.Range(minTimeToWait, maxTimeToWait);
            timeToWaitSet = true;
            timerWaiting = 0F;
            stats.velocity.x = 0F;
        }

        timerWaiting += Time.deltaTime;


        if (timerWaiting >= timeToWait)
        {
            timerWaiting = 0F;
            timeToWaitSet = false;

            SwitchToState(State.walking);

            if (needToReturn)
            {
                timerBeforeWaiting = 0F;
                needToReturn = false;

                if (stats.movingRight == true)
                {
                    stats.movingRight = false;
                }
                else
                {
                    stats.movingRight = true;
                }
            }
            else
            {
                if (Random.Range(float.MinValue, float.MaxValue) > float.MinValue / 1.5)
                {
                    if (stats.movingRight == true)
                    {
                        stats.movingRight = false;
                    }
                    else
                    {
                        stats.movingRight = true;
                    }
                }
            }
        }
    }

    void UpdateBeforeWaiting()
    {

        RaycastHit2D hit;

        hit = GroundDetection();

        if (hit)
        {
            if (!timeBeforeWaitSet)
            {
                timeBeforeWait = Random.Range(minTimeBeforeWait, maxTimeBeforeWait);
                timeBeforeWaitSet = true;
                timerBeforeWaiting = 0;
            }

            timerBeforeWaiting += Time.deltaTime;

            if (timerBeforeWaiting >= timeBeforeWait)
            {
                timeBeforeWaitSet = false;
                SwitchToState(State.waiting);
            }
        }
    }

    void UpdateFall()
    {
        if (controller.collisions.below)
        {
            SwitchToState(State.walking);
        }
    }

    void StateMachine()
    {
        switch (state)
        {
            case State.attackCac:
                UpdateAttackCac();
                break;
            case State.walking:
                UpdateAiMove();
                UpdateBeforeWaiting();
                UpdateAgro();
                break;
            case State.waiting:
                UpdateWaiting();
                UpdateAgro();
                break;
            case State.seeFocus:
                UpdateSeeFocus();
                UpdateAgro();
                break;
            case State.fall:
                UpdateFall();
                break;
            default:
                break;
        }
        UpdateCoolDown();
        base.Update();
    }

    public void InstanciateTaupard()
    {
        GameObject newTaupard = (GameObject)Instantiate(taupard, transform.parent) as GameObject;
        newTaupard.GetComponent<Taupard>().Init(stats.movingRight, transform, statsAttack[1]);
    }

    private void SwitchToState(State newState)
    {
        stats.velocity.x = 0F;
        stats.speed = 4;
        prevState = state;
        state = newState;
    }

    void CheckHit()
    {
        if (checkHit)
        {
            Collider2D[] collider2Ds = new Collider2D[200];

            Physics2D.OverlapCollider(attackBox, hitFilter, collider2Ds);
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider != null)
                {
                    Player playerHit = collider.GetComponent<Player>();

                    if (playerHit != null)
                    {
                        int factorMove = (stats.movingRight) ? 1 : -1;
                        Vector2 force = statsAttack[0].knockBackForce;
                        force.x *= factorMove;
                        playerHit.TakeDamage(statsAttack[0].damage, force);
                        break;
                    }
                }
            }
            checkHit = false;
        }
    }

    void CheckHitEnabled()
    {
        checkHit = true;
    }

    void EndTakeDamage()
    {
        SwitchToState(prevState);
        animator.Play("Idle");
    }

    public override void TakeDamage(int damage)
    {
        if (state != State.attackCac && state != State.attackDist)
        {
            base.TakeDamage(damage);
            if (state != State.takeDamage)
            {
                SwitchToState(State.takeDamage);
            }
        }
        else
        {
            if (stats.health > 0F)
            {
                if (stats.movingRight != (transform.position.x < targetFocus.transform.position.x))
                {
                    stats.movingRight = !stats.movingRight;
                    UpdateScale();
                }
                stats.health -= damage;

                //Feedback
                CanvasWorld.Singleton.damageTextManager.AddDamageText(damage, centerEntity);

                if (stats.health <= 0F)
                {
                    SpriteRenderer deadSprite = deadSpriteParent.GetComponentInChildren<SpriteRenderer>();
                    stats.lifeState = StatsEnemy.LifeState.dying;
                    GetComponent<SpriteRenderer>().enabled = false;
                    deadSprite.enabled = true;
                    deadFactor = -1;
                    if (targetCenter.position.x >= transform.position.x)
                    {
                        deadFactor = 1;
                    }
                    Vector3 scale = Vector3.one;
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

                    // Drop item(s) from the loot table if have to.
                    if (lootTable != null)
                    {
                        lootTable.Drop(transform.position + Vector3.up * 1.5f);
                    }


                    stats.velocity.x = 8F * deadFactor * -1;
                    stats.velocity.y = 15F;
                    animator.Play("TakeDamage");
                }
            }
        }
    }

    public enum State
    {
        attackDist,
        attackCac,
        walking,
        running,
        waiting,
        seeFocus,
        fall,
        takeDamage,
    }
}
