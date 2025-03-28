using Fusion;
using UnityEngine;
using TMPro; // Thêm namespace cho TextMeshPro (nếu bạn dùng UI)

public class PlayerMovement2D : NetworkBehaviour
{
    public Rigidbody2D rb; // Điều khiển nhân vật 2D
    public float speed = 5f;
    public float jumpForce = 7f; // Lực nhảy
    public float gravityScale = 3f; // Tỷ lệ trọng lực (điều chỉnh trong Inspector)
    private float yVelocity; // Vận tốc rơi (không cần thiết khi dùng Rigidbody2D)
    public Transform groundCheck; // Điểm kiểm tra tiếp đất
    public float groundRadius = 0.15f; // Bán kính kiểm tra tiếp đất
    public LayerMask whatIsGround; // Lớp mặt đất
    private bool isGrounded;

    [Header("Interaction")]
    public float interactionRadius = 1.5f; // Bán kính tương tác với NPC
    public KeyCode interactionKey = KeyCode.E; // Phím để tương tác
    private INPC currentNPC; // NPC hiện tại mà người chơi đang ở trong phạm vi

    [Header("UI")]
    public GameObject dialoguePanel; // Panel chứa UI hiển thị hội thoại
    public TextMeshProUGUI dialogueText; // Text hiển thị lời thoại
    private bool isTalking = false;

    public override void FixedUpdateNetwork()
    {
        // Kiểm tra quyền điều khiển nhân vật
        if (!Object.HasStateAuthority) return;

        // Kiểm tra xem nhân vật có đang đứng trên mặt đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        // Nhận input từ bàn phím
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Di chuyển ngang
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Tính toán vận tốc để cập nhật Animator (nếu bạn có PlayerProperties cho 2D)
        PlayerProperties playerProperties = GetComponent<PlayerProperties>();
        if (playerProperties != null)
        {
            playerProperties.speed = Mathf.Abs(moveInput); // Độ lớn của input ngang
        }

        // Xử lý nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Xử lý tương tác với NPC
        HandleNPCInteraction();
    }

    private void HandleNPCInteraction()
    {
        // Nếu đang nói chuyện, chỉ cho phép kết thúc hội thoại
        if (isTalking)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                EndDialogue();
            }
            return;
        }

        // Tìm NPC trong phạm vi tương tác
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        INPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            INPC npc = hitCollider.GetComponent<INPC>();
            if (npc != null)
            {
                float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestNPC = npc;
                    closestDistance = distance;
                }
            }
        }

        // Nếu tìm thấy NPC trong phạm vi và người chơi nhấn nút tương tác
        if (closestNPC != null && Input.GetKeyDown(interactionKey))
        {
            StartDialogue(closestNPC);
        }

        // Cập nhật NPC hiện tại (có thể dùng để hiển thị gợi ý tương tác)
        currentNPC = closestNPC;
        //Debug.Log(currentNPC);
    }

    private void StartDialogue(INPC npc)
    {
        isTalking = true;
        currentNPC = npc;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        if (dialogueText != null)
        {
            dialogueText.text = currentNPC.GetDialogue();
        }
        // Có thể thêm logic khác khi bắt đầu hội thoại, ví dụ như vô hiệu hóa di chuyển
        // (Bạn có thể đặt rb.velocity = Vector2.zero; để dừng di chuyển)
    }

    private void EndDialogue()
    {
        isTalking = false;
        currentNPC = null;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
        // Có thể thêm logic khác khi kết thúc hội thoại, ví dụ như kích hoạt lại di chuyển
    }

    // Vẽ gizmos để dễ dàng quan sát phạm vi tương tác trong Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}