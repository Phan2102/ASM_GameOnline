using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private Collider2D attackHitbox;

    public void EnableHitbox()
    {
        attackHitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        attackHitbox.enabled = false;
    }
}
