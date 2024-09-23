using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;

    public float speed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;

    private CharacterController characterController;
    private float ySpeed;
    private float originalStepOffset;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private bool isToolUse = false;
    private int comboCount = 0;
    private float lastClickTime;

    public GameObject meleeCollider;
    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        rb = gameObject.GetComponent<Rigidbody>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();


        originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isToolUse && Input.GetButtonDown("Fire1"))
        {
            
            Attack();
        }
        AttackAnimCheck();


        CharacterMove();
    }

    private void FixedUpdate()
    {
        
    }

    private void CharacterMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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

        if (!isToolUse && Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
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


    private void Attack()
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
            meleeCollider.transform.localScale = new Vector3(-1, 1, 1);
        }else if(x < 0)
        {
            meleeCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
