using MeshCombineStudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class ScarlettBossBehavior : MonoBehaviour
{
    private GameObject Player;

    private Animator anim;

    private int nextAttack = 0; // 1 -> normal, 2-> ranged

    public GameObject prefabRanged;

    private float attackBuffer = 0f;
    public float attack1CD = 1f;
    public float attack2CD = 3f;

    public Vector3 moveDirection;
    public float maxDashTime = 1.0f;
    public float dashSpeed = 1.0f;
    private float currentDashTime;

    private bool isAttacking = false;

    private float bufferDelay;
    public float Delay = 0.3f;
    private bool isAttack2;
    private int numOfAttack2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        attackBuffer = attack1CD;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextAttack == 0)
        {
            nextAttack = Random.Range(1, 3);
        }

        attackBuffer -= Time.deltaTime;
        
        if(attackBuffer <= 0f)
        {
            if(nextAttack == 1)
            {
                Attack1();
                attackBuffer = attack1CD;
                nextAttack = 0;
            }else if (nextAttack == 2)
            {
                numOfAttack2 = 0;
                Attack2();
                attackBuffer = attack2CD;
                nextAttack = 0;
            }
        }

        Attack2Count();
        Dash();

    }

    public void Attack1()
    {
        currentDashTime = 0.0f;
    }

    public void Attack2Count()
    {
        if (isAttack2 && bufferDelay < Delay && numOfAttack2 < 4)
        {
            bufferDelay += Time.deltaTime;
        }
        else if(isAttack2 && numOfAttack2 < 4)
        {
            bufferDelay = 0;
            Attack2Ranged();
            numOfAttack2++;
        }
        else
        {
            bufferDelay = 0f;
        }
    }

    public void Attack2()
    {
        isAttack2 = true;
    }

    public void Attack2Ranged()
    {
        GameObject projectile = Instantiate(prefabRanged, transform.position, Quaternion.identity);

        projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y - 0.7f, projectile.transform.position.z);

        Vector3 direction = Player.transform.position - this.transform.position;
        direction = direction.normalized;

        direction = new Vector3(direction.x, Player.transform.position.y-1f, direction.z);

        // Add force or velocity to the projectile to move it in the direction
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * 10f;
        }

        GameObject projectile2 = Instantiate(prefabRanged, transform.position, Quaternion.identity);

        projectile2.transform.position = new Vector3(projectile2.transform.position.x, projectile2.transform.position.y - 0.7f, projectile2.transform.position.z);

        Vector3 direction2 = Player.transform.position - this.transform.position;
        direction2 = direction2.normalized;
        direction2 = Quaternion.Euler(0, 30, 0) * direction2;

        direction2 = new Vector3(direction2.x, Player.transform.position.y-1f, direction2.z);

        // Add force or velocity to the projectile to move it in the direction
        Rigidbody rb2 = projectile2.GetComponent<Rigidbody>();
        if (rb2 != null)
        {
            rb2.velocity = direction2 * 10f;
        }

        GameObject projectile3 = Instantiate(prefabRanged, transform.position, Quaternion.identity);

        projectile3.transform.position = new Vector3(projectile3.transform.position.x, projectile3.transform.position.y - 0.7f, projectile3.transform.position.z);

        Vector3 direction3 = Player.transform.position - this.transform.position;
        direction3 = direction3.normalized;
        direction3 = Quaternion.Euler(0, -30, 0) * direction3;

        direction3 = new Vector3(direction3.x, Player.transform.position.y - 1f, direction3.z);

        // Add force or velocity to the projectile to move it in the direction
        Rigidbody rb3 = projectile3.GetComponent<Rigidbody>();
        if (rb3 != null)
        {
            rb3.velocity = direction3 * 10f;
        }
    }

    public void Dash()
    {
        if (currentDashTime < maxDashTime)
        {
            moveDirection = Player.transform.position - this.transform.position;
            
            currentDashTime += Time.deltaTime;
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        this.GetComponent<CharacterController>().Move(moveDirection * dashSpeed * Time.deltaTime);
    }
}
