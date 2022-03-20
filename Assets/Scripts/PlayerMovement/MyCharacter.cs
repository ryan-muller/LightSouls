
using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MyCharacter : ThirdPersonCharacter
{
    #region EDITOR EXPOSED FIELDS

    public AnimatorOverrideController OneHand_WeaponController;
    public List<Slash> slashes;

    private float rollDuration = 1.6f;
    private float rollImpulse = 5f;

    #endregion

    #region FIELDS

    private PlayerAnimationHandler _anim;

    //private Animator _animator;

    private AnimatorOverrideController originalAnimator;

    private PlayerInventory playerInventory;


    private bool isAttacking;
    private bool isRolling;
    private float rollingTime;
    private Vector3 rollingDirection;

    //public GameObject sword;
    private int comboCount = 0;
    private bool canCombo;

    public string lastAttack;

    #endregion


    #region INPUT ACTIONS

    /// <summary>
    /// Interact InputAction.
    /// </summary>

    private InputAction drawWeaponInputAction { get; set; }
    private InputAction holsterWeaponInputAction { get; set; }
    private InputAction lightAttackInputAction { get; set; }
    private InputAction heavyAttackInputAction { get; set; }
    private InputAction rollingInputAction { get; set; }

    #endregion

    #region INPUT ACTION HANDLERS

    /// <summary>
    /// Interact input action handler.
    /// </summary>

    private void OnDrawWeapon(InputAction.CallbackContext context)
    {
        DrawWeapon();
    }

    private void OnHolsterWeapon(InputAction.CallbackContext context)
    {
        HolsterWeapon();
    }

    private void OnLightAttack(InputAction.CallbackContext context)
    {
        if (!IsGrounded())
        {
            return;
        }
          
        if (canCombo)
        {
            HandleWeaponCombo(playerInventory.rightWeapon);
        }
        else
        {
            HandleLightAttack(playerInventory.rightWeapon);
        }         
    }

    private void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (!IsGrounded())
        {
            return;
        }

        if (!isAttacking)
        {
            HandleHeavyAttack(playerInventory.rightWeapon);         
            return;
        }

    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        if (!IsGrounded())
            return;

        rollingDirection = GetMovementDirection();
        HandleRollingAndSprinting();
    }

    #endregion

    #region METHODS

    protected override void OnAwake()
    {
        base.OnAwake();

        DisableSlashes();
        playerInventory = GetComponentInChildren<PlayerInventory>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        Rolling();
        canCombo = _animator.GetBool("CanDoCombo");
    }

    /// <summary>
    /// Draw Weapon
    /// </summary>

    public void DrawWeapon()
    {
        //sword.SetActive(true);
        _animator.runtimeAnimatorController = OneHand_WeaponController;
    }

    /// <summary>
    /// HolsterWeapon
    /// </summary>

    public void HolsterWeapon()
    {
        //sword.SetActive(false);
        _animator.runtimeAnimatorController = originalAnimator;
    }

    /// <summary>
    /// Attacking
    /// </summary>
    /// 
    public void HandleWeaponCombo(WeaponItem weapon)
    {
        _animator.SetBool("CanDoCombo", false);
        if (lastAttack == weapon.OH_Light_Attack_1)
        {
            _anim.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
        }
        
    }
     
    private void HandleLightAttack(WeaponItem weapon)
    {
        comboCount++;
        isAttacking = true;
        _anim.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
        lastAttack = weapon.OH_Light_Attack_1;
        //StartCoroutine(Attack());
    }

    private void HandleHeavyAttack(WeaponItem weapon)
    {
        comboCount++;
        isAttacking = true;
        _anim.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
        lastAttack = weapon.OH_Heavy_Attack_1;
    }
        
    public IEnumerator Attack()
    {
        canCombo = false;
        for (int i = 0; i < slashes.Count; i++)
        {
            yield return new WaitForSeconds(slashes[i].delay);
            slashes[i].slashObj.SetActive(true);
        }

        yield return new WaitForSeconds(.2f);
        canCombo = true;

        yield return new WaitForSeconds(.6f);
        DisableSlashes();
        isAttacking = false;
    }

    private void DisableSlashes()
    {
        for (int i = 0; i < slashes.Count; i++)
            slashes[i].slashObj.SetActive(false);
    }

    private void HandleRollingAndSprinting()
    {
        if (_animator.GetBool("IsInteracting"))
            return;

        if (isRolling)
            return;

        isRolling = true;
        rollingTime = 0f;

        useSeparateBrakingFriction = true;
        brakingFriction = 0.3f;

        


        _anim.PlayTargetAnimation("Stand To Roll",true);
        //Quaternion rollRotation = Quaternion.LookRotation(rollingDirection);
        //SetRotation(rollRotation);

        StartCoroutine(RollMovement());
    }

    private void StopRolling()
    {
        if (!isRolling)
            return;

        isRolling = false;

        useSeparateBrakingFriction = false;
        brakingFriction = 0f;

        //SetVelocity(Vector3.zero);
    }

    private IEnumerator RollMovement()
    {
        yield return new WaitForSeconds(0.3f);
        LaunchCharacter(rollingDirection * rollImpulse,true);
    }

    private void Rolling()
    {
        if (!isRolling)
            return;

        rollingTime += deltaTime;
            

        if (rollingTime > rollDuration)
            StopRolling();
    }

    protected override void HandleInput()
    {
        base.HandleInput();
        
        //Set direction to roll direction to prevent rotating while rolling
        
        if (isRolling)
            SetMovementDirection(rollingDirection);


        
        if (animator.GetBool("IsInteracting"))
        {
            SetVelocity(Vector3.zero);
            SetMovementDirection(Vector3.zero);
            //DeinitPlayerInput();
        }
        else
        {
            //InitPlayerInput();
        }
            
    }

    /// <summary>
    /// Setup player input actions.
    /// </summary>

    protected override void InitPlayerInput()
    {
        // Setup base input actions (eg: Movement, Jump, Sprint, Crouch)

        base.InitPlayerInput();

        // Setup Interact input action handlers

        _anim = GetComponentInChildren<PlayerAnimationHandler>();
        _animator = GetComponentInChildren<Animator>();

        originalAnimator = new AnimatorOverrideController(_animator.runtimeAnimatorController);


        drawWeaponInputAction = inputActions.FindAction("DrawWeapon");
        holsterWeaponInputAction = inputActions.FindAction("HolsterWeapon");
        lightAttackInputAction = inputActions.FindAction("LightAttack");
        heavyAttackInputAction = inputActions.FindAction("HeavyAttack");
        rollingInputAction = inputActions.FindAction("Roll");


        //Adding Drawing weapon method to input
        if (drawWeaponInputAction != null)
        {
            drawWeaponInputAction.performed += OnDrawWeapon;

            drawWeaponInputAction.Enable();
        }

        //Add Holster weapon method to input
        if (holsterWeaponInputAction != null)
        {
            holsterWeaponInputAction.performed += OnHolsterWeapon;

            holsterWeaponInputAction.Enable();
        }

        //Add Attacking method to input
        if (lightAttackInputAction != null)
        {
            lightAttackInputAction.performed += OnLightAttack;

            lightAttackInputAction.Enable();
        }

        if (heavyAttackInputAction != null)
        {
            heavyAttackInputAction.performed += OnHeavyAttack;

            heavyAttackInputAction.Enable();
        }

        //Add Rolling method to input
        if (rollingInputAction != null)
        {
            rollingInputAction.performed += OnRoll;
            rollingInputAction.Enable();
        }

            
    }

    /// <summary>
    /// Unsubscribe from input action events and disable input actions.
    /// </summary>

    protected override void DeinitPlayerInput()
    {
        base.DeinitPlayerInput();

        if (drawWeaponInputAction != null)
        {
            drawWeaponInputAction.performed -= OnDrawWeapon;
            drawWeaponInputAction.Disable();
            drawWeaponInputAction = null;
        }

        //Add Holster weapon method to input
        if (holsterWeaponInputAction != null)
        {
            holsterWeaponInputAction.performed -= OnHolsterWeapon;
            holsterWeaponInputAction.Disable();
            holsterWeaponInputAction = null;
        }

        //Add Attacking method to input
        if (lightAttackInputAction != null)
        {
            lightAttackInputAction.performed -= OnLightAttack;
            lightAttackInputAction.Disable();
            lightAttackInputAction = null;
        }

        if (heavyAttackInputAction != null)
        {
            heavyAttackInputAction.performed -= OnHeavyAttack;
            heavyAttackInputAction.Disable();
            heavyAttackInputAction = null;
        }

        //Add Rolling method to input
        if (rollingInputAction != null)
        {
            rollingInputAction.performed -= OnRoll;
            rollingInputAction.Disable();
            rollingInputAction = null;
        }
    }

    #endregion
}

