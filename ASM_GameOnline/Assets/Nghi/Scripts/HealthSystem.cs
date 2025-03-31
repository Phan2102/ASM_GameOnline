using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public event Action<int> OnHealthChanged; // Sự kiện để UI cập nhật
    public event Action OnDeath; // Sự kiện khi chết

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " mất " + damage + " máu! Máu còn: " + currentHealth);
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log(gameObject.name + " đã chết!");
        //Destroy(gameObject); // Mặc định hủy object khi chết
    }

    public int GetHealth() => currentHealth;
}
