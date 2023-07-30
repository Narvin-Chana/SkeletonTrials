using System;
using UnityEngine;

public enum EnemyStrategy
{
    Idle,
    Chase,
    Shoot,
}

public class EnemyController : MonoBehaviour
{
    public int health = 3;

    public float speed;
    public float attackRange;
    public float attackRadius;
    public float attackCooldown;
    public int attackDamage;
    public Transform attackTransform;
    public LayerMask attackableLayer;

    public float damageStunTime;

    public EnemyStrategy strategy;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private float attackCounter;
    private float damageStunCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damageStunCounter > 0)
        {
            damageStunCounter -= Time.deltaTime;
            if (damageStunCounter <= 0)
            {
                animator.SetTrigger("stunOver");
            }

            return;
        }

        if (attackCounter > 0)
        {
            attackCounter -= Time.deltaTime;
            return;
        }

        switch (strategy)
        {
            case EnemyStrategy.Idle:
                animator.SetBool("isMoving", false);
                rb.velocity = Vector2.zero;
                break;
            case EnemyStrategy.Chase:
                float distanceToPlayer = Vector2.Distance(transform.position,
                    TopDownCharacterController.Instance.transform.position);

                if (distanceToPlayer < attackRange)
                {
                    // Stop moving
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isMoving", false);

                    Attack();
                }
                else
                {
                    Vector2 directionToPlayer =
                        (TopDownCharacterController.Instance.transform.position - transform.position).normalized;
                    animator.SetBool("isMoving", true);
                    rb.velocity = directionToPlayer * speed;
                }

                break;
            case EnemyStrategy.Shoot:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (rb.velocity.x != 0)
        {
            sr.flipX = rb.velocity.x < 0;
            // Modify attackTransform's position
            attackTransform.localPosition = new Vector2(sr.flipX ? -0.158f : 0.158f, attackTransform.localPosition.y);
            // Modify box collider's offset
            bc.offset = new Vector2(sr.flipX ? 0.03f : -0.03f, bc.offset.y);
        }
    }

    private void Attack()
    {
        attackCounter = attackCooldown;
        animator.SetTrigger("Attack");
    }

    public void PerformCircleCast()
    {
        RaycastHit2D[] hits =
            Physics2D.CircleCastAll(attackTransform.position, attackRadius, Vector2.zero, 0, attackableLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<TopDownCharacterController>().TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (damageStunCounter > 0)
        {
            return;
        }

        rb.velocity = Vector2.zero;
        damageStunCounter = damageStunTime;
        health -= damage;
        animator.SetTrigger(health <= 0 ? "Die" : "takeDamage");
        if (health <= 0)
        {
            bc.enabled = false;
            GameManager.Instance.enemiesLeftCounter--;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRadius);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}