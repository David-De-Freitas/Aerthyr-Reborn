using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castureuil : Enemy
{

    [Header("Info Castureuil")]
    public float jumpForce;
    public State state = State.fall;
    public Transform groundBackDetection;
    public Transform firePos;
    float factorVelAnim = 1;
    [Space]

    [Header("Type of Castureuil")]
    public bool isStatic;

    [Space]
    [Header("Combat info")]
    State prevState = State.fall;
    public bool escape;
    public string actualState;
    private bool checkHit = false;
    private ContactFilter2D hitFilter;
    [Space]
    public StatsAttack[] statsAttack;
    public Object nutPrefab;


    protected override void Start()
    {
        base.Start();
        species = Species.Castureuil;

        onDeath += OnDeathInvoke;
        // a revoir
        if (groundDetection == null)
        {
            groundDetection = transform.GetComponentInChildren<Transform>();
        }

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
                GM.gameData.GetCurrentGameDifficultyProgress().castureuilBasicDefeatedCount++;
                break;
            case Type.Pirate:
                GM.gameData.GetCurrentGameDifficultyProgress().castureuilPirateDefeatedCount++;
                break;
            default:
                break;
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(stats.velocity.x));
        animator.SetBool("OnGround", controller.collisions.below);

        animator.SetBool("Walking", (state == State.walking || (state == State.combat && Mathf.Abs(stats.velocity.x) > 0) || factorVelAnim == 0));
    }

    void UpdateAiMove()
    {
        RaycastHit2D hit;
        float factorVel = -1;

        if (isStatic)
        {
            SwitchToState(State.isStatic);
        }

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        hit = GroundDetection();

        if (!hit)
        {
            factorVelAnim = 1;
            SwitchToState(State.waiting);
            needToReturn = true;
        }
        else if (controller.collisions.right || controller.collisions.left)
        {
            factorVelAnim = 1;
            SwitchToState(State.waiting);
            needToReturn = false;
            blockReturn = true;
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
            stats.velocity.x = stats.speed * factorVel * factorVelAnim;
        }
    }

    void UpdateAgro()
    {
        float distance = Vector2.Distance(eyes.position, targetCenter.position);

        if (distance <= rayAgrooLenght)
        {
            AgroInfo agroInfo = CheckAgro();
            if (!agroInfo.hit)
            {
                if (distance <= statsAttack[0].attackRange && statsAttack[0].currentCooldown <= 0)
                {
                    stats.movingRight = (agroInfo.directionRay.x > 0);
                    UpdateScale();
                    statsAttack[0].currentCooldown = statsAttack[0].cooldown;
                    animator.Play("AttackCac");
                    stats.actualAttack = 0;
                    SwitchToState(State.attackCac);
                    return;
                }
                if ((agroInfo.directionRay.x > 0) == stats.movingRight)
                {
                    if (distance >= statsAttack[0].attackRange && distance <= statsAttack[1].attackRange && statsAttack[1].currentCooldown <= 0)
                    {
                        statsAttack[1].currentCooldown = statsAttack[1].cooldown;
                        animator.Play("AttackDist");
                        stats.actualAttack = 1;
                        SwitchToState(State.attackDist);
                    }
                    else if (state != State.combat && state != State.blocked && !isStatic)
                    {
                        SwitchToState(State.combat);
                    }
                }
            }
        }
        else if (distance > rayAgrooLenght + 3f)
        {
            if (state == State.blocked || state == State.combat)
            {
                SwitchToState(State.walking);
            }
        }
    }

    void UpdateWaiting()
    {
        factorVelAnim = 0;
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
            else if (!blockReturn)
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
            else
            {
                blockReturn = false;
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

    void UpdateStatic()
    {
        if (!isStatic)
        {
            SwitchToState(State.walking);
        }

    }

    void UpdateFall()
    {
        if (controller.collisions.below)
        {
            if (isStatic)
            {
                SwitchToState(State.isStatic);
            }
            else
            {
                SwitchToState(State.walking);
            }
        }
    }

    void UpdateCombat()
    {
        RaycastHit2D hit;
        float distance = Vector2.Distance(eyes.position, targetCenter.position);

        //follow player 
        if (!escape)
        {
            stats.movingRight = (targetFocus.transform.position.x > transform.position.x);
            UpdateScale();
        }
        else
        {
            stats.movingRight = !(targetFocus.transform.position.x > transform.position.x);
            UpdateScale();
        }

        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        hit = GroundDetection();

        if (!hit)
        {
            stats.velocity.x = 0;
            if (escape)
            {
                SwitchToState(State.blocked);
                stats.movingRight = !stats.movingRight;
                needToReturn = false;
            }
            else if (distance < statsAttack[0].attackRange + 2f)
            {
                escape = true;

                hit = GroundDetection(groundBackDetection);
                if (hit)
                {
                    stats.movingRight = !stats.movingRight;
                    factorVel *= -1;
                    UpdateScale();
                    stats.velocity.x = stats.speed * factorVel * factorVelAnim;
                }
            }
        }
        else if (controller.collisions.right || controller.collisions.left)
        {
            SwitchToState(State.blocked);
            stats.movingRight = !stats.movingRight;
            UpdateScale();
        }
        else
        {
            if (escape)
            {
                stats.velocity.x = stats.speed * factorVel * factorVelAnim;
                if (distance > statsAttack[1].attackRange - 1.5f)
                {
                    escape = false;
                    stats.movingRight = !stats.movingRight;
                    factorVel *= -1;
                    stats.velocity.x = 0;
                }
            }
            else if (distance < statsAttack[0].attackRange + 2f)
            {
                escape = true;

                hit = GroundDetection(groundBackDetection);
                if (hit)
                {
                    stats.movingRight = !stats.movingRight;
                    factorVel *= -1;
                    UpdateScale();
                    stats.velocity.x = stats.speed * factorVel * factorVelAnim;
                }
            }
            else
            {
                if (distance >= statsAttack[1].attackRange)
                {
                    stats.velocity.x = stats.speed * factorVel * factorVelAnim;
                }
                else
                {
                    stats.velocity.x = 0;
                }
            }
        }

        if (distance >= rayAgrooLenght + 2f)
        {
            SwitchToState(State.walking);
            escape = false;
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
        if (statsAttack[0].moveEnable)
        {
            float factorVel = -1;
            if (stats.movingRight)
            {
                factorVel = 1;
            }
            RaycastHit2D hit;

            hit = GroundDetection(statsAttack[0].moveSpeed.x, true);
            if (hit)
            {
                stats.velocity.x = statsAttack[0].moveSpeed.x * factorVel * factorVelAnim;
            }
            else
            {
                stats.velocity.x = 0F;
            }
        }
        else
        {
            stats.velocity.x = 0F;
        }
        CheckHit();
    }

    void UpdateBlocked()
    {
        stats.velocity.x = 0F;

        if (targetFocus.transform.position.x > transform.position.x)
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

        UpdateAgro();

    }

    void StateMachine()
    {
        switch (state)
        {
            case State.walking:
                UpdateBeforeWaiting();
                UpdateAiMove();
                UpdateAgro();
                break;
            case State.waiting:
                UpdateWaiting();
                UpdateAgro();
                break;
            case State.isStatic:
                UpdateStatic();
                UpdateAgro();
                break;
            case State.seeFocus:
                UpdateAgro();
                break;
            case State.fall:
                UpdateFall();
                break;
            case State.blocked:
                UpdateBlocked();
                break;
            case State.attackCac:
                UpdateAttackCac();
                break;
            case State.combat:
                UpdateCombat();
                UpdateAgro();
                break;
            default:
                break;
        }

        UpdateCoolDown();

        base.Update();
    }

    void SetFactorVel(float factor)
    {
        factorVelAnim = factor;
    }

    private void SwitchToState(State newState)
    {
        stats.velocity.x = 0F;
        factorVelAnim = 1;
        prevState = state;
        state = newState;
    }

    private void EndAttack()
    {
        stats.velocity.x = 0F;
        factorVelAnim = 1;
        
        SwitchToState(prevState);
    }

    private void EnableMoveAttack()
    {
        statsAttack[stats.actualAttack].moveEnable = true;
    }

    private void DisableMoveAttack()
    {
        statsAttack[stats.actualAttack].moveEnable = false;
    }

    private void ShootNut()
    {
        Vector2 direction = targetCenter.position - eyes.position;
        direction = direction.normalized;

        GameObject newNut = (GameObject)Instantiate(nutPrefab, firePos.position, Quaternion.identity) as GameObject;
        newNut.GetComponent<Projectile>().Init(direction, statsAttack[stats.actualAttack].damage);
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
                        checkHit = false;
                        break;
                    }
                }           
            }         
        }
    }

    void CheckHitEnabled()
    {
        checkHit = true;
    }


    public enum State
    {
        isStatic,
        blocked,
        attackDist,
        attackCac,
        combat,
        walking,
        waiting,
        seeFocus,
        fall
    }

}
