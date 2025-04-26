/*using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;

public class EnemyController : NetworkBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1f; // Đánh khi trong 1m
    [SerializeField] private float chaseStopRange = 5f; // Ngưng chase khi >5m
    [SerializeField] private float attackCooldown = 1f;

    [Header("Auto Setup")]
    private Transform pointA;
    private Transform pointB;
    private Transform currentTargetPoint;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask playerLayer;

    private GameObject currentTargetPlayer;
    private Coroutine attackRoutine;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    public int damage = 10;
    public Transform attackPoint;


    private void Start()
    {
        pointA = GameObject.Find("PointA")?.transform;
        pointB = GameObject.Find("PointB")?.transform;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("⚠️ Không tìm thấy PointA hoặc PointB trong scene!");
            enabled = false;
            return;
        }

        currentTargetPoint = pointB;

        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Tìm Player gần nhất
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerInRange != null)
        {
            currentTargetPlayer = playerInRange.gameObject;
            float distance = Vector2.Distance(transform.position, currentTargetPlayer.transform.position);

            if (distance <= attackRange)
            {
                if (currentState != State.Attack)
                {
                    currentState = State.Attack;
                    StartAttackLoop();
                }
            }
            else if (distance <= chaseStopRange)
            {
                StopAttackLoop();
                currentState = State.Chase;
            }
            else
            {
                StopAttackLoop();
                currentState = State.Patrol;
            }
        }
        else
        {
            currentTargetPlayer = null;
            StopAttackLoop();
            currentState = State.Patrol;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack(); // dừng movement và giữ hướng
                break;
        }

        UpdateAnimation();
    }

    private void Patrol()
    {
        Vector2 dir = (currentTargetPoint.position - transform.position).normalized;
        rb.linearVelocity = dir * patrolSpeed;

        float distance = Vector2.Distance(transform.position, currentTargetPoint.position);
        if (distance < 0.5f)
            currentTargetPoint = (currentTargetPoint == pointA) ? pointB : pointA;

        Flip(dir.x);
    }

    private void Chase()
    {
        if (currentTargetPlayer == null) return;

        Vector2 dir = (currentTargetPlayer.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed;

        Flip(dir.x);
    }

    private void Attack()
    {
        //**************************
        if (!HasStateAuthority) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (var hit in hits)
        {
            HitBox box = hit.GetComponent<HitBox>();
            if (box != null && box.type == HitBox.HitBoxType.Player)
            {
                //var health = box.owner.GetComponent<PlayerProperties>();
                var health = box.owner.GetComponent<HealthSystem>();
                health?.TakeDamage(7);
            }
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        rb.linearVelocity = Vector2.zero;

        if (currentTargetPlayer != null)
        {
            float dirX = currentTargetPlayer.transform.position.x - transform.position.x;
            Flip(dirX);
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (currentTargetPlayer == null) yield break;

            float distance = Vector2.Distance(transform.position, currentTargetPlayer.transform.position);
            if (distance > attackRange) yield break;

            rb.linearVelocity = Vector2.zero;

            int randomAttack = Random.Range(0, 8);
            animator.SetInteger("AttackIndex", randomAttack);
            animator.SetTrigger("isAttack");

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void StartAttackLoop()
    {
        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AttackLoop());
    }

    private void StopAttackLoop()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        animator.SetBool("isWalk", currentState == State.Patrol && isMoving);
        animator.SetBool("isRun", currentState == State.Chase && isMoving);
    }

    private void Flip(float directionX)
    {
        if (directionX == 0) return;
        transform.localScale = new Vector3(Mathf.Sign(directionX), 1, 1);
    }

    //$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        
        //Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseStopRange);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

   
}
*/

using UnityEngine;
using Fusion;
using System.Collections;

public class EnemyController : NetworkBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float chaseStopRange = 5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Auto Setup")]
    private Transform pointA;
    private Transform pointB;
    private Transform currentTargetPoint;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask playerLayer;
    public Transform attackPoint;

    private GameObject currentTargetPlayer;
    private Coroutine attackRoutine;
    private bool isAttacking = false;

    public int damage = 10;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private void Start()
    {
        pointA = GameObject.Find("PointA")?.transform;
        pointB = GameObject.Find("PointB")?.transform;

        if (pointA == null || pointB == null)
        {
            Debug.LogError("⚠️ Không tìm thấy PointA hoặc PointB trong scene!");
            enabled = false;
            return;
        }

        currentTargetPoint = pointB;

        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerInRange != null)
        {
            currentTargetPlayer = playerInRange.gameObject;
            float distance = Vector2.Distance(transform.position, currentTargetPlayer.transform.position);

            if (distance <= attackRange)
            {
                if (currentState != State.Attack)
                {
                    currentState = State.Attack;
                    StartAttackLoop();
                }
            }
            else if (distance <= chaseStopRange)
            {
                StopAttackLoop();
                currentState = State.Chase;
            }
            else
            {
                StopAttackLoop();
                currentState = State.Patrol;
            }
        }
        else
        {
            currentTargetPlayer = null;
            StopAttackLoop();
            currentState = State.Patrol;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                rb.linearVelocity = Vector2.zero;
                FacePlayer();
                break;
        }

        UpdateAnimation();
    }

    private void Patrol()
    {
        Vector2 dir = (currentTargetPoint.position - transform.position).normalized;
        rb.linearVelocity = dir * patrolSpeed;

        float distance = Vector2.Distance(transform.position, currentTargetPoint.position);
        if (distance < 0.5f)
            currentTargetPoint = (currentTargetPoint == pointA) ? pointB : pointA;

        Flip(dir.x);
    }

    private void Chase()
    {
        if (currentTargetPlayer == null) return;

        Vector2 dir = (currentTargetPlayer.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed;

        Flip(dir.x);
    }

    private void StartAttackLoop()
    {
        if (attackRoutine == null)
            attackRoutine = StartCoroutine(AttackLoop());
    }

    private void StopAttackLoop()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (currentTargetPlayer == null) yield break;

            float distance = Vector2.Distance(transform.position, currentTargetPlayer.transform.position);
            if (distance > attackRange) yield break;

            rb.linearVelocity = Vector2.zero;
            FacePlayer();

            if (HasStateAuthority)
            {
                DealDamage();
            }

            int randomAttack = Random.Range(0, 8);
            animator.SetInteger("AttackIndex", randomAttack);
            animator.SetTrigger("isAttack");

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (var hit in hits)
        {
            HitBox box = hit.GetComponent<HitBox>();
            if (box != null && box.type == HitBox.HitBoxType.Player)
            {
                var health = box.owner.GetComponent<HealthSystem>();
                health?.TakeDamage(damage);
            }
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        animator.SetBool("isWalk", currentState == State.Patrol && isMoving);
        animator.SetBool("isRun", currentState == State.Chase && isMoving);
        animator.SetBool("isIdle", currentState == State.Attack);
    }

    private void Flip(float directionX)
    {
        if (directionX == 0) return;
        transform.localScale = new Vector3(Mathf.Sign(directionX), 1, 1);
    }

    private void FacePlayer()
    {
        if (currentTargetPlayer != null)
        {
            float dirX = currentTargetPlayer.transform.position.x - transform.position.x;
            Flip(dirX);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseStopRange);

        Gizmos.color = Color.red;
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
