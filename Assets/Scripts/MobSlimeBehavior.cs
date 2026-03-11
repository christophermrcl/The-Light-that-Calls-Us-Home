using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSlimeBehavior : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float attackRadius = 1.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private bool isAggro = false;
    private bool isAttacking = false;

    public GameObject leftAttackCollider;
    public GameObject rightAttackCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(NonAggroBehavior());
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (!isAggro)
            {
                
                isAggro = true;
                StopAllCoroutines();
            }

            AggroBehavior(distanceToPlayer);
        }
        else if (isAggro)
        {
            isAggro = false;
            isAttacking = false;
            StopAllCoroutines();
            StartCoroutine(NonAggroBehavior());
        }
    }

    private void AggroBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (!isAttacking)
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.Play("SlimeAttack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length/2);

        if (spriteRenderer.flipX)
        {
            rightAttackCollider.SetActive(true);
        }
        else
        {
            leftAttackCollider.SetActive(true);
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);

        leftAttackCollider.SetActive(false);
        rightAttackCollider.SetActive(false);

        animator.Play("SlimeIdle");

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void MoveTowardsPlayer()
    {
        animator.Play("SlimeMove");
        Vector3 direction = (player.position - transform.position).normalized;

        // Flip the sprite based on movement direction
        spriteRenderer.flipX = direction.x > 0;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private IEnumerator NonAggroBehavior()
    {
        while (!isAggro)
        {
            int action = Random.Range(0, 2); // 0 = idle, 1 = move
            if (action == 0)
            {
                animator.Play("SlimeIdle");
                yield return new WaitForSeconds(1f);
            }
            else
            {
                animator.Play("SlimeMove");
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

                // Flip the sprite based on movement direction
                spriteRenderer.flipX = randomDirection.x > 0;

                float moveDuration = 1f;
                for (float t = 0; t < moveDuration; t += Time.deltaTime)
                {
                    transform.position += randomDirection * moveSpeed * Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
