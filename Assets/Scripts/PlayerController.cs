using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{


    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;

    public float speed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    private CharacterController characterController;
    public Vector2 moveDirection;
    public Vector2 lastMoveDirection;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;
    public Vector3 velocity;

    private bool isToolUse = false;
    private int comboCount = 0;
    private float lastClickTime;
    public GameObject RangedPrefab;

    public float rangedAttackCooldown = 0.5f;
    private float lastRangedAttack;

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
    private InputAction interact;

    public LayerMask specialGroundLayer;
    public bool isSliding = false;
    private Vector2 slideDirection;

    private GameObject held;
    // Start is called before the first frame update

    private GameState GameState;

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

        GameState = GameObject.FindGameObjectWithTag("GameManager").gameObject.GetComponent<GameState>();

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

        interact = playerControls.Control.Interact;
        interact.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        attack.Disable();
        rangedGamepad.Disable();
        rangedMouse.Disable();
        aimGamepad.Disable();
        interact.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.isPaused)
        {
            anim.SetBool("isAttacking", false);
            anim.SetBool("isWalk", false);
            anim.SetBool("isJumping", false);
            trajectoryLine.SetActive(false);
            isToolUse = false;
            return;
        }

        if (rangedGamepad.IsPressed() || rangedMouse.IsPressed() && !isSliding && held == null)
        {
            trajectoryLine.SetActive(true);
            TrajectoryLine();
        }
        else
        {
            trajectoryLine.SetActive(false);
        }


        AttackAnimCheck();

        if (!isSliding)
        {
            CharacterMove();
        }
        else
        {
            Slide();
        }
        if (!IsSlippery() || (Mathf.Abs(characterController.velocity.x) < 0.01f && Mathf.Abs(characterController.velocity.z) < 0.01f))
        {
            isSliding = false;
        }

        if (lastMoveDirection != Vector2.zero)
        {
            SnapFace();
        }

        if (interact.IsPressed())
        {
            GetFront();
        }
        else
        {
            if (held != null)
            {
                held.transform.parent = null;
                held = null;
            }
        }

        if(held != null)
        {
            StopMovableObject();
        }
    }

    private void FixedUpdate()
    {

    }

    private void CharacterMove()
    {
        moveDirection = move.ReadValue<Vector2>();

        if (held)
        {
            if (lastMoveDirection.x == 0 && lastMoveDirection.y != 0)
            {
                moveDirection = new Vector2(0, moveDirection.y);
                if((lastMoveDirection.y < 0 && moveDirection.y > 0) || (lastMoveDirection.y > 0 && moveDirection.y < 0))
                {
                    moveDirection = Vector2.zero;
                }

            }

            if (lastMoveDirection.x != 0 && lastMoveDirection.y == 0)
            {
                moveDirection = new Vector2(moveDirection.x, 0);

                if ((lastMoveDirection.x < 0 && moveDirection.x > 0) || (lastMoveDirection.x > 0 && moveDirection.x < 0))
                {
                    moveDirection = Vector2.zero;
                }
            }
        }

        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
        }

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

        if (characterController.isGrounded && !isToolUse)
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

        velocity = movementDirection * magnitude;
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

        if (IsSlippery())
        {
            slideDirection = GetClosestDirection(move.ReadValue<Vector2>());
            isSliding = true;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!isToolUse && Input.GetButtonDown("Jump") && held == null)
        {
            jumpButtonPressedTime = Time.time;
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (isSliding || held != null)
        {
            return;
        }
        if (!isToolUse && !(rangedGamepad.IsPressed() || rangedMouse.IsPressed()))
        {
            MeleeAttack();
        }
        else if (!isToolUse && (rangedGamepad.IsPressed() || rangedMouse.IsPressed()))
        {
            if(Time.time - lastRangedAttack >= rangedAttackCooldown)
            {
                RangedAttack();
                lastRangedAttack = Time.time;
            }
            
        }
    }

    private void MeleeAttack()
    {
        if (comboCount <= 0)
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
        if (x > 0)
        {
            meleeCollider.transform.localScale = new Vector3(-1f, meleeCollider.transform.localScale.y, meleeCollider.transform.localScale.z);
        }
        else if (x < 0)
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


    private Vector2 GetClosestDirection(Vector2 input)
    {
        // Define the four cardinal directions
        Vector2 up = Vector2.up;
        Vector2 down = Vector2.down;
        Vector2 left = Vector2.left;
        Vector2 right = Vector2.right;

        // Compare the input direction with each cardinal direction and find the closest one
        Vector2[] directions = { up, down, left, right };
        Vector2 closestDirection = directions[0];
        float maxDot = Vector2.Dot(input.normalized, closestDirection); // Start by comparing with 'up'

        // Loop through each direction to find the closest one
        foreach (Vector2 dir in directions)
        {
            float dot = Vector2.Dot(input.normalized, dir);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestDirection = dir;
            }
        }

        return closestDirection;
    }

    private bool IsSlippery()
    {
        RaycastHit hit;
        // Cast a ray downwards from the player's position to check if the ground below is tagged as "Slippery"
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, specialGroundLayer))
        {
            if (hit.collider.CompareTag("Slippery"))
            {
                return true;
            }
        }
        return false;
    }

    private void Slide()
    {
        Debug.Log("sliding");
        Vector3 movementDirection = new Vector3(slideDirection.x, 0, slideDirection.y);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
        movementDirection.Normalize();

        velocity = movementDirection * magnitude;

        characterController.Move(velocity * Time.deltaTime);
    }

    private void GetFront()
    {
        RaycastHit hit;
        Vector3 directionRay = new Vector3(lastMoveDirection.x, 0, lastMoveDirection.y);
        if (Physics.Raycast(transform.position, directionRay, out hit, 0.5f))
        {
            if (hit.collider.CompareTag("Movable"))
            {
                if (held == null)
                {
                    held = hit.transform.gameObject;
                    hit.transform.parent = this.transform;
                }
            }
        }
    }

    private void SnapFace()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            lastMoveDirection = lastMoveDirection.x > 0 ? Vector2.right : Vector2.left; // Right or Left
        }
        else
        {
            lastMoveDirection = lastMoveDirection.y > 0 ? Vector2.up : Vector2.down; // Up or Down
        }
    }

    private void StopMovableObject()
    {
        RaycastHit hit;
        Vector3 directionRay = new Vector3(lastMoveDirection.x, 0, lastMoveDirection.y);
        float halfExt = held.GetComponent<BoxCollider>().bounds.size.x / 2;
        Vector3 halfExtend = new Vector3(halfExt + 0.001f, halfExt + 0.001f, halfExt + 0.001f);

        if (Physics.BoxCast(held.transform.position, halfExtend , directionRay, out hit, Quaternion.Euler(0,0,0), held.GetComponent<BoxCollider>().bounds.size.x / 32 + 0.01f))
        {
            held.transform.parent = null;
            held = null;
        }
    }
}
