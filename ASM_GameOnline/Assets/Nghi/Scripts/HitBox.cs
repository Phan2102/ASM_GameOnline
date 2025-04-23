using Fusion;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public enum HitBoxType { Enemy, Player }
    public HitBoxType type;
    public NetworkObject owner; // Ai sở hữu cái HitBox này

    private void Awake()
    {
        owner = GetComponentInParent<NetworkObject>();
    }

    //[SerializeField] private int damage = 10;
    //[SerializeField] private string ownerTag; // "Player" hoặc "Enemy"
    //[SerializeField] private LayerMask targetLayer;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (((1 << other.gameObject.layer) & targetLayer) == 0)
    //        return;

    //    if (other.transform.root.CompareTag(ownerTag)) return; // Tránh tự đánh chính mình

    //    var health = other.GetComponentInParent<Health_Base>();
    //    if (health != null)
    //    {
    //        Debug.Log($"{ownerTag} đánh trúng {other.transform.root.name}");
    //        health.TakeDamage(damage, ownerTag);
    //    }
    //}
    //*************************************************

    //[SerializeField] private int damage = 10;
    //[SerializeField] private LayerMask targetLayer;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (((1 << other.gameObject.layer) & targetLayer) != 0)
    //    {
    //        var health = other.GetComponentInParent<HealthSystem>();
    //        if (health != null)
    //        {
    //            health.TakeDamage(damage);
    //        }
    //    }
    //}








    //[SerializeField] private Collider2D attackHitbox;

    //public void EnableHitbox()
    //{
    //    attackHitbox.enabled = true;
    //}

    //public void DisableHitbox()
    //{
    //    attackHitbox.enabled = false;
    //}
}
