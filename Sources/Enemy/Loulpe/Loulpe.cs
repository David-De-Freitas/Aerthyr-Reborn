using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loulpe : Enemy
{
    [Space]
    [Header("Info Loulpe")]
    public float jumpForce;
    public State state = State.fall;
    float timerStun;
    public float timeStun;
    [Space]

    [Header("Attack Info")]
    public float speedAttack;
    private bool checkHit = false;
    private bool haveJump = false;
    private ContactFilter2D hitFilter;
    public StatsAttack statsAttack;
    AgroAnim agroAnim;
    protected override void Start()
    {
        base.Start();
        species = Species.Loulpe;

        onDeath += OnDeathInvoke;

        // a revoir
        if (groundDetection == null)
        {
            groundDetection = transform.GetComponentInChildren<Transform>();
        }

        statsAttack.Update(damageFactor);

        agroAnim = gameObject.GetComponentInChildren<AgroAnim>();
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
            GetComponentInChildren<AgroAnim>().StopAgroAnim();
            base.Update();
        }
    }

    private void OnDeathInvoke()
    {
        GameDifficulty actualDiff = GM.gameData.GetGameDifficulty();

        switch (type)
        {
            case Type.Wild:               
                GM.gameData.GetCurrentGameDifficultyProgress().loulpeBasicDefeatedCount++;
                break;
            case Type.Pirate:
                GM.gameData.GetCurrentGameDifficultyProgress().loulpePirateDefeatedCount++;
                break;
            default:
                break;
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(stats.velocity.x));
        animator.SetBool("OnGround", controller.collisions.below);
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
            state = State.waiting;
            stats.velocity.x = 0;
            needToReturn = true;
        }
        else if (controller.collisions.right || controller.collisions.left)
        {
            state = State.waiting;
            stats.velocity.x = 0;
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

    bool UpdateAgro()
    {
        float distance = Vector2.Distance(eyes.position, targetFocus.transform.position);
        if (distance > statsAttack.attackRange)
        {
            return false;
        }
        AgroInfo agroInfo = CheckAgro();

        if (!agroInfo.hit)
        {
            if ((agroInfo.directionRay.x > 0) == stats.movingRight && statsAttack.currentCooldown <= 0F)
            {
                statsAttack.currentCooldown = statsAttack.cooldown;
                state = State.seeFocus;
                stats.velocity.x = 0F;
                return true;
            }
        }
        return false;
    }

    void UpdateWaiting()
    {
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

            state = State.walking;
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
                if (Random.Range(float.MinValue, float.MaxValue) > (75 - 50) / 50 * float.MaxValue)
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
                state = State.waiting;
            }
        }
    }

    void UpdateFall()
    {
        if (controller.collisions.below)
        {
            state = State.walking;
        }
    }

    void UpdateAttack()
    {
        RaycastHit2D hit;

        float factorVel = -1;
        if (stats.movingRight)
        {
            factorVel = 1;
        }

        if (controller.collisions.right || controller.collisions.left)
        {
            state = State.stun;
            animator.Play("Stun");
            stats.velocity = Vector2.zero;
            timerStun = timeStun;
            return;
        }

        hit = GroundDetection();
        if (!hit)
        {
            Jump();
            haveJump = true;
        }
        else if (haveJump)
        {
            AgroInfo agroInfo = CheckAgro();
            if (agroInfo.hit || agroInfo.rayLenght > statsAttack.attackRange || (agroInfo.directionRay.x > 0) != stats.movingRight)
            {
                state = State.walking;
                animator.Play("Walk");
                haveJump = false;
            }
        }

        stats.velocity.x = speedAttack * factorVel;
    }

    void UpdateSeeFocus()
    {
        agroAnim.PlayAgroAnim();
    }

    void UpdateStun()
    {
        timerStun -= Time.deltaTime;
        if (timerStun <= 0F)
        {
            animator.Play("Idle");
            if (!UpdateAgro())
            {
                state = State.walking;
            }
        }
    }

    public void LaunchAttack()
    {
        if (stats.lifeState == StatsEnemy.LifeState.alive)
        {
            animator.Play("Attack");
            state = State.attack;
        }
    }

    void Jump()
    {
        if (controller.collisions.below)
        {
            stats.velocity.y = jumpForce;
        }
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
            case State.seeFocus:
                UpdateSeeFocus();
                break;
            case State.attack:
                UpdateAttack();
                CheckHit();
                break;
            case State.fall:
                UpdateFall();
                break;
            case State.stun:
                UpdateStun();
                break;
            default:
                break;
        }
        statsAttack.currentCooldown -= Time.deltaTime;

        base.Update();
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
                        Vector2 force = statsAttack.knockBackForce;
                        force.x *= factorMove;
                        bool hit = playerHit.TakeDamage(statsAttack.damage, force);
                        if (hit)
                        {

                            animator.Play("Stun");
                            state = State.stun;
                            timerStun = timeStun;
                            stats.velocity = Vector2.zero;
                            checkHit = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    void CheckHitEnabled()
    {
        checkHit = true;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        stats.velocity.x = 0;
        state = State.takeDamage;
        agroAnim.StopAgroAnim(false);
    }

    void EndTakeDamage()
    {
        state = State.seeFocus;
        animator.Play("Idle");
    }

    public enum State
    {
        walking,
        waiting,
        seeFocus,
        attack,
        stun,
        fall,
        takeDamage
    }

}
