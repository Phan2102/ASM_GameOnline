using Fusion;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NPC : NetworkBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float minFollowDistance = 1f;
    [SerializeField] private float maxFollowRange = 10f;

    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private float respawnTime = 5f;
    [SerializeField] private GameObject hpBarRoot;

    [Networked] public int CurrentHP { get; set; }

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellSpawnPoint;

    [Networked] private TickTimer attackDelay { get; set; }
    [Networked] public NetworkObject Target { get; set; }
    [Networked] private NetworkBool TriggerAttackAnim { get; set; }

    [Networked] private float NetworkedSpeed { get; set; }
    [Networked] private bool NetworkedFlipX { get; set; }
    [Networked] private NetworkBool IsAttacking { get; set; }
    [Networked] private NetworkBool IsDead { get; set; }

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform currentEnemy;

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        CurrentHP = maxHP;
        IsDead = false;
    }

    public void SetTarget(Transform target)
    {
        if (target != null)
            Target = target.GetComponent<NetworkObject>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || Target == null || IsDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Target.transform.position);

        // Không xử lý nếu quá xa hoặc đang chết
        if (distanceToPlayer > maxFollowRange)
            currentEnemy = null;

        if (currentEnemy == null || Vector3.Distance(transform.position, currentEnemy.position) > attackRange)
            FindClosestEnemy();

        if (currentEnemy != null && distanceToPlayer <= maxFollowRange)
        {
            Vector3 dirToEnemy = currentEnemy.position - transform.position;
            Flip(dirToEnemy.x);

            if (dirToEnemy.magnitude <= attackRange && attackDelay.ExpiredOrNotRunning(Runner))
                StartCoroutine(AttackCoroutine());

            NetworkedSpeed = 0;
            return;
        }



        // Không có enemy → follow player
        MoveTo(Target.transform.position);
    }

    private void MoveTo(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        float distance = direction.magnitude;
        Vector3 moveDirection = Vector3.zero;

        if (distance > followDistance)
            moveDirection = direction.normalized;
        else if (distance < minFollowDistance)
            moveDirection = -direction.normalized;

        transform.position += moveDirection * followSpeed * Runner.DeltaTime;
        Flip(moveDirection.x);
        NetworkedSpeed = moveDirection.magnitude > 0 ? followSpeed : 0;
        IsAttacking = false;
    }

    private void Flip(float x)
    {
        if (Mathf.Abs(x) > 0.1f)
        {
            bool shouldFlip = x < 0;
            if (spriteRenderer.flipX != shouldFlip)
            {
                spriteRenderer.flipX = shouldFlip;
                NetworkedFlipX = shouldFlip;
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        IsAttacking = true;
        TriggerAttackAnim = true;
        attackDelay = TickTimer.CreateFromSeconds(Runner, attackCooldown);

        yield return new WaitForSeconds(0.3f); // Sync anim

        if (spellPrefab && currentEnemy != null && spellSpawnPoint != null)
        {
            Runner.Spawn(spellPrefab, spellSpawnPoint.position, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
            {
                obj.GetComponent<MagicProjectile>().Init(currentEnemy);
            });
        }

        yield return new WaitForSeconds(0.1f);
        IsAttacking = false;
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist && dist <= attackRange)
            {
                minDist = dist;
                closest = e.transform;
            }
        }

        currentEnemy = closest;
    }

    public override void Render()
    {
        if (animator)
        {
            animator.SetFloat("Speed", NetworkedSpeed);

            if (TriggerAttackAnim)
            {
                animator.SetTrigger("Attack");
                TriggerAttackAnim = false;
            }

            animator.SetBool("Dead", IsDead);
        }

        if (hpFillImage != null)
            hpFillImage.fillAmount = (float)CurrentHP / maxHP;

        if (spriteRenderer)
            spriteRenderer.flipX = NetworkedFlipX;
    }

    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority || IsDead) return;

        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("NPC died");
        IsDead = true;
        NetworkedSpeed = 0;

        if (hpBarRoot != null)
            hpBarRoot.SetActive(false);

        spriteRenderer.enabled = false;

        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        CurrentHP = maxHP;
        IsDead = false;

        spriteRenderer.enabled = true;

        if (hpBarRoot != null)
            hpBarRoot.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            TakeDamage(10);
    }
}
