using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class FoodPickupTrigger : NetworkBehaviour
{
    [SerializeField] private int healAmount = 20; // Mỗi loại có thể khác nhau

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Dừng lại khi chạm đất
        if (collision.collider.CompareTag("Ground"))
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ server xử lý nhặt đồ ăn
        if (!HasStateAuthority) return;

        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null && playerHealth.Object.HasInputAuthority) // Bảo vệ
            {
                // Hồi máu cho người chơi
                playerHealth.Heal(healAmount);
                //GetComponent<HealthSystem>().Heal(20);
                //?????
                //GetComponent<PlayerProperties>().HealFromItem(30);
                //playerHealth.GetComponent<HealthSystem>().Heal(healAmount);
                // Despawn đồ ăn từ Server để đồng bộ với tất cả
                Runner.Despawn(Object);
            }
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    NetworkObject netObj = GetComponentInParent<NetworkObject>();
    //    if (netObj == null) return;

    //    if (!netObj.HasStateAuthority) return;

    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.TryGetComponent(out HealthSystem health))
    //        {
    //            health.Heal(20); // Hồi máu
    //        }

    //        // Gọi xoá object qua server
    //        netObj.GetComponent<FoodItemController>()?.PickedUp();
    //    }
    //}
}
