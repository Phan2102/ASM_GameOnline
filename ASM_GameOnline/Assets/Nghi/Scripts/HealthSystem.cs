using Fusion;
using System;
using TMPro;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [Networked] public int CurrentHealth { get; set; }
    public int MaxHealth = 100;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Animator animator;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            CurrentHealth = MaxHealth;
        }
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        RPC_TakeHit();
        UpdateHealthUI();

        if (CurrentHealth <= 0)
        {
            Debug.Log($"{gameObject.name} died!");
            // Gọi Animation chết ở đây nếu có
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_TakeHit()
    {
        if (animator != null)
            animator.SetTrigger("isHurt");

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{CurrentHealth}/{MaxHealth}";
        }
    }

    //[SerializeField] private int maxHealth = 100;
    //private int currentHealth;

    //public event Action<int> OnHealthChanged; // Sự kiện để UI cập nhật
    //public event Action OnDeath; // Sự kiện khi chết

    //private void Awake()
    //{
    //    currentHealth = maxHealth;
    //}

    //public void TakeDamage(int damage)
    //{
    //    if (currentHealth <= 0) return;

    //    currentHealth -= damage;
    //    Debug.Log(gameObject.name + " mất " + damage + " máu! Máu còn: " + currentHealth);
    //    currentHealth = Mathf.Max(0, currentHealth);

    //    OnHealthChanged?.Invoke(currentHealth);

    //    if (currentHealth <= 0)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    OnDeath?.Invoke();
    //    Debug.Log(gameObject.name + " đã chết!");
    //    //Destroy(gameObject); // Mặc định hủy object khi chết
    //}

    public int GetHealth() => CurrentHealth;
}
