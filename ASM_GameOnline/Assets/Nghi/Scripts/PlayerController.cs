using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    private float moveInput;
    private bool isGrounded;
    private bool wasGroundedLastFrame = true;
    private bool wasFalling = false;
    //***
    private bool isLanding = false;
    private bool facingRight = true;
    private bool isAttacking = false;
    private float attackCooldown = 2f;
    private float lastAttackTime = -999f;

    [Networked] private Vector3 NetworkedPosition { get; set; }
    [Networked] private NetworkBool IsWalking { get; set; }
    [Networked] private NetworkBool IsRunning { get; set; }
    [Networked] private NetworkBool IsFacingRight { get; set; }
    [Networked, OnChangedRender(nameof(OnAttackIndexChanged))] private int AttackIndex { get; set; }
    [Networked] private NetworkBool IsAttackTrigger { get; set; }
    [Networked] private NetworkBool IsShooting { get; set; }

    [Networked] private NetworkBool IsJumping { get; set; }
    [Networked] private NetworkBool IsFalling { get; set; }
    [Networked] private NetworkBool IsLanding { get; set; }
    //***
    private bool wasJumpingLastFrame = false;
    private bool wasLandingLastFrame = false;

    [Networked] private TickTimer JumpResetTimer { get; set; }
    [Networked] private TickTimer LandResetTimer { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        HandleInput();

        //***
        if (JumpResetTimer.Expired(Runner))
            IsJumping = false;

        if (LandResetTimer.Expired(Runner))
            IsLanding = false;
    }

    private void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool isAttackPressed = Input.GetKeyDown(KeyCode.J);
        bool isShootPressed = Input.GetKeyDown(KeyCode.O);

        float speed = isShiftPressed ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Trạng thái di chuyển ngang
        IsWalking = moveInput != 0 && !isShiftPressed;
        IsRunning = moveInput != 0 && isShiftPressed;

        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.ResetTrigger("isLand");      // Reset trước để tránh bug
            animator.ResetTrigger("isJumping");   // Reset trước rồi mới set lại
            animator.SetTrigger("isJumping");

            //***
            IsJumping = true;
            JumpResetTimer = TickTimer.CreateFromSeconds(Runner, 0.3f); // Tuỳ thời lượng animation
            IsFalling = false;
            IsLanding = false;

            // Reset trạng thái sau animation
            StartCoroutine(ResetJumpTrigger());
        }

        // Flip
        if (moveInput > 0 && !facingRight) FlipCharacter(true);
        else if (moveInput < 0 && facingRight) FlipCharacter(false);

        NetworkedPosition = transform.position;
        IsFacingRight = facingRight;

        // Tấn công
        if (isAttackPressed && !isAttacking && Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            StartCoroutine(PerformAttack());
        }

        // Bắn
        if (isShootPressed)
        {
            IsShooting = true;
            animator.SetTrigger("isShooting");
        }

        // -------------------- FALL --------------------
        if (!isGrounded && rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("isFalling", true);
            //***
            IsFalling = true;
        }
        else if (isGrounded)
        {
            animator.SetBool("isFalling", false);
            //***
            IsLanding = false;
        }

        // -------------------- LAND --------------------
        if (!wasGroundedLastFrame && isGrounded)
        {
            isLanding = true;
            animator.SetTrigger("isLand");
            LandResetTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
            //***
            IsJumping = false;
            IsFalling = false;
            IsLanding = true;

            StartCoroutine(ResetLandTrigger());
        }

        // Update network
        NetworkedPosition = transform.position;
        IsFacingRight = facingRight;

        // Update trạng thái grounded frame trước
        wasGroundedLastFrame = isGrounded;
    }

    private IEnumerator ResetLandTrigger()
    {
        yield return new WaitForSeconds(0.3f); // Thời lượng animation land
        animator.ResetTrigger("isLand");
        isLanding = false;
    }

    private IEnumerator ResetJumpTrigger()
    {
        yield return new WaitForSeconds(0.1f); // Tùy theo thời gian Trigger
        //!!!!!!!!!!!!!!!!!!
        //IsJumping = false;
    }

    IEnumerator PerformAttack()
    {
        //isAttacking = true;
        IsAttackTrigger = true;

        AttackIndex = Random.Range(0, 6);
        //animator.SetInteger("AttackIndex", AttackIndex);
        //animator.SetTrigger("isAttack");

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public override void Render()
    {
        transform.position = NetworkedPosition;

        //***
        animator.SetBool("isFalling", IsFalling);
        //if (IsJumping) animator.SetTrigger("isJumping");
        //if (IsLanding) animator.SetTrigger("isLand");
        //***

        if (!isLanding) // Đừng đụng tới Idle/Fall/Jump nếu đang Land
        {
            animator.SetBool("isWalking", IsWalking);
            animator.SetBool("isRunning", IsRunning);
        }

        if (IsFacingRight != facingRight)
        {
            FlipCharacter(IsFacingRight);
        }

        if (IsShooting)
        {
            animator.SetTrigger("isShooting");
            IsShooting = false;
        }

        // 👉 CHỈ PHÁT Trigger Jump nếu vừa mới nhảy
        if (IsJumping && !wasJumpingLastFrame)
        {
            animator.SetTrigger("isJumping");
        }

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // ✅ Nếu không còn nhảy nhưng frame trước là nhảy → Reset trigger lại
        else if (!IsJumping && wasJumpingLastFrame)
        {
            animator.ResetTrigger("isJumping");
        }

        // 👉 CHỈ PHÁT Trigger Land nếu vừa mới tiếp đất
        if (IsLanding && !wasLandingLastFrame)
        {
            animator.SetTrigger("isLand");
        }

        // 👉 Cập nhật biến tạm để so sánh ở frame sau
        wasJumpingLastFrame = IsJumping;
        wasLandingLastFrame = IsLanding;
    }


    private void OnAttackIndexChanged()
    {
        animator.SetInteger("AttackIndex", AttackIndex);
        animator.SetTrigger("isAttack");
    }

    private void FlipCharacter(bool faceRight)
    {
        facingRight = faceRight;
        transform.localScale = new Vector3(faceRight ? 1 : -1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        // 🔒 Đảm bảo triệt tiêu Jump/Fall khi tiếp đất
        IsJumping = false;
        IsFalling = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // Gọi bởi animation event
    public void Shoot()
    {
        if (!HasStateAuthority) return;

        Runner.Spawn(bulletPrefab, firePoint.position, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        {
            obj.GetComponent<Bullet>().Initialize(facingRight ? 1 : -1);
        });
    }
}
