using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController1 : MonoBehaviour
{
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;

    public float speed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    private CharacterController characterController;
    private Vector2 moveDirection;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private bool isToolUse = false;
    private int comboCount = 0;
    private float lastClickTime;
    public GameObject RangedPrefab;


    public GameObject trajectoryLine;
    private Vector2 mousePosition;
    private Vector2 screenCenter;
    private Vector2 trajectoryDirection;
    private float rangedAngle;

    public GameObject meleeCollider;
    public PlayerInputAction playerControls;
    private InputAction move;
    private InputAction jump;
    private InputAction attack;
    private InputAction rangedGamepad;
    private InputAction rangedMouse;
    private InputAction aimGamepad;

    // Start is called before the first frame update

    private void Awake()
    {
        playerControls = new PlayerInputAction();
    }
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        rb = gameObject.GetComponent<Rigidbody>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();


        originalStepOffset = characterController.stepOffset;
    }

    private void OnEnable()
    {
        move = playerControls.Control.Move;
        move.Enable();

        jump = playerControls.Control.Jump;
        jump.Enable();
        jump.performed += Jump;

        attack = playerControls.Control.Attack;
        attack.Enable();
        attack.performed += Attack;

        rangedGamepad = playerControls.Control.RangedGamepad;
        rangedGamepad.Enable();
        rangedMouse = playerControls.Control.RangedMouse;
        rangedMouse.Enable();

        aimGamepad = playerControls.Control.AimGamepad;
        aimGamepad.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        attack.Disable();
        rangedGamepad.Disable();
        rangedMouse.Disable();
        aimGamepad.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (rangedGamepad.IsPressed() || rangedMouse.IsPressed())
        {
            trajectoryLine.SetActive(true);
            TrajectoryLine();
        }
        else
        {
            trajectoryLine.SetActive(false);
        }
        
        
        AttackAnimCheck();

        CharacterMove();
    }

    private void FixedUpdate()
    {
        
    }

    private void CharacterMove()
    {
        moveDirection = move.ReadValue<Vector2>();

        float horizontalInput = moveDirection.x;
        float verticalInput = moveDirection.y;

        if (isToolUse)
        {
            horizontalInput = 0;
            verticalInput = 0;
        }

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                anim.SetBool("isJumping", true);
                ySpeed = jumpSpeed;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = ySpeed;


        characterController.Move(velocity * Time.deltaTime);
        
        float x = horizontalInput;
        float y = verticalInput;

        if (x != 0 && x < 0)
        {
            sr.flipX = false;

        }
        else if (x != 0 && x > 0)
        {
            sr.flipX = true;
        }

        if ((x != 0 && (x > 0 || x < 0)) || (y != 0 && (y > 0 || y < 0)))
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            anim.SetBool("isWalk", false);
        }

        if (y != 0 && y > 0)
        {
            anim.SetBool("faceFront", false);
        }
        else if (y != 0 && y < 0)
        {
            anim.SetBool("faceFront", true);
        }

        if (characterController.isGrounded)
        {
            anim.SetBool("isJumping", false);
        }

        MeleeDirection(x);
    }

    private void Jump(InputAction.CallbackContext context) 
    {
        if (!isToolUse && Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }
    }

    private void Attack(InputAction.CallbackContext context) 
    {
        if (!isToolUse && !(rangedGamepad.IsPressed() || rangedMouse.IsPressed()))
        {
            MeleeAttack();
        }else if (!isToolUse && (rangedGamepad.IsPressed() || rangedMouse.IsPressed()))
        {
            RangedAttack();
        }
    }

    private void MeleeAttack()
    {
            if(comboCount <= 0)
            {
                comboCount++;
                isToolUse = true;
                anim.SetInteger("attackSeq", comboCount);
                anim.SetBool("isAttacking", true);
                lastClickTime = Time.time;
            }
    }

    private void RangedAttack()
    {
        GameObject projectile = Instantiate(RangedPrefab, transform.position, Quaternion.identity);

        // Set the rotation of the projectile
        projectile.transform.rotation = Quaternion.Euler(0, -rangedAngle, 0);

        // Get the direction vector from the angle
        Vector3 direction = new Vector3(Mathf.Cos(rangedAngle * Mathf.Deg2Rad), 0, Mathf.Sin(rangedAngle * Mathf.Deg2Rad));

        // Add force or velocity to the projectile to move it in the direction
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * 5f;
        }
    }

    private void AttackAnimCheck()
    {
        
        if (!isToolUse && Time.time - lastClickTime > 0.5f)
        {
            comboCount = 0;
        }
        if (anim.GetBool("isAttacking") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            meleeCollider.SetActive(true);
        }
        if (anim.GetBool("isAttacking") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
        {
            meleeCollider.SetActive(false);
        }
        if (anim.GetBool("isAttacking") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            isToolUse = false;
            anim.SetBool("isAttacking", false);
        }
    }

    private void MeleeDirection(float x)
    {
        if(x > 0)
        {
            meleeCollider.transform.localScale = new Vector3(-1f, meleeCollider.transform.localScale.y, meleeCollider.transform.localScale.z);
        }else if(x < 0)
        {
            meleeCollider.transform.localScale = new Vector3(1f, meleeCollider.transform.localScale.y, meleeCollider.transform.localScale.z);
        }
    }

    private void TrajectoryLine()
    {
        if (rangedMouse.IsPressed())
        {
            mousePosition = Input.mousePosition;
            screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            trajectoryDirection = mousePosition - screenCenter;
        }
        else if (rangedGamepad.IsPressed())
        {
            trajectoryDirection = aimGamepad.ReadValue<Vector2>();
        }
        

        rangedAngle = Mathf.Atan2(trajectoryDirection.y, trajectoryDirection.x) * Mathf.Rad2Deg;

        trajectoryLine.transform.rotation = Quaternion.Euler(0, -rangedAngle, 0);
    }
}
