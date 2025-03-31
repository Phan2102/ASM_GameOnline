using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;

    [Header("Waypoints & Player")]
    [SerializeField] private Transform pointA, pointB;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemyStateMachine stateMachine;

    public Transform Player => player;
    public float WalkSpeed => walkSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float AttackRange => attackRange;
    public Transform PointA => pointA;
    public Transform PointB => pointB;

    public bool IsPlayerDetected { get; private set; }
    public bool IsInAttackRange { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stateMachine = new EnemyStateMachine(this);
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        IsPlayerDetected = distanceToPlayer < detectionRange;
        IsInAttackRange = distanceToPlayer < attackRange;

        stateMachine.Update();
    }

    public EnemyStateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public void Move(float speed)
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        transform.localScale = new Vector3(Mathf.Sign(speed), 1, 1);
    }

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void SetAnimation(string anim)
    {
        animator.Play(anim);
    }

    public void Attack()
    {
        int randomAttack = Random.Range(0, 3);
        string attackAnim = $"Male_SoulBender_Attack{randomAttack}";
        SetAnimation(attackAnim);
    }
}


