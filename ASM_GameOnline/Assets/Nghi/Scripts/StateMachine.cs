using System.Collections;
using UnityEngine;


public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState currentState;
    private PlayerController player; // Thay vì { get; }

    public PlayerController Player => player; // Getter để truy cập PlayerController

    //public PlayerController Player { get; private set; }

    private void Awake()
    {
        player = GetComponent<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Không tìm thấy PlayerController trên GameObject!");
        }
    }

    private void Start()
    {
        ChangeState(new IdleState()); // Đảm bảo trạng thái đầu tiên luôn được gán
    }

    //public PlayerStateMachine(PlayerController player)
    //{
    //    //this.Player = player;// Gán giá trị
    //    this.player = player ?? throw new System.ArgumentNullException(nameof(player)); // Chắc chắn không null
    //    ChangeState(new IdleState());//Chắc chắn rằng trạng thái đầu tiên được thiết lập
    //}

    public void HandleInput(Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld)
    {
        if (currentState == null)
        {
            Debug.LogError("CurrentState bị null khi xử lý input!");
            return;
        }

        currentState?.HandleInput(this, moveInput, jumpPressed, runPressed, shootPressed, autoShootHeld);
    }

    public void Update()
    {
        if (currentState == null)
        {
            Debug.LogError("CurrentState bị null trong PlayerStateMachine!");
            return;
        }

        currentState.UpdateState(this);
    }

    public void ChangeState(IPlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Không thể chuyển đổi sang trạng thái null!");
            return;
        }

        currentState = newState;
        currentState.EnterState(Player);
    }
}

public interface IPlayerState
{
    void EnterState(PlayerController player);
    void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld);
    void UpdateState(PlayerStateMachine stateMachine);
}

public class IdleState : IPlayerState
{
    public void EnterState(PlayerController player) => player.SetAnimation("Male_Gunner_Idle");

    public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld)
    {
        if (moveInput.x != 0)
        {
            stateMachine.ChangeState(runPressed ? new RunState() : new WalkState());
        }
        else if (jumpPressed)
        {
            stateMachine.ChangeState(new JumpState());
        }
        else if (shootPressed)
        {
            stateMachine.ChangeState(new ShootState());
        }
    }

    public void UpdateState(PlayerStateMachine stateMachine) { }
}

public class WalkState : IPlayerState
{
    public void EnterState(PlayerController player) => player.SetAnimation("Male_Gunner_Walk");

    public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld)
    {
        if (moveInput.x == 0) stateMachine.ChangeState(new IdleState());
        else if (runPressed) stateMachine.ChangeState(new RunState());
        else if (jumpPressed) stateMachine.ChangeState(new JumpState());
        else if (shootPressed) stateMachine.ChangeState(new ShootState());
    }

    public void UpdateState(PlayerStateMachine stateMachine)
    {
        PlayerController player = stateMachine.Player;
        player.Move(player.WalkSpeed * player.MoveInput.x);
    }
}

public class RunState : IPlayerState
{
    public void EnterState(PlayerController player) => player.SetAnimation("Male_Gunner_Run");

    public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld)
    {
        if (moveInput.x == 0) stateMachine.ChangeState(new IdleState());
        else if (!runPressed) stateMachine.ChangeState(new WalkState());
        else if (jumpPressed) stateMachine.ChangeState(new JumpState());
        else if (shootPressed) stateMachine.ChangeState(new ShootState());
    }

    public void UpdateState(PlayerStateMachine stateMachine)
    {
        PlayerController player = stateMachine.Player;
        player.Move(player.RunSpeed * player.MoveInput.x);
    }
}

public class JumpState : IPlayerState
{
    private bool isFalling = false;
    public void EnterState(PlayerController player)
    {
        player.SetAnimation("Male_Gunner_Jump");
        player.Jump();
    }

    public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld) { }

    public void UpdateState(PlayerStateMachine stateMachine)
    {
        PlayerController player = stateMachine.Player;
        // Kiểm tra khi nhân vật rơi xuống
        if (player.Rb.linearVelocity.y < 0 && !isFalling)
        {
            isFalling = true;
            player.SetAnimation("Male_Gunner_Landing");
        }

        // Cho phép di chuyển khi đang nhảy
        player.Move(player.WalkSpeed * player.MoveInput.x);

        //Nếu nhân vật tiếp đất, chuyển về Idle
        if (player.IsGrounded && Mathf.Abs(player.Rb.linearVelocity.y) < 0.1f)
        {
            if (player.MoveInput.x != 0)
                stateMachine.ChangeState(new WalkState());
            else
                stateMachine.ChangeState(new IdleState());
        }
    }
}

public class ShootState : IPlayerState
{
    //private bool hasShot = false;
    public void EnterState(PlayerController player)
    {
        player.SetAnimation("Male_Gunner_Shoot");
        player.SetShooting(true); // Gọi hàm thay vì set trực tiếp
    }


    public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld) { }

    public void OnShootEvent(PlayerStateMachine stateMachine)
    {
        stateMachine.Player.FireBullet();
    }

    public void OnShootEndEvent(PlayerStateMachine stateMachine)
    {
        stateMachine.Player.SetShooting(false);
        stateMachine.ChangeState(new IdleState());
    }

    public void UpdateState(PlayerStateMachine stateMachine)
    {
        //stateMachine.ChangeState(new IdleState());
        PlayerController player = stateMachine.Player;

        // Giữ nguyên animation nếu tiếp tục bắn, còn nếu không bắn nữa sẽ gọi `OnShootEnd()`
        if (!Input.GetKey(KeyCode.J) && !player.IsShooting)
        {
            stateMachine.ChangeState(new IdleState());
        }
    }

    public class AutoShootState : IPlayerState
    {
        private float shootInterval = 0.3f;
        private float shootTimer;

        public void EnterState(PlayerController player)
        {
            player.SetAnimation("Male_Gunner_Shoot");
            player.SetAutoShooting(true);
        }

        public void HandleInput(PlayerStateMachine stateMachine, Vector2 moveInput, bool jumpPressed, bool runPressed, bool shootPressed, bool autoShootHeld)
        {
            if (!autoShootHeld)
            {
                stateMachine.ChangeState(new ShootState());
            }
        }

        public void UpdateState(PlayerStateMachine stateMachine)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                stateMachine.Player.FireBullet();
                shootTimer = shootInterval;
            }
        }

        public void OnShootEndEvent(PlayerStateMachine stateMachine)
        {
            stateMachine.Player.SetAutoShooting(false);
            stateMachine.ChangeState(new IdleState());
        }
    }
}
