using UI;
using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    public static TopDownCharacterController Instance;

    public Animator playerSfxAnimator;
    
    public Transform attackTransform;
    public LayerMask attackableLayer;

    public float damageStunTime;
    public readonly PlayerStats PlayerStats = new();
    
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private ShootingController shootingController;

    private Vector2 dashDir;
    private float dashCounter;
    private float dashCoolCounter;
    private float attackCoolCounter;
    private float damageStunCounter;

    private float activeSpeed;
    private bool isFacingRight = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        shootingController = GetComponent<ShootingController>();

        activeSpeed = PlayerStats.Speed;
        dashDir = Vector2.zero;
    }

    private void Update()
    {
        // Manage inputs
        Vector2 dir = Vector2.zero;
        if (!IsDashing() && attackCoolCounter <= 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                isFacingRight = false;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                isFacingRight = true;
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            // Update player direction
            sr.flipX = !isFacingRight;
            // Update attackTransform position depending on facing direction
            attackTransform.position = transform.position + (Vector3)(isFacingRight ? Vector2.right : Vector2.left) / 3;

            if (Input.GetMouseButtonDown(0) && PlayerStats.HasAttack)
            {
                Attack();
            }

            if (Input.GetMouseButtonDown(1) && PlayerStats.HasSpell)
            {
                Spell(dir);
            }
        }

        rb.velocity = activeSpeed * (IsDashing() ? dashDir : dir);

        // Dash
        if (Input.GetKeyDown(KeyCode.Space) && PlayerStats.HasDash)
        {
            if (dashCoolCounter <= 0 && dashCounter <= 0)
            {
                // Play dash smoke animation
                playerSfxAnimator.SetTrigger("DashSmoke");
                playerSfxAnimator.gameObject.transform.position = transform.position + new Vector3(-0.03f, -0.6f, 0);

                bc.excludeLayers = LayerMask.GetMask("Layer 3");
                activeSpeed = PlayerStats.DashSpeed;
                dashCounter = PlayerStats.DashLength;
                dashDir = dir != Vector2.zero ? dir : isFacingRight ? Vector2.right : Vector2.left;
                animator.SetBool("isDashing", true);
            }
        }

        // Decay dash
        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0)
            {
                // Disable excludeLayers
                bc.excludeLayers = 0;
                activeSpeed = PlayerStats.Speed;
                dashCoolCounter = PlayerStats.DashCooldown;
                dashDir = Vector2.zero;
                animator.SetBool("isDashing", false);
            }
        }

        // Decay dash cd
        if (dashCoolCounter > 0)
        {
            dashCoolCounter -= Time.deltaTime;
        }

        // Decay attack cd
        if (attackCoolCounter > 0)
        {
            attackCoolCounter -= Time.deltaTime;
        }

        // Decay stun
        if (damageStunCounter > 0)
        {
            damageStunCounter -= Time.deltaTime;
            if (damageStunCounter <= 0)
            {
                animator.SetTrigger("stunEnd");
            }
        }
    }

    private void Spell(Vector2 dir)
    {
        shootingController.Shoot(dir != Vector2.zero ? dir : isFacingRight ? Vector2.right : Vector2.left,
            PlayerStats.SpellDamage, PlayerStats.SpellSpeed, Vector3.one * PlayerStats.SpellScale, PlayerStats.HasSpellPiercing);
    }

    private bool IsDashing()
    {
        return dashDir != Vector2.zero;
    }

    private void Attack()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(attackTransform.position, PlayerStats.AttackRange, Vector2.zero,
            0, attackableLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                hit.collider.gameObject.GetComponent<EnemyController>().TakeDamage(PlayerStats.AttackDamage);
            }
        }

        attackCoolCounter = PlayerStats.AttackCooldown;
        animator.SetTrigger("Attack");
    }

    public void TakeDamage(int damage)
    {
        if (damageStunCounter > 0)
        {
            return;
        }

        rb.velocity = Vector2.zero;
        damageStunCounter = damageStunTime;
        PlayerStats.Health -= damage;
        
        UIManager.Instance.UpdateHearts();
        
        if (PlayerStats.Health <= 0)
        {
            KillPlayer();
        }

        animator.SetTrigger("takeDamage");
    }

    private void KillPlayer()
    {
        Debug.Log("Player died");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackTransform.position, PlayerStats.AttackRange);
    }
}