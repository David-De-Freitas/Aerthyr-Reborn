using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crechon : Boss
{
    ParticleAttackBoss particlesController;
    [Space]
    [Header("Attack")]
    [SerializeField]
    BossAttack[] attacks;
    [SerializeField] ContactFilter2D hitFilter;
    [SerializeField] BoxCollider2D attackBox;
    [SerializeField] LoulpeArmy loulpeArmy;

    private float distanceTarget;
    private float timerWaiting;
    private float timeToWait;

    [SerializeField] bool checkHit;
    [SerializeField] bool haveHitPlayer;
    [Space]
    [Header("Boss Phase")]
    [SerializeField]
    int currentPhase;
    [Range(0, 100)]
    [SerializeField]
    List<int> phaseBoss = new List<int>();

    [Header("Boss Dialogue")]
    public Canvas canvasDialogue;
    Text dialogueText;
    [SerializeField]
    string[] dialogue;
    int actualText = 0;

    public float timeDialogue;
    float timerDialogue;

    bool dialogueStart = false;


    Vector2 chargeStartPosition;

    float summonAnimTime;
    float summonAnimMaxTime;

    private void Awake()
    {
        currentPhase = 0;
        summonAnimMaxTime = 5f;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        particlesController = transform.GetComponent<ParticleAttackBoss>();

        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].Update();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        StateMachine();
        UpdateAnimation();
    }

    void UpdateWalk()
    {
        distanceTarget = Mathf.Abs(targetFocus.transform.position.x - transform.position.x);
        if (targetFocus.transform.position.x < transform.position.x && lookingRight)
        {
            lookingRight = false;
        }
        else if (targetFocus.transform.position.x > transform.position.x && !lookingRight)
        {
            lookingRight = true;
        }

        int factorMove = (lookingRight) ? 1 : -1;

        if (distanceTarget > 2.5)
        {
            stats.velocity.x = stats.speed * factorMove;
        }
        else
        {
            stats.velocity.x = 0;
        }

        if (timerWaiting < timeToWait)
        {
            timerWaiting += Time.deltaTime;
        }
        else
        {
            CheckAttacks();
        }
    }

    void UpdateIdle()
    {
        distanceTarget = Mathf.Abs(targetFocus.transform.position.x - transform.position.x);
        if (targetFocus.transform.position.x < transform.position.x && lookingRight)
        {
            lookingRight = false;
        }
        else if (targetFocus.transform.position.x > transform.position.x && !lookingRight)
        {
            lookingRight = true;
        }

        if (distanceTarget > attacks[0].maxAttackRange)
        {
            state = State.walk;
        }

        if (timerWaiting < timeToWait)
        {
            timerWaiting += Time.deltaTime;
        }
        else
        {
            state = State.walk;
            CheckAttacks();
        }
    }

    void LaunchAttack(int indexAttack)
    {
        stats.actualAttack = indexAttack;
        state = State.attack;
        timerWaiting = 0F;
        timeToWait = attacks[indexAttack].timeToWait;

        switch (indexAttack)
        {
            case ((int)CrechonAttacks.Base):
                animator.Play("Attack Base");
                break;

            case ((int)CrechonAttacks.Heavy):
                animator.Play("Attack Heavy");
                break;

            case ((int)CrechonAttacks.Charge):
                animator.Play("Attack Charge");
                chargeStartPosition = transform.position;
                break;

            case ((int)CrechonAttacks.LoulpeArmySummon):
                animator.Play("Attack Invoc Start");

                BlockDamageReceived = true;
                summonAnimTime = summonAnimMaxTime;

                float scaleX = -1f;

                if (targetFocus.transform.position.x > transform.position.x)
                {
                    scaleX = 1f;
                }

                loulpeArmy.ActiveLoulpeArmy(scaleX);
                SetVelocityX(0f);
                break;

            default:
                break;
        }
        attacks[indexAttack].currentCooldown = attacks[indexAttack].cooldown;
    }

    void CheckAttacks()
    {
        int healthPercent;

        healthPercent = (int)(stats.health * 100 / stats.maxHealth);

        if (currentPhase < phaseBoss.Count && healthPercent <= phaseBoss[currentPhase])
        {
            currentPhase++;
            LaunchAttack((int)CrechonAttacks.LoulpeArmySummon);
        }
        else
        {
            BossAttack attack;

            // CHarge attack
            attack = attacks[(int)CrechonAttacks.Charge];
            if (attack.currentCooldown <= 0f && distanceTarget < attack.maxAttackRange && distanceTarget > attack.minAttackRange)
            {
                LaunchAttack((int)CrechonAttacks.Charge);
                return;
            }
            // Heavy attack
            attack = attacks[(int)CrechonAttacks.Heavy];
            if (attack.currentCooldown <= 0f && distanceTarget < attack.maxAttackRange && distanceTarget > attack.minAttackRange)
            {
                LaunchAttack((int)CrechonAttacks.Heavy);
                return;
            }
            // Base attack
            attack = attacks[(int)CrechonAttacks.Base];
            if (attack.currentCooldown <= 0f && distanceTarget < attack.maxAttackRange)
            {
                LaunchAttack((int)CrechonAttacks.Base);
                return;
            }
        }
    }

    bool CheckCurrentAttack(BossAttack attack, int attackID)
    {
        if (attack.currentCooldown <= 0f && distanceTarget < attack.maxAttackRange)
        {
            LaunchAttack(attackID);
            return true;
        }
        return false;
    }

    void UpdateAnimation()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(stats.velocity.x));
    }

    void UpdateCooldown()
    {
        if (state != State.waitingPlayer)
        {
            foreach (BossAttack attack in attacks)
            {
                if (attack.currentCooldown > 0F)
                {
                    attack.currentCooldown -= Time.deltaTime;
                }
            }
        }
    }

    void UpdateActualAttack()
    {
        switch (stats.actualAttack)
        {
            case ((int)CrechonAttacks.Base):

                break;
            case ((int)CrechonAttacks.Heavy):

                break;
            case ((int)CrechonAttacks.Charge):
                ChargeAttackUpdate();
                break;
            case ((int)CrechonAttacks.LoulpeArmySummon):
                SummonAttackUpdate();
                break;
            default:
                break;
        }
    }

    void ChargeAttackUpdate()
    {
        float chargeDistance;

        chargeDistance = Vector2.Distance(transform.position, chargeStartPosition);

        if (haveHitPlayer)
        {
            Player player = targetFocus.GetComponent<Player>();

            if (player.GetCollisions().left || player.GetCollisions().right)
            {
                // Stop player tows
                player.StopTows();

                haveHitPlayer = false;
                player.TakeDamage(150, 1f);

                // Stop Boss movement
                animator.Play("Idle");
                state = State.idle;
                stats.velocity.x = 0f;

                particlesController.StopEmit((int)CrechonAttacks.Charge);
            }
            else if (chargeDistance >= 18)
            {
                player.StopTows();

                haveHitPlayer = false;
                //LaunchAttack((int)CrechonAttacks.Base);
                particlesController.StopEmit((int)CrechonAttacks.Charge);
            }
        }
        else
        {
            if (controller.collisions.right || controller.collisions.left)
            {
                animator.Play("Idle");
                state = State.idle;
                stats.velocity.x = 0f;
                TakeDamage(250);

                particlesController.StopEmit((int)CrechonAttacks.Charge);
            }
            else if (chargeDistance >= 18)
            {
                stats.velocity.x = 0f;
                animator.Play("Idle");
                state = State.idle;

                particlesController.StopEmit((int)CrechonAttacks.Charge);
            }
        }
    }

    void SummonAttackUpdate()
    {
        if (summonAnimTime > 0)
        {
            summonAnimTime -= Time.deltaTime;
        }
        else if (summonAnimTime <= summonAnimMaxTime / 2f && BlockDamageReceived)
        {
            BlockDamageReceived = false;
        }
        else if (summonAnimTime <= 0f)
        {
            summonAnimTime = 0f;
            animator.Play("Idle");
            state = State.idle;
        }
    }

    public void ChangeStateTo(State newState)
    {
        state = newState;
    }

    public void SetVelocityX(float velX)
    {
        int factorMove = (lookingRight) ? 1 : -1;

        stats.velocity.x = velX * factorMove;
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

                    if (playerHit != null && !haveHitPlayer)
                    {
                        int factorMove = (lookingRight) ? 1 : -1;
                        Vector2 force = attacks[stats.actualAttack].knockBackForce;
                        force.x *= factorMove;

                        playerHit.SetOppenentAttackDir(force.x);
                        haveHitPlayer = playerHit.TakeDamage(attacks[stats.actualAttack].damage, attacks[stats.actualAttack].stunTime, force);

                        if (stats.actualAttack == (int)CrechonAttacks.Charge && haveHitPlayer)
                        {
                            playerHit.Tows(stats.velocity);
                            checkHit = false;
                        }
                        break;
                    }
                }
            }

            if (stats.actualAttack != (int)CrechonAttacks.Charge)
            {
                checkHit = false;
                haveHitPlayer = false;
            }
        }
    }

    void CheckHitEnabled()
    {
        checkHit = true;
    }

    void StateMachine()
    {
        switch (state)
        {
            case State.waitingPlayer:
                break;
            case State.idle:
                UpdateIdle();
                break;
            case State.walk:
                UpdateWalk();
                break;
            case State.attack:
                UpdateActualAttack();
                CheckHit();
                break;
            case State.defeated:
                UpdateDialogue();
                break;
            case State.dead:
                break;
            default:
                break;
        }
        UpdateCooldown();

        base.Update();
    }

    void UpdateDialogue()
    {
        if (!dialogueStart)
        {
            StartDialogue();
        }

        timerDialogue -= Time.deltaTime;
        if (timerDialogue <= 0)
        {
            actualText++;
            timerDialogue = timeDialogue;
            if (actualText >= dialogue.Length)
            {
                StopDialogue();
                return;
            }
            dialogueText.text = dialogue[actualText];
        }
    }

    void StopDialogue()
    {
        dialogueStart = false;
        canvasDialogue.gameObject.SetActive(false);
        dialogueText.text = dialogue[0];
        StartEscape();
    }

    void StartDialogue()
    {
        if (dialogueText == null)
        {
            dialogueText = canvasDialogue.transform.GetChild(0).GetComponentInChildren<Text>();
        }

        Vector3 scale = Vector3.one;
        if (lookingRight)
        {
            scale.x = -1;
        }

        dialogueText.transform.localScale = scale;

        dialogueStart = true;
        canvasDialogue.gameObject.SetActive(true);
        dialogueText.text = dialogue[actualText];
        timerDialogue = timeDialogue;
    }


    void StartEscape()
    {

        state = State.escape;
        animator.Play("Escape");
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        sprite.sortingLayerName = "Front";
        sprite.sortingOrder = 80;

        Vector3 scale = Vector3.one;
        if (transform.localScale.x > 0f )
        {
            lookingRight = false;
            scale.x = -1;
        }
        else
        {
            lookingRight = true;
        }
        transform.localScale = scale;

        stats.velocity.y = 8f;
        stats.velocity.x = 6f * scale.x;

        eventBoss.EndEvent();
    }

    public enum CrechonAttacks
    {
        Charge,
        Heavy,
        Base,
        LoulpeArmySummon
    }
}
