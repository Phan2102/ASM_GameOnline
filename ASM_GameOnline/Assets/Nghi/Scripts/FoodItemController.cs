using Fusion;
using UnityEngine;

public class FoodItemController : NetworkBehaviour
{
    [Networked] private bool HasLanded { get; set; }

    private Rigidbody2D rb;

    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || HasLanded)
            return;

        // Kiểm tra chạm mặt đất bằng raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
        if (hit.collider != null)
        {
            LandOnGround();
        }
    }

    private void LandOnGround()
    {
        HasLanded = true;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void PickedUp()
    {
        if (!HasStateAuthority)
            return;

        Runner.Despawn(Object); // Xoá item trên toàn bộ clients
    }
}
