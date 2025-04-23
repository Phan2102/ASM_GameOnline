using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;

public class HealthSystem_Enemy : NetworkBehaviour
{
    [Networked] public int Health { get; private set; }
    [SerializeField] private int maxHealth = 100;
    private TextMeshPro healthText;
    [SerializeField] private Animator animator;
    private bool isDead = false;

    public override void Spawned()
    {
        if (HasStateAuthority)
            Health = maxHealth;

        SetupHealthText();
        UpdateHealthUI();

    }

    public void TakeDamage(int dmg)
    {
        if (!HasStateAuthority || isDead) return;

        Health -= dmg;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        UpdateHealthUI();
        RPC_PlayHitAnim();

        if (Health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        animator?.SetTrigger("isDead");
        Runner.Despawn(Object);
    }

    private void SetupHealthText()
    {
        Transform healthTextTransform = transform.Find("Health Text");
        if (healthTextTransform != null)
        {
            healthText = healthTextTransform.GetComponent<TextMeshPro>();
            healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
        }
        else
        {
            GameObject textObject = new GameObject("Health Text");
            textObject.transform.SetParent(transform);
            textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
            healthText = textObject.AddComponent<TextMeshPro>();
            healthText.fontSize = 3;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText == null) return;
        healthText.text = $"{Health}/{maxHealth}";
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayHitAnim()
    {
        if (animator != null)
            animator.SetTrigger("isHurt");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int dmg)
    {
        TakeDamage(dmg);
    }

    public override void FixedUpdateNetwork()
    {
        if (healthText != null)
            healthText.text = $"{Health}/{maxHealth}";

        UpdateHealthUI(); // ← GỌI CẬP NHẬT CHUẨN MỖI FRAME
    }



    //[Networked] public int Health { get; private set; }
    //[SerializeField] private int maxHealth = 100;
    //[SerializeField] private Animator animator;

    //private TextMeshPro healthText;
    //private int lastSyncedHealth = -999; // giá trị không thể có để trigger cập nhật UI đầu tiên
    //private bool isDead = false;

    //public override void Spawned()
    //{
    //    if (HasStateAuthority)
    //    {
    //        Health = maxHealth;
    //    }

    //    SetupHealthText();
    //    UpdateHealthUI();
    //}

    //public void TakeDamage(int damage)
    //{
    //    if (!HasStateAuthority || isDead) return;

    //    Health = Mathf.Max(Health - damage, 0);
    //    Debug.Log($"🔥 Enemy trúng đòn! Còn lại {Health} máu.");

    //    if (Health == 0 && !isDead)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    isDead = true;
    //    Debug.Log("💀 Enemy chết.");
    //    animator?.SetTrigger("isDead");
    //    Runner.Despawn(Object);
    //}

    //public override void FixedUpdateNetwork()
    //{
    //    // Chỉ Client cần UI mới làm chuyện này
    //    if (!Object.HasInputAuthority && !Object.HasStateAuthority) return;

    //    // Check nếu máu thay đổi thì update UI
    //    if (Health != lastSyncedHealth)
    //    {
    //        UpdateHealthUI();
    //        PlayHitAnimation();

    //        lastSyncedHealth = Health;
    //    }
    //}

    //private void SetupHealthText()
    //{
    //    Transform t = transform.Find("Health Text");
    //    if (t != null)
    //    {
    //        healthText = t.GetComponent<TextMeshPro>();
    //    }
    //    else
    //    {
    //        GameObject textObj = new GameObject("Health Text");
    //        textObj.transform.SetParent(transform);
    //        textObj.transform.localPosition = new Vector3(0, 1f, 0);
    //        healthText = textObj.AddComponent<TextMeshPro>();
    //        healthText.fontSize = 4;
    //        healthText.color = Color.white;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //    }
    //}

    //private void UpdateHealthUI()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{Health}/{maxHealth}";
    //    }
    //}

    //private void PlayHitAnimation()
    //{
    //    if (animator != null && Health > 0)
    //    {
    //        animator.SetTrigger("isHurt");
    //    }
    //}


    //[Networked] public int Health { get; set; }
    //[SerializeField] private int maxHealth = 100;
    //private TextMeshPro healthText;
    //[SerializeField] private Animator animator;

    //public override void Spawned()
    //{
    //    // Sync initial health
    //    if (HasStateAuthority)
    //        Health = maxHealth;
    //    Debug.Log("Enemy đã được spawn bởi server!");
    //    SetupHealthText(); // Gán hoặc tạo mới
    //    UpdateHealthUI();  // Hiển thị ban đầu
    //}

    //public void TakeDamage(int dmg)
    //{
    //    if (!HasStateAuthority) return;

    //    Health -= dmg;
    //    Health = Mathf.Clamp(Health, 0, maxHealth);

    //    UpdateHealthUI(); // Gọi ở server để update UI cho local người xem

    //    RPC_PlayHitAnim();

    //    if (Health <= 0)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    Debug.Log("Enemy die.");
    //    // TODO: trigger animation die, disable attack/move, destroy object...
    //}

    //private void SetupHealthText()
    //{
    //    // 1. Tìm sẵn Text trong prefab nếu có
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        Debug.Log("Tìm thấy Health Text trong prefab!");
    //    }
    //    else
    //    {
    //        // 2. Tự tạo mới nếu không có
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.white;
    //        Debug.LogWarning("KHÔNG tìm thấy Health Text, đã tự tạo mới!");
    //    }
    //}

    //private void UpdateHealthUI()
    //{
    //    if (healthText == null) return;

    //    healthText.text = $"{Health}/{maxHealth}";
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_PlayHitAnim()
    //{
    //    if (animator != null)
    //        animator.SetTrigger("Hit");
    //}
    //????

    //[Networked] public float CurrentHealth { get; set; }
    //[SerializeField] private float MaxHealth = 100f;

    //private Animator animator;
    //private TextMeshPro healthText;

    //[SerializeField] private float startHealth = 100f;

    //private bool isInitialized = false;

    //public override void Spawned()
    //{
    //    ManualInit(); // Server + Client
    //}

    //private void Start()
    //{
    //    // Dự phòng nếu Spawned chưa gọi
    //    if (!isInitialized)
    //        StartCoroutine(WaitToInit());
    //}

    //private System.Collections.IEnumerator WaitToInit()
    //{
    //    yield return null;
    //    ManualInit();
    //}

    //private void ManualInit()
    //{
    //    if (isInitialized) return;
    //    isInitialized = true;

    //    animator = GetComponent<Animator>();

    //    if (Object.HasStateAuthority)
    //    {
    //        CurrentHealth = startHealth;
    //    }

    //    SetupHealthText();

    //    // Dù là server hay client đều gọi để show đúng máu hiện tại
    //    Rpc_UpdateHealthText(CurrentHealth, MaxHealth);
    //}

    //private void SetupHealthText()
    //{
    //    Transform healthTextTransform = transform.Find("Health Text");

    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(0, 1.5f, 0);
    //        Debug.Log("✅ Enemy: Tìm thấy Health Text!");
    //    }
    //    else
    //    {
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(0, 1.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.red;
    //        Debug.LogWarning("⚠️ Enemy: KHÔNG tìm thấy Health Text, đã tạo mới!");
    //    }
    //}

    //private void UpdateHealthText(float current, float max)
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{current}/{max}";
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void Rpc_UpdateHealthText(float current, float max)
    //{
    //    UpdateHealthText(current, max);
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void Rpc_PlayHurtAnim()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetTrigger("isHurt");
    //        Debug.Log("😵 Enemy phát animation bị đau.");
    //    }
    //}

    //public void TakeDamage(int damage, string attackerName)
    //{
    //    if (!Object.HasStateAuthority) return;

    //    CurrentHealth -= damage;
    //    Debug.Log($"💥 Enemy bị {attackerName} đánh trúng! Mất {damage} máu. Còn lại: {CurrentHealth}");

    //    if (CurrentHealth <= 0)
    //    {
    //        Die(attackerName);
    //        return;
    //    }

    //    Rpc_UpdateHealthText(CurrentHealth, MaxHealth);
    //    Rpc_PlayHurtAnim();
    //}

    //private void Die(string killer)
    //{
    //    Debug.Log($"☠️ Enemy bị {killer} giết!");
    //    // TODO: hiệu ứng chết, hủy, v.v.
    //}

    //[Networked] public float CurrentHealth { get; set; }
    //[SerializeField] private float MaxHealth { get; set; }

    //private Animator animator;
    //private TextMeshPro healthText;

    //[SerializeField] private float startHealth = 100f;

    //private bool isInitialized = false;

    //private void Start()
    //{
    //    // Nếu chưa được gọi từ Spawned() thì khởi tạo thủ công
    //    if (!isInitialized)
    //    {
    //        StartCoroutine(WaitToInit());
    //    }
    //}

    //public override void Spawned()
    //{
    //    ManualInit(); // Đảm bảo khởi tạo trong cả trường hợp được spawn
    //}

    //private IEnumerator WaitToInit()
    //{
    //    yield return null;
    //    ManualInit();
    //}

    //private void ManualInit()
    //{
    //    if (isInitialized) return;
    //    isInitialized = true;

    //    animator = GetComponent<Animator>();

    //    if (Object != null && Object.HasStateAuthority)
    //    {
    //        CurrentHealth = startHealth;
    //        MaxHealth = startHealth;
    //    }

    //    SetupHealthText();
    //    UpdateHealthText();
    //}

    //private void SetupHealthText()
    //{
    //    // Ưu tiên tìm "Health Text" trong con
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(0, 1.5f, 0); // Tuỳ vị trí
    //        Debug.Log("✅ Enemy: Tìm thấy Health Text!");
    //    }
    //    else
    //    {
    //        // Không tìm thấy thì tạo mới
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(0, 1.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.red;
    //        Debug.LogWarning("⚠️ Enemy: KHÔNG tìm thấy Health Text, đã tạo mới!");
    //    }
    //}

    //private void UpdateHealthText()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //    }
    //}

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void Rpc_UpdateHealthText(float current, float max)
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{current}/{max}";
    //    }
    //}

    //public void TakeDamage(int damage, string attackerName)
    //{
    //    if (!Object.HasStateAuthority) return;

    //    CurrentHealth -= damage;
    //    Debug.Log($"💥 Enemy bị {attackerName} đánh trúng! Mất {damage} máu. Còn lại: {CurrentHealth}");

    //    if (CurrentHealth <= 0)
    //    {
    //        Die(attackerName);
    //        return;
    //    }

    //    Rpc_UpdateHealthText(CurrentHealth, MaxHealth);
    //    Rpc_PlayHurtAnim();
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void Rpc_PlayHurtAnim()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetTrigger("isHurt");
    //        Debug.Log("😵 Enemy phát animation bị đau.");
    //    }
    //}

    //private void Die(string killer)
    //{
    //    Debug.Log($"☠️ Enemy bị {killer} giết!");
    //    // TODO: xử lý chết, hiệu ứng, v.v.
    //}

    //*************************************************

    //[Networked] public float CurrentHealth { get; set; }
    //[SerializeField] private float MaxHealth { get; set; }

    //private Animator animator;
    //private TextMeshPro healthText;

    //[SerializeField] private float startHealth = 100f;

    //public override void Spawned()
    //{
    //    animator = GetComponent<Animator>();

    //    // Gán máu mặc định nếu là server
    //    if (Object.HasStateAuthority)
    //    {
    //        CurrentHealth = startHealth;
    //        MaxHealth = startHealth;
    //    }

    //    // Auto tìm TMP_Text tên chứa "Health" trong con của Enemy
    //    healthText = GetComponentInChildren<TextMeshPro>(true);
    //    animator = GetComponent<Animator>();

    //    SetupHealthText();
    //    UpdateHealthText();
    //}

    //private void SetupHealthText()
    //{
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(0, 1.5f, 0); // Tuỳ vị trí
    //        Debug.Log("✅ Enemy: Tìm thấy Health Text!");
    //    }
    //    else
    //    {
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(0, 1.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.red;
    //        Debug.LogWarning("⚠️ Enemy: KHÔNG tìm thấy Health Text, đã tạo mới!");
    //    }
    //}

    //[Rpc(RpcSources.All, RpcTargets.All)]
    //public void Rpc_UpdateHealthText(float current, float max)
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{current}/{max}";
    //    }
    //}

    //public void TakeDamage(int damage, string attackerName)
    //{
    //    if (!Object.HasStateAuthority) return; // Chỉ xử lý ở Server

    //    CurrentHealth -= damage;
    //    Debug.Log($"💥 Enemy bị {attackerName} đánh trúng! Mất {damage} máu. Còn lại: {CurrentHealth}");

    //    if (CurrentHealth <= 0)
    //    {
    //        Die(attackerName);
    //        return;
    //    }

    //    // Update máu cho tất cả client
    //    Rpc_UpdateHealthText(CurrentHealth, MaxHealth);

    //    // Gọi animation bị đau trên mọi client
    //    Rpc_PlayHurtAnim();
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void Rpc_PlayHurtAnim()
    //{
    //    if (animator != null)
    //    {
    //        animator.SetTrigger("isHurt");
    //        Debug.Log("😵 Enemy phát animation bị đau.");
    //    }
    //}

    //private void Die(string killer)
    //{
    //    Debug.Log($"☠️ Enemy bị {killer} giết!");
    //    // Em có thể cho Enemy chết, play anim, destroy v.v.
    //}

    //private void UpdateHealthText()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //    }
    //}
    //!!!!!!!!!!!!!!!!

    //[Networked] public int Health { get; set; }
    //[SerializeField] private int maxHealth = 100;
    //private TextMeshPro healthText;
    //[SerializeField] private Animator animator;

    //public override void Spawned()
    //{
    //    // Sync initial health
    //    if (HasStateAuthority)
    //        Health = maxHealth;

    //    SetupHealthText(); // Gán hoặc tạo mới
    //    UpdateHealthUI();  // Hiển thị ban đầu
    //}

    //public void TakeDamage(int dmg)
    //{
    //    if (!HasStateAuthority) return;

    //    Health -= dmg;
    //    Health = Mathf.Clamp(Health, 0, maxHealth);

    //    UpdateHealthUI(); // Gọi ở server để update UI cho local người xem

    //    RPC_PlayHitAnim();

    //    if (Health <= 0)
    //    {
    //        Die();
    //    }
    //}

    //private void Die()
    //{
    //    Debug.Log("Enemy die.");
    //    // TODO: trigger animation die, disable attack/move, destroy object...
    //}

    //private void SetupHealthText()
    //{
    //    // 1. Tìm sẵn Text trong prefab nếu có
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        Debug.Log("Tìm thấy Health Text trong prefab!");
    //    }
    //    else
    //    {
    //        // 2. Tự tạo mới nếu không có
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.white;
    //        Debug.LogWarning("KHÔNG tìm thấy Health Text, đã tự tạo mới!");
    //    }
    //}

    //private void UpdateHealthUI()
    //{
    //    if (healthText == null) return;

    //    healthText.text = $"{Health}/{maxHealth}";
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_PlayHitAnim()
    //{
    //    if (animator != null)
    //        animator.SetTrigger("isHurt");
    //}




    //protected override void InitHealthText()
    //{
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        Debug.Log("Tìm thấy Health Text trong prefab Enemy!");
    //    }
    //    else
    //    {
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.text = $"{MaxHealth}/{MaxHealth}";
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.red;
    //        Debug.LogWarning("KHÔNG tìm thấy Health Text, đã tạo mới cho Enemy!");
    //    }
    //}
}
