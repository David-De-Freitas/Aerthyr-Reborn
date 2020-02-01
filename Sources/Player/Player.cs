using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("CHILDRENS")]

    public Transform centerTransform;

    [Header("STATS")]

    public PlayerStats stats;

    public Animator AnimatorRef { get { return animator; } }

    #region Private Stats

    GameManager gameManager;
    InputControler inputs;
    Controller2D controller;
    PlayerFight playerFight;
    Inventory inventory;

    CameraController cameraController;
    Camera2D camera2D;
    Animator animator;
    Vector2 directionalInput;

    Vector3 lastGroundedPosition;
    // Alterations

    bool isStun;
    bool isKnockBack;
    float stunTimer;
    float knockBackTimer;

    #endregion

    #region Natives Functions
    private void Awake()
    {
        inventory = GetComponentInChildren<Inventory>();
        GameManager.Singleton.SetPlayer(this, inventory);
        stats.Init();
        StartCoroutine(stats.LateInit());
    }

    // Use this for initialization
    void Start()
    {
        //Components refs
        gameManager = GameManager.Singleton;
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        playerFight = GetComponent<PlayerFight>();

        inputs = gameManager.inputControler;
        cameraController = Camera.main.GetComponent<CameraController>();
        camera2D = cameraController.GetComponent<Camera2D>();

        StartCoroutine(UpdateLastGroundedPos());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();

        playerFight.UpdateFight();

        StateUpdate();

        CheckWorldPosition();

        UpdateAnimations();

        stats.UpdateRegenStats();
    }

    #endregion

    public void Init(Transform refTransform)
    {
        transform.position = refTransform.position;
        cameraController = Camera.main.GetComponent<CameraController>();
        camera2D = cameraController.GetComponent<Camera2D>();

        FlipX(1);

        isKnockBack = false;
        isStun = false;
        stunTimer = 0f;

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            stats.Restore();
        }
    }

    #region Inputs Functions

    /// <summary>
    /// To call when directionals inputs are used.
    /// </summary>
    /// <param name="horizontalInput"></param>
    public void OnHorizontalInput(int horizontalInput)
    {
        if (stats.state != PlayerStats.State.Dodging && stats.state != PlayerStats.State.Towed)
        {
            if (!playerFight.comboActive)
            {
                float moveSpeed = stats.moveSpeed;

                if (stats.runningJump)
                {
                    moveSpeed *= stats.runningJumpSpeedMultiplier;
                }

                stats.velocity.x = horizontalInput * moveSpeed;
                cameraController.SetCameraState(CameraController.CameraState.PlayerFollow);
                FlipX(horizontalInput);
            }

        }
    }

    public void OnHorizontalInputUp()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && stats.state != PlayerStats.State.Towed)
        {
            stats.velocity.x = 0;
        }
    }

    /// <summary>
    /// To call when directionals inputs are used.
    /// </summary>
    /// <param name="verticalInput"></param>
    public void OnVerticalInput(float verticalInput)
    {
        if (stats.state == PlayerStats.State.Idle)
        {
            if (verticalInput < 0f)
            {
                cameraController.SetCameraState(CameraController.CameraState.LookDown);
            }
        }
    }

    public void OnVerticalInputUp()
    {
        cameraController.SetCameraState(CameraController.CameraState.PlayerFollow);
    }

    /// <summary>
    /// To call when the jump input is pressed
    /// </summary>
    public void OnJumpInputDown()
    {
        if (stats.state != PlayerStats.State.Towed)
        {
            if (controller.collisions.below || stats.jumpsAvailable > 0)
            {
                bool isRunning = stats.state == PlayerStats.State.Running;

                CancelDodge(false);
                playerFight.CancelCombo();
                animator.Play("Jump_Impulse", 0);

                stats.SetJumpVelocity(controller.Gravity, isRunning);
                stats.velocity.y = stats.jumpVelocity;
                stats.jumpsAvailable--;
                stats.jumpsUsed++;

                // Reset the jump modifier
                if (stats.runningJump && stats.jumpsUsed > 1)
                {
                    stats.runningJump = false;
                }
            }
        }
    }

    /// <summary>
    /// To call when the dodge input is pressed
    /// </summary>
    public void OnDodgeInputDown()
    {
        if (stats.stamina >= stats.dodgeStaminaCost && stats.state != PlayerStats.State.Towed)
        {
            int horizontalInput = Mathf.FloorToInt(Input.GetAxis("Horizontal"));
            FlipX(horizontalInput);
            // Set animation
            animator.Play("Dodge");
            cameraController.SetCameraState(CameraController.CameraState.PlayerFollow);

            // Set stats
            stats.isInvincible = true;
            stats.StaminaReduction(stats.dodgeStaminaCost);
            stats.staminaRegenRatio = 0f;

            // Cancel the attacks
            playerFight.CancelCombo();
        }
    }

    public void OnMeleeAttackInputDown()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && stats.state != PlayerStats.State.Towed)
        {
            int horizontalInput = Mathf.FloorToInt(Input.GetAxis("Horizontal"));

            playerFight.UseCombo();
            FlipX(horizontalInput);

            cameraController.SetCameraState(CameraController.CameraState.PlayerFollow);
        }
    }

    public void OnSkillInputDown(int skillIndex)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge") && stats.state != PlayerStats.State.Towed)
        {
            int otherSkillIndex = skillIndex == 1 ? 0 : 1;

            if (playerFight.skills[skillIndex] != null)
            {
                if (playerFight.skills[otherSkillIndex] != null)
                {
                    if (playerFight.skills[otherSkillIndex].IsActive)
                    {
                        if (playerFight.skills[skillIndex].AllowMultiUse)
                        {
                            playerFight.skills[skillIndex].Use(this);
                        }
                    }
                    else
                    {
                        playerFight.skills[skillIndex].Use(this);
                    }
                }
                else
                {
                    playerFight.skills[skillIndex].Use(this);
                }
            }
        }
    }

    #endregion

    #region States Functions

    void CalculateVelocity()
    {
        if (!playerFight.IsComboIgnoringGravity())
        {
            stats.velocity.y += controller.Gravity * Time.deltaTime;
        }

        playerFight.FightVelocityUpdate();

        controller.Move(stats.velocity * Time.deltaTime);

        if (controller.collisions.left || controller.collisions.right)
        {
            stats.velocity.x = 0;
        }
    }

    void StateUpdate()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (playerFight.comboActive)
        {
            stats.staminaRegenRatio = 0.5f;
            stats.state = PlayerStats.State.Attacking;
        }
        else if (currentState.IsName("Idle"))
        {
            stats.staminaRegenRatio = 1f;
            stats.state = PlayerStats.State.Idle;
        }
        else if (currentState.IsName("Run"))
        {
            stats.staminaRegenRatio = 0.7f;
            stats.state = PlayerStats.State.Running;
        }
        else if (currentState.IsName("Jump_Impulse"))
        {
            stats.staminaRegenRatio = 0f;
            stats.state = PlayerStats.State.Jumping_Impulse;
        }
        else if (currentState.IsName("Jump_Ascend") || currentState.IsName("Jump_Fall"))
        {
            stats.staminaRegenRatio = 0f;
            stats.state = PlayerStats.State.Jumping_InAir;
        }
        else if (currentState.IsName("Jump_Landing"))
        {
            stats.state = PlayerStats.State.Jumping_Landing;
        }
        else if (currentState.IsName("Dodge"))
        {
            stats.state = PlayerStats.State.Dodging;
        }
        else if (currentState.IsName("Player_Knockback"))
        {
            stats.staminaRegenRatio = 0f;
            stats.state = PlayerStats.State.KnockBack;
        }
        else if (currentState.IsName("Player_Stun"))
        {
            stats.staminaRegenRatio = 0.5f;
            stats.state = PlayerStats.State.Stun;
        }
        else if (currentState.IsName("Player_Towed"))
        {
            stats.staminaRegenRatio = 0.5f;
            stats.state = PlayerStats.State.Towed;
        }

        JumpUpdate();
        DodgeUpdate();
        KnockBackUpdate();
        StunUpdate();
        TowedUpdate();
    }

    void JumpUpdate()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            stats.velocity.y = 0f;
            stats.runningJump = false;

            if (controller.collisions.below)
            {
                stats.jumpsAvailable = stats.maxJumpNumber;
                stats.jumpsUsed = 0;
            }
        }
        else if (!controller.collisions.below)
        {
            if (stats.jumpsAvailable == stats.maxJumpNumber)
            {
                stats.jumpsAvailable--;
            }
        }
    }

    void DodgeUpdate()
    {
        if (stats.state == PlayerStats.State.Dodging)
        {
            if (stats.currentDodgeTime < stats.dodgeTime)
            {
                stats.currentDodgeTime += Time.deltaTime;
                stats.velocity.x = stats.dodgeImpulseForce * (stats.isFacingRight ? 1 : -1);

                if (controller.collisions.right || controller.collisions.left)
                {
                    CancelDodge(true);
                }
            }
            else
            {
                CancelDodge(true);
            }
        }
    }

    void KnockBackUpdate()
    {
        if (stats.state == PlayerStats.State.KnockBack)
        {
            knockBackTimer -= Time.deltaTime;
            if (controller.collisions.below || knockBackTimer <= 0f)
            {
                if (!isStun)
                {
                    inputs.ControlBlocked = false;
                    animator.Play("Idle");
                }
                else
                {
                    animator.Play("Player_Stun");
                }

                isKnockBack = false;
                stats.velocity = Vector2.zero;
            }
        }
    }

    void StunUpdate()
    {
        if (stats.state == PlayerStats.State.Stun)
        {
            if (stunTimer > 0f)
            {
                stunTimer -= Time.deltaTime;
            }
            else
            {
                stunTimer = 0f;
                animator.Play("Idle");
                inputs.ControlBlocked = false;
                isStun = false;
            }
        }
    }

    void TowedUpdate()
    {
        if (stats.state == PlayerStats.State.Towed)
        {
            stats.velocity = stats.tempVelocity;
        }
    }

    void CheckWorldPosition()
    {
        bool isUnderWorldBounds;
        isUnderWorldBounds = transform.position.y < camera2D.cameraLimits.y;

        if (isUnderWorldBounds && stats.health > 0f)
        {
            float damageAmount;
            float healthPerc;
                  
            if (stats.fallCount < stats.maxFallCount)
            {
                healthPerc = 25f;
                damageAmount = ((stats.healthMax * healthPerc) / 100F);
                stats.health -= damageAmount;

                if (stats.health > 0f)
                {
                    transform.position = lastGroundedPosition;
                    stats.fallCount++;
                }
            }
            else
            {
                healthPerc = 25f;
                damageAmount = ((stats.healthMax * healthPerc) / 100F) * Time.deltaTime;
                stats.health -= damageAmount;
            }

           
            if (stats.health <= 0f)
            {
                stats.state = PlayerStats.State.Dying;
                stats.health = 0f;
                Dead();
            }

        }
    }

    /// <summary>
    /// Cancel the Dodge and change automaticaly the current state if it have to
    /// </summary>
    /// <param name="haveToSwitchState"></param>
    void CancelDodge(bool haveToSwitchState)
    {
        stats.isInvincible = false;
        stats.currentDodgeTime = 0f;
        stats.velocity.x = 0;

        if (haveToSwitchState)
        {
            if (animator.GetBool("OnGround"))
            {
                if (stats.velocity.x <= 1f)
                {
                    animator.Play("Idle", 0);
                }
                else
                {
                    animator.Play("Run", 0);
                }
            }
            else
            {
                if (stats.velocity.y < 0)
                {
                    animator.Play("Jump_Fall", 0);
                }
                else
                {
                    animator.Play("Jump_Ascend", 0);
                }
            }
        }
    }

    void Dead()
    {
        inventory.DestroyNonSavedItems();
        HudManager.Singleton.equipmentUI.DestroyNonSavedItems();

        gameManager.InstanceManager.EndExpedition();
    }

    void UpdateAnimations()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(stats.velocity.x));
        animator.SetFloat("VelocityY", stats.velocity.y);
        animator.SetBool("OnGround", controller.collisions.below);
    }
    #endregion

    #region Utils
    void FlipX(float dirX)
    {
        if (dirX < 0 && stats.isFacingRight || dirX > 0 && !stats.isFacingRight)
        {
            stats.isFacingRight = !stats.isFacingRight;
            gameObject.transform.localScale = new Vector3(Mathf.Sign(dirX), 1);
        }
    }

    /// <summary>
    /// Get the collisions Infos of the player.
    /// </summary>
    /// <returns></returns>
    public Controller2D.CollisionInfo GetCollisions()
    {
        return controller.collisions;
    }

    private IEnumerator UpdateLastGroundedPos()
    {
        while(true)
        {
            if (controller.collisions.below)
            {
                lastGroundedPosition = transform.position;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    #endregion

    #region Alterations Functions

    float ArmorDamageReduction(float rawDamage)
    {
        float netDamage; // The damage dealt after reduction
        float drFactor; // The damage reduction factor
        float armor; // The armor value

        armor = stats.armor;

        // Calculate the damage reduction factor

        drFactor = armor / (armor + 10 * rawDamage);

        // Calculate the damage after reduction

        netDamage = rawDamage - rawDamage * drFactor;

        return netDamage;
    }

    float ArmorStunReduction(float _rawStunTime)
    {
        float netStunTime;
        float rFactor;
        float maxArmor;
        float armor;

        maxArmor = 1000f;
        armor = stats.armor;

        rFactor = maxArmor / (maxArmor + armor);

        netStunTime = _rawStunTime * rFactor;

        return netStunTime;
    }

    /// <summary>
    /// Deal damage to the player.
    /// </summary>
    /// <param name="_rawDamage">The raw damage to deal</param>
    public bool TakeDamage(float _rawDamage)
    {
        bool hasTakeDamages = false;

        if (!stats.isInvincible && stats.health > 0.1f)
        {
            float damageDealt;

            damageDealt = ArmorDamageReduction(_rawDamage);

            if (stats.damageAbsorptionActive)
            {
                if (damageDealt > stats.damageAbsorptionAmount)
                {
                    stats.health -= damageDealt - stats.damageAbsorptionAmount;
                    stats.damageAbsorptionAmount = 0;
                }
                else
                {
                    stats.damageAbsorptionAmount -= damageDealt;
                }
            }
            else
            {
                stats.health -= damageDealt;
            }

            hasTakeDamages = true;

            if (stats.health <= 0f)
            {
                stats.state = PlayerStats.State.Dying;
                stats.health = 0f;
                Dead();
            }
        }

        return hasTakeDamages;
    }

    /// <summary>
    /// Deal damage to the player and knockback him.
    /// </summary>
    /// <param name="_rawDamage"></param>
    /// <param name="knockBackForce"></param>
    public bool TakeDamage(float _rawDamage, Vector2 knockBackForce)
    {
        bool hasTakeDamages = false;

        if (!stats.isInvincible && stats.health > 0f)
        {
            float damageDealt;

            damageDealt = ArmorDamageReduction(_rawDamage);

            if (stats.damageAbsorptionActive)
            {
                if (damageDealt > stats.damageAbsorptionAmount)
                {
                    stats.health -= damageDealt - stats.damageAbsorptionAmount;
                    stats.damageAbsorptionAmount = 0;
                }
                else
                {
                    stats.damageAbsorptionAmount -= damageDealt;
                }
            }
            else
            {
                stats.health -= damageDealt;
            }

            hasTakeDamages = true;

            if (stats.health <= 0f)
            {
                stats.state = PlayerStats.State.Dying;
                stats.health = 0f;
                Dead();
            }
            else
            {
                KnockBack(knockBackForce);
            }
        }
        return hasTakeDamages;
    }

    /// <summary>
    /// Deal damage to the player and stun him according to the stun time.
    /// </summary>
    /// <param name="_rawDamage"></param>
    /// <param name="_stunTime"></param>
    public bool TakeDamage(float _rawDamage, float _stunTime)
    {
        bool hasTakeDamages = false;

        if (!stats.isInvincible && stats.health > 0f)
        {
            float damageDealt;

            damageDealt = ArmorDamageReduction(_rawDamage);

            if (stats.damageAbsorptionActive)
            {
                if (damageDealt > stats.damageAbsorptionAmount)
                {
                    stats.health -= damageDealt - stats.damageAbsorptionAmount;
                    stats.damageAbsorptionAmount = 0;
                }
                else
                {
                    stats.damageAbsorptionAmount -= damageDealt;
                }
            }
            else
            {
                stats.health -= damageDealt;
            }

            hasTakeDamages = true;

            if (stats.health <= 0f)
            {
                stats.state = PlayerStats.State.Dying;
                stats.health = 0f;
                Dead();
            }
            else
            {
                Stun(_stunTime);
            }
        }
        return hasTakeDamages;
    }

    /// <summary>
    /// Deal damage to the player, stun and knockback him.
    /// </summary>
    /// <param name="_rawDamage"></param>
    /// <param name="_stunTime"></param>
    /// <param name="knockBackForce"></param>
    public bool TakeDamage(float _rawDamage, float _stunTime, Vector2 knockBackForce)
    {
        bool hasTakeDamages = false;

        if (!stats.isInvincible && stats.health > 0f)
        {
            float damageDealt;

            damageDealt = ArmorDamageReduction(_rawDamage);

            if (stats.damageAbsorptionActive)
            {
                if (damageDealt > stats.damageAbsorptionAmount)
                {
                    stats.health -= damageDealt - stats.damageAbsorptionAmount;
                    stats.damageAbsorptionAmount = 0;
                }
                else
                {
                    stats.damageAbsorptionAmount -= damageDealt;
                }
            }
            else
            {
                stats.health -= damageDealt;
            }

            hasTakeDamages = true;

            if (stats.health <= 0f)
            {
                stats.state = PlayerStats.State.Dying;
                stats.health = 0f;
                Dead();
            }
            else
            {
                Stun(_stunTime, true);
                KnockBack(knockBackForce);
            }
        }

        return hasTakeDamages;
    }

    public void Stun(float stunTime)
    {
        if (!stats.isInvincible)
        {
            if (!isStun && stunTime != 0f)
            {
                isStun = true;
                stunTimer = stunTime;
                inputs.ControlBlocked = true;
                stats.velocity = Vector2.zero;
                animator.Play("Player_Stun");
                playerFight.CancelCombo();
            }
        }
    }

    public void Stun(float stunTime, bool playAnim)
    {
        if (!stats.isInvincible)
        {
            if (!isStun && stunTime != 0f)
            {
                isStun = true;
                stunTimer = stunTime;
                inputs.ControlBlocked = true;
                stats.velocity = Vector2.zero;
                if (playAnim)
                {
                    animator.Play("Player_Stun");
                    playerFight.CancelCombo();
                }
            }
        }
    }

    public void KnockBack(Vector2 knockBackForce)
    {
        if (!stats.isInvincible)
        {
            if (!isKnockBack && knockBackForce != Vector2.zero)
            {
                isKnockBack = true;
                inputs.ControlBlocked = true;
                stats.velocity = knockBackForce;
                knockBackTimer = 0.5f;

                animator.Play("Player_Knockback");
                playerFight.CancelCombo();
            }
        }
    }

    /// <summary>
    /// Tows the player with the velocity pass in parameter.
    /// Disable inputs.
    /// </summary>
    /// <param name="velocity"></param>
    public void Tows(Vector2 velocity)
    {
        if (!stats.isInvincible)
        {
            stats.tempVelocity = velocity;
            inputs.ControlBlocked = true;
            playerFight.CancelCombo();

            animator.Play("Player_Towed");
            stats.state = PlayerStats.State.Towed;
        }
    }

    /// <summary>
    /// Release the player and set his state to Idle.
    /// </summary>
    public void StopTows()
    {
        print("STOP TOWS");
        inputs.ControlBlocked = false;
        stats.velocity = Vector2.zero;
        animator.Play("Idle");
        stats.state = PlayerStats.State.Idle;
    }

    /// <summary>
    /// Enable or Disable the control of the player with inputs.
    /// </summary>
    /// <param name="state"></param>
    public void SetControlBlocked(bool state)
    {
        inputs.ControlBlocked = state;

        if (state)
        {
            playerFight.CancelCombo();

            if (controller.collisions.below)
            {
                stats.velocity = Vector2.zero;
            }
            else
            {
                stats.velocity.x = 0f;
            }

            animator.Play("Idle");
        }
    }

    /// <summary>
    /// Flip the player according to the position of the oppenent.
    /// <para>
    /// Flip only if the player is not already altered.
    /// </para>
    /// </summary>
    /// <param name="position"></param>
    public void SetOppenentPosition(Vector2 position)
    {
        if (!inputs.ControlBlocked)
        {
            bool isAtRight;
            float dir;

            isAtRight = position.x > transform.position.x;

            dir = isAtRight ? 1 : -1;

            FlipX(dir);
        }
    }

    /// <summary>
    /// Flip the player according to the direction of the attack of the oppenent.
    /// <para>
    /// Flip only if the player is not already altered.
    /// </para>
    /// </summary>
    /// <param name="attackDirX"></param>
    public void SetOppenentAttackDir(float attackDirX)
    {
        if (!inputs.ControlBlocked)
        {
            FlipX(attackDirX * -1f);
        }
    }
    #endregion
}

// ---------------------** NEW CLASS **---------------------

[System.Serializable]
public class PlayerStats
{
    [Header("STATES")]
    public State state;
    public bool isInvincible;
    public bool isFacingRight = true;
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("VIABILITY STATS")]

    public float baseHealthMax;
    public float healthMax { get; set; }
    public float health { get; set; }
    [Space]
    public float baseHealthRegen;
    public float healthRegen { get; set; }
    [Space]
    public float baseArmor;
    public float armor { get; set; }
    [Space]
    public float baseStaminaMax;
    public float staminaMax { get; set; }
    public float stamina;
    [Space]
    public float baseStaminaRegen;
    public float staminaRegen { get; set; }
    public float staminaRegenRatio;
    [Space]
    public int maxFallCount;
    public int fallCount;
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("OFFENSIVES STATS")]
    public float baseNormalDamages;
    public float normalDamages { get; set; }
    [Space]
    public float baseCriticalChances;
    public float criticalChances { get; set; }
    public float baseCriticalDamagesM;
    public float criticalDamagesM { get; set; }
    [Space]
    public float baseCapacitiesDamagesM;
    public float capacitiesDamagesM { get; set; }
    public float baseCooldownReduction;
    public float cooldownReduction { get; set; }
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("MOVEMENTS STATS")]
    public float moveSpeed;
    [Space]
    public float jumpHeight;
    public float firstJumpHeight;
    [Space]
    public float runningJumpHeightMultiplier;
    public float runningJumpSpeedMultiplier;
    [Space]
    public int maxJumpNumber;
    public int jumpsAvailable;
    public int jumpsUsed;
    [Space]
    public float timeToJumpApex;
    public float jumpVelocity;
    [Space]
    public Vector2 velocity;
    public Vector2 tempVelocity { get; set; }
    public bool runningJump;
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("CAPACITIES STATS")]
    public float dodgeImpulseForce;
    public float dodgeTime;
    public float currentDodgeTime;
    public float dodgeStaminaCost;
    [Space]
    public bool damageAbsorptionActive;
    public float damageAbsorptionAmount;
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("STATS ALTERATIONS")]
    public float criticalChanceMultiplier = 1f;
    public float normalDamagesMultiplier = 1f;
    public float staminaRegenMultiplier = 1f;
    [Space]
    [Header("________________________________________")]
    [Space]
    [Header("LEVELING")]
    public int maxLevel;
    public int currentLevel;
    [Space]
    public float currentXp;
    public int[] perLvlExpNeeded = new int[50];
    [Space]
    public float perLvlHealthAugment;
    public float perLvlHealthRegenAugment;
    public float perLvlStaminaAugment;
    public float perLvlStaminaRegenAugment;
    public float perLvlNormalDamageAugment;

    public void Init()
    {
        jumpsAvailable = maxJumpNumber;
        staminaRegenRatio = 1f;

        // INIT STATS

        healthMax = baseHealthMax;
        health = healthMax;
        healthRegen = baseHealthRegen;

        armor = baseArmor;

        staminaMax = baseStaminaMax;
        stamina = staminaMax;
        staminaRegen = baseStaminaRegen;

        normalDamages = baseNormalDamages;

        criticalChances = baseCriticalChances;
        criticalDamagesM = baseCriticalDamagesM;

        capacitiesDamagesM = baseCapacitiesDamagesM;
        cooldownReduction = baseCooldownReduction;

        maxLevel = perLvlExpNeeded.Length + 1;

    }

    public IEnumerator LateInit()
    {
        yield return new WaitForSeconds(0.2f);

        HudManager.Singleton.playerInfoUI.UpdateLvl(currentLevel);
        HudManager.Singleton.playerInfoUI.UpdateXpProgressBar(GetXpProgressFactor());
    }

    public void SetJumpVelocity(float gravity, bool isRunning)
    {
        float tmpJumpHeight = (jumpsUsed == 0) ? firstJumpHeight : jumpHeight;

        if (isRunning)
        {
            tmpJumpHeight *= runningJumpHeightMultiplier;
            runningJump = true;
        }

        timeToJumpApex = Mathf.Sqrt(-(2 * tmpJumpHeight) / gravity);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    public void UpdateRegenStats()
    {
        if (health < healthMax && health > 0f)
        {
            health += healthRegen * Time.deltaTime;
            if (health > healthMax)
            {
                health = healthMax;
            }
        }

        if (stamina < staminaMax)
        {
            stamina += staminaRegen * staminaRegenRatio * Time.deltaTime;
            if (stamina > staminaMax)
            {
                stamina = staminaMax;
            }
        }
    }

    public void HealhReduction(float amount)
    {
        health -= amount;
        if (health < 0f)
        {
            health = 0f;
        }
    }

    public void StaminaReduction(float amount)
    {
        stamina -= amount;
        if (stamina < 0f)
        {
            stamina = 0f;
        }
    }

    public void EarnXP(float amount)
    {
        if (currentLevel == maxLevel)
        {
            return;
        }

        currentXp += amount;

        if (currentXp >= perLvlExpNeeded[currentLevel])
        {
            while(currentXp >= perLvlExpNeeded[currentLevel])
            {
                LvlUp();
            }
        }
        else
        {
            HudManager.Singleton.playerInfoUI.UpdateXpProgressBar(GetXpProgressFactor());
        }
    }

    public void LvlUp()
    {
        currentXp -= perLvlExpNeeded[currentLevel];

        healthMax += perLvlHealthAugment;
        healthRegen += perLvlHealthRegenAugment;
        staminaMax += perLvlStaminaAugment;
        staminaRegen += perLvlStaminaRegenAugment;
        normalDamages += perLvlNormalDamageAugment;

        currentLevel++;

        HudManager.Singleton.statsDisplayer.UpdateStats();
        HudManager.Singleton.playerInfoUI.UpdateLvl(currentLevel);
        HudManager.Singleton.playerInfoUI.UpdateXpProgressBar(GetXpProgressFactor());
    }

    public void UpdateLevelFromSave()
    {
        for (int i = 0; i < currentLevel; i++)
        {
            healthMax += perLvlHealthAugment;
            healthRegen += perLvlHealthRegenAugment;
            staminaMax += perLvlStaminaAugment;
            staminaRegen += perLvlStaminaRegenAugment;
            normalDamages += perLvlNormalDamageAugment;
        }

        HudManager.Singleton.statsDisplayer.UpdateStats();
        HudManager.Singleton.playerInfoUI.UpdateLvl(currentLevel);
        HudManager.Singleton.playerInfoUI.UpdateXpProgressBar(GetXpProgressFactor());
    }

    public float GetXpProgressFactor()
    {
        return currentXp / perLvlExpNeeded[currentLevel];
    }

    public void Restore()
    {
        health = healthMax;
        stamina = staminaMax;
        fallCount = 0;
    }

    public enum State
    {
        Idle,
        Running,
        Jumping_Impulse,
        Jumping_InAir,
        Jumping_Landing,
        Dodging,
        Attacking,
        Taking_Damages,
        KnockBack,
        Stun,
        Towed,
        Dying,
        Dead
    }

    [System.Serializable]
    public class SavedDatas
    {
        public int currentLevel;
        public float currentXP;

        public void ToPlayerStats(PlayerStats _playerStats)
        {
            _playerStats.currentXp = currentXP;
            _playerStats.currentLevel = currentLevel;

            _playerStats.UpdateLevelFromSave();
            _playerStats.Restore();
        }

        public void FromPlayerStats(PlayerStats _playerStats)
        {
            currentLevel = _playerStats.currentLevel;
            currentXP = _playerStats.currentXp;
        }
    }
}
