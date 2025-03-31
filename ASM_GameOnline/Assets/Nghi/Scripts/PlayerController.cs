using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private  float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Transform groundCheck; // Điểm kiểm tra mặt đất
    [SerializeField] private LayerMask groundLayer; // Chỉ kiểm tra va chạm với mặt đất

    [SerializeField] private Transform firePoint; // Vị trí bắn
    [SerializeField] private GameObject bulletPrefab; // Prefab đạn
    private bool canShoot = true; // Kiểm soát chỉ bắn 1 viên mỗi lần nhấn phím
    public bool CanShoot { get; private set; } = true; // Cho phép bắn hay không

    //Thêm property để State có thể truy cập speed
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public Rigidbody2D Rb => rb;
    public Animator Anim => animator;
    

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerStateMachine stateMachine;

    public Vector2 MoveInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsShooting { get; private set; } // Để kiểm soát trạng thái bắn
    public bool isShooting => IsShooting; // Chỉ có getter

    private bool isAutoShooting;
    public bool IsAutoShooting => isAutoShooting;

    public void SetShooting(bool value)
    {
        IsShooting = value;
    }
    public void SetAutoShooting(bool value)
    {
        isAutoShooting = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator chưa được gán! Hãy đảm bảo Player có component Animator.");

        //stateMachine = new PlayerStateMachine(this);
        // KHÔNG TRUYỀN `this` nữa, mà gán State Machine vào PlayerController sau khi khởi tạo
        stateMachine = gameObject.AddComponent<PlayerStateMachine>();

        Debug.Log("State Machine Initialized: " + (stateMachine != null));
    }

    private void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool shootPressed = Input.GetKeyDown(KeyCode.J);
        bool autoShootHeld = Input.GetKey(KeyCode.J);

        //Cập nhật trạng thái IsGrounded
        //IsGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        IsGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);

        if (shootPressed && canShoot)
        {
            //Shoot();
            stateMachine.ChangeState(new ShootState());
        }

        //stateMachine.HandleInput(MoveInput, jumpPressed, runPressed, shootPressed);
        stateMachine.HandleInput(MoveInput, jumpPressed, runPressed, shootPressed, autoShootHeld);
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
    }

    public void Move(float speed)
    {
        if (speed == 0)
        {
            rb.linearVelocity = Vector2.zero; // Dừng ngay lập tức
        }
        else
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
            transform.localScale = new Vector3(Mathf.Sign(speed), 1, 1);
        }
        //rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        //if (speed != 0) transform.localScale = new Vector3(Mathf.Sign(speed), 1, 1);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }


    // Gọi từ Animation Event khi bắn**
    public void FireBullet()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(transform.localScale.x * 10f, 0);
        }
    }

    // Gọi từ Animation Event khi kết thúc animation bắn**
    public void ReturnIdle()
    {
        stateMachine.ChangeState(new IdleState());
        canShoot = true; // Cho phép bắn tiếp
    }

    private void Shoot()
    {
        if (!canShoot) return;
        canShoot = false; // Ngăn bắn tiếp
        if (firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            Debug.LogWarning("FirePoint chưa được gán trong Inspector!");
        }
    }


    private void ResetShoot()
    {
        canShoot = true;
    }

    //Event gọi khi animation bắn kết thúc
    public void OnShootEnd()
    {
        if (!Input.GetKey(KeyCode.J)) // Nếu không bấm giữ J nữa
        {
            IsShooting = false;
            stateMachine.ChangeState(new IdleState());
        }
    }

    public void SetAnimation(string animName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator chưa được gán!");
            return;
        }
        Debug.Log("Chuyển animation: " + animName);
        animator.Play(animName);
    }
}
