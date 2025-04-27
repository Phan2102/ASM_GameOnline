using Fusion;
using System;
using TMPro;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public int Health { get; set; }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Animator animator;

    private PlayerProperties playerProperties;

    private void Awake()
    {
        playerProperties = GetComponent<PlayerProperties>();
    }

    private void Start()
    {
        if (HasStateAuthority)
        {
            Health = maxHealth;

            if (playerProperties != null)
            {
                playerProperties.currentHealth = Health;
            }
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Health = maxHealth;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (!HasStateAuthority) return;

        //Health -= dmg;
        //Health = Mathf.Max(Health, 0);

        Health = Mathf.Max(Health - dmg, 0);
        Debug.Log($"Máu còn lại: {Health}/{maxHealth}");

        if (playerProperties != null)
            playerProperties.currentHealth = Health;

        RPC_PlayHitAnim();

        if (Health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        Health = Mathf.Min(Health + amount, maxHealth);

        if (playerProperties != null)
            playerProperties.currentHealth = Health;

        Debug.Log($"[Player] Healed +{amount}, current HP: {Health}");
    }

    private void Die()
    {
        Debug.Log("Player Dead");
        // TODO: Respawn hoặc GameOver
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayHitAnim()
    {
        if (animator != null)
            animator.SetTrigger("isHurt");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && HasStateAuthority)
        {
            TakeDamage(10);
        }
    }

    private void OnHealthChanged()
    {
        // Nếu có Health Text thì update text
        if (playerProperties != null)
        {
            playerProperties.UpdateHealthText(Health, maxHealth);
        }
    }

    //&&&&&&&&&&&&&&&&&&&&&&&&
    //[Networked] public int Health { get; set; }
    //[SerializeField] private int maxHealth = 100;
    //[SerializeField] private Animator animator;
    //[SerializeField] private TMP_Text healthText; // Thêm Text UI

    //private int previousHealth = -1; // Dùng để kiểm tra thay đổi

    //private void Start()
    //{
    //    if (HasInputAuthority) // Chỉ player mình tự update UI của mình
    //    {
    //        UpdateHealthUI();
    //    }
    //}

    //private void Update()
    //{
    //    // Chỉ người chơi chính mới tự update UI của mình
    //    if (!HasInputAuthority) return;

    //    // Khi Health thay đổi, cập nhật UI
    //    if (Health != previousHealth)
    //    {
    //        UpdateHealthUI();
    //        previousHealth = Health;
    //    }
    //}

    //public void TakeDamage(int dmg)
    //{
    //    if (!HasStateAuthority) return;

    //    Health -= dmg;
    //    Health = Mathf.Max(Health, 0);
    //    RPC_PlayHitAnim();

    //    if (Health <= 0)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    Debug.Log("Player Dead");
    //    // TODO: Respawn hoặc GameOver
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_PlayHitAnim()
    //{
    //    if (animator != null)
    //    {
    //        Debug.Log("Player hurt!");
    //        animator.SetTrigger("isHurt");
    //    }
    //}

    //public void Heal(int amount)
    //{
    //    if (!HasStateAuthority) return;

    //    Health = Mathf.Min(Health + amount, maxHealth);
    //    Debug.Log($"[Player] Healed +{amount}, current HP: {Health}");
    //}

    //private void UpdateHealthUI()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"HP: {Health}/{maxHealth}";
    //    }
    //}
    //!!!!!!!!!
    //[Networked] public int Health { get; set; }
    //[SerializeField] private int maxHealth = 100;
    //[SerializeField] private Animator animator;

    //private void Start()
    //{
    //    Health = maxHealth;
    //}

    //public void TakeDamage(int dmg)
    //{
    //    if (!HasStateAuthority) return;

    //    Health -= dmg;
    //    Health = Mathf.Max(Health, 0);
    //    RPC_PlayHitAnim();

    //    if (Health <= 0)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    Debug.Log("Player Dead");
    //    // TODO: Respawn hoặc GameOver
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_PlayHitAnim()
    //{
    //    if (animator != null)
    //        Debug.Log("Player hurt!");
    //        animator.SetTrigger("isHurt");
    //}

    //public void Heal(int amount)
    //{
    //    if (!HasStateAuthority) return;

    //    Health = Mathf.Min(Health + amount, maxHealth);
    //    Debug.Log($"[Player] Healed +{amount}, current HP: {Health}");
    //}
    //***************
    //[Networked] public int CurrentHealth { get; set; }
    //public int MaxHealth = 100;

    //[SerializeField] private TMP_Text healthText;
    //[SerializeField] private Animator animator;

    //public override void Spawned()
    //{
    //    if (Object.HasStateAuthority)
    //    {
    //        CurrentHealth = MaxHealth;
    //    }
    //    UpdateHealthUI();
    //}

    //public void TakeDamage(int amount)
    //{
    //    if (!Object.HasStateAuthority) return;

    //    CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
    //    RPC_TakeHit();
    //    UpdateHealthUI();

    //    if (CurrentHealth <= 0)
    //    {
    //        Debug.Log($"{gameObject.name} died!");
    //        // Gọi Animation chết ở đây nếu có
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_TakeHit()
    //{
    //    if (animator != null)
    //        animator.SetTrigger("isHurt");

    //    UpdateHealthUI();
    //}

    //private void UpdateHealthUI()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //    }
    //}

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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

    public int GetHealth() => Health;
}
