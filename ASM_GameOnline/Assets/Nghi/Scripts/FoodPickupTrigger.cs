using Fusion;
using UnityEngine;

public class FoodPickupTrigger : MonoBehaviour
{
    public float healAmount = 25f; // lượng máu hồi

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra va chạm với Player
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerProperties>();
            if (health != null)
            {
                health.Heal(healAmount);
                Destroy(gameObject); // Item biến mất sau khi dùng
            }
        }
    }
}
