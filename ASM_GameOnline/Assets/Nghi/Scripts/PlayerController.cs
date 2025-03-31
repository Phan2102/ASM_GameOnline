using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    //************************
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab; // Prefab đạn

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;


    [Networked] private Vector3 NetworkedPosition { get; set; }
    [Networked] private NetworkBool IsWalking { get; set; }
    [Networked] private NetworkBool IsRunning { get; set; }
    [Networked] private NetworkBool IsJumping { get; set; }
    [Networked] private NetworkBool IsFacingRight { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (IsProxy) return; // Nếu không phải máy local thì không xử lý đầu vào

        MovePlayer(); // Gọi trực tiếp MovePlayer() mà không cần UpdateState()
    }

    private void MovePlayer()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool isShooting = Input.GetMouseButtonDown(0); // Bấm chuột trái để bắn
        // Xác định tốc độ chạy hoặc đi bộ
        float speed = isShiftPressed ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Kiểm tra trạng thái di chuyển
        IsWalking = moveInput != 0 && !isShiftPressed;
        IsRunning = moveInput != 0 && isShiftPressed;

        // Nhảy
        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            IsJumping = true;
        }
        else if (rb.linearVelocity.y == 0)
        {
            IsJumping = false;
        }

        // Lật nhân vật theo hướng di chuyển
        if (moveInput > 0 && !facingRight) FlipCharacter(true);
        else if (moveInput < 0 && facingRight) FlipCharacter(false);

        // Cập nhật trạng thái lên Network
        NetworkedPosition = transform.position;
        IsFacingRight = facingRight;

        if (isShooting)
        {
            animator.SetTrigger("isShooting"); // Kích hoạt animation bắn
        }
    }

    public override void Render()
    {
        // Cập nhật vị trí từ Network
        transform.position = NetworkedPosition;

        // Cập nhật animation
        animator.SetBool("isWalking", IsWalking);
        animator.SetBool("isRunning", IsRunning);
        animator.SetBool("isJumping", IsJumping);

        // Cập nhật Flip hướng quay mặt
        if (IsFacingRight != facingRight)
        {
            FlipCharacter(IsFacingRight);
        }
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
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    //**Hàm này sẽ được gọi bởi Animation Event khi đến frame bắn**
    public void Shoot()
    {
        if (!Object.HasStateAuthority) return; // Chỉ máy chủ hoặc máy local có quyền bắn

        Runner.Spawn(bulletPrefab, firePoint.position, Quaternion.identity, Object.InputAuthority, (runner, obj) =>
        {
            obj.GetComponent<Bullet>().Initialize(facingRight ? 1 : -1);
        });
    }
}
