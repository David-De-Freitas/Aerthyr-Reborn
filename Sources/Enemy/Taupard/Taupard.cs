using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taupard : Enemy
{
    public State state;
    public Collider2D detection;
    public bool checkHit;
    StatsAttack statsAttack;
    private ContactFilter2D hitFilter;

    protected override void Start()
    {
        species = Species.Taupard;

        if (targetFocus == null)
        {
            targetFocus = GameManager.Singleton.Player.GetComponent<Player>();
        }
        base.Start();
    }

    protected override void Update()
    {
        StateMachine();
        base.Update();
    }

    public void Init(bool goRight, Transform cerfTransform, StatsAttack newStatsAttack)
    {
        stats.movingRight = goRight;
        transform.position = cerfTransform.position;
        statsAttack = newStatsAttack;
    }

    void UpdateRush()
    {

        RaycastHit2D hit;
        float factorVel = -1;

        if (stats.movingRight)
        {
            factorVel = 1;
        }

        hit = GroundDetection();
        if (controller.collisions.right || controller.collisions.left)
        {
            animator.Play("Look");
            state = State.look;
            stats.velocity.x = 0;
            return;
        }
        if (!hit)
        {
            animator.Play("Look");
            state = State.look;
            stats.velocity.x = 0;
        }
        else
        {
            Collider2D[] collider2Ds = new Collider2D[200];

            Physics2D.OverlapCollider(detection, hitFilter, collider2Ds);
            foreach (Collider2D collider in collider2Ds)
            {

                if (collider != null)
                {
                    Player playerHit = collider.GetComponent<Player>();
                    if (playerHit != null)
                    {
                        animator.Play("Attack");
                        SwitchToState(State.attack);
                        stats.velocity.x = 0;
                        stats.movingRight = !stats.movingRight;

                        return;
                    }
                }
            }
            stats.velocity.x = stats.speed * factorVel;
        }
    }

    void UpdateAttack()
    {
        CheckHit();
    }


    void CheckHit()
    {
        if (checkHit)
        {
           // Debug.Log("CheckHit");
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
                        playerHit.TakeDamage(statsAttack.damage, force);
                        checkHit = false;
                        return;
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


    private void SwitchToState(State newState)
    {
        state = newState;
    }

    void StateMachine()
    {
        switch (state)
        {
            case State.rush:
                UpdateRush();
                break;
            case State.attack:
                UpdateAttack();
                break;
            case State.look:
                break;
            case State.toDel:
                AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
                if (currentState.IsName("Rush"))
                {
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
    }


    public enum State
    {
        rush,
        attack,
        look,
        toDel
    }

}
