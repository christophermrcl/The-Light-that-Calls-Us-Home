using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundDist;

    public LayerMask terrainLayer;
    public Rigidbody rb;
    public SpriteRenderer sr;
    public Animator anim;
    public PlayerState pst;

    public float jumpForce = 20f;
    // Start is called before the first frame update
    void Start()
    {
        pst = gameObject.GetComponent<PlayerState>();
        rb = gameObject.GetComponent<Rigidbody>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        RaycastHit hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;

        
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }
        */

        //MOVE

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(x, 0, y);
        rb.velocity = moveDir * speed;

        //JUMP

        if (pst.isGrounded && Input.GetButtonDown("Jump"))
        {
            pst.isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        if (x != 0 && x < 0)
        {
            sr.flipX = false;

        } else if (x != 0 && x > 0)
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
        }else if(y != 0 && y < 0)
        {
            anim.SetBool("faceFront", true);
        }
    }

    private void FixedUpdate()
    {
        
    }
}
