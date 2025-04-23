using Fusion;
using UnityEngine;

public class FoodPickupTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        NetworkObject netObj = GetComponentInParent<NetworkObject>();
        if (netObj == null) return;

        if (!netObj.HasStateAuthority) return;

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out HealthSystem health))
            {
                health.Heal(20); // Hồi máu
            }

            // Gọi xoá object qua server
            netObj.GetComponent<FoodItemController>()?.PickedUp();
        }
    }
}
