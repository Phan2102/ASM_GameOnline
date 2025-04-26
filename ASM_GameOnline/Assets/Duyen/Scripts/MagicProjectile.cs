using UnityEngine;
using Fusion;


public class MagicProjectile : NetworkBehaviour
{
    public float speed = 5f;
    public float damage = 10f;

    private Transform target;

    public void Init(Transform enemy)
    {
        target = enemy;
    }

    public override void FixedUpdateNetwork()
    {
        if (target == null)
        {
            Runner.Despawn(Object);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Runner.DeltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            Debug.Log("NPC Hit enemy!");
            // Gây damage cho enemy ở đây
            target.GetComponent<HealthSystem_Enemy>()?.TakeDamage(10);
            Runner.Despawn(Object);
        }
    }
}


