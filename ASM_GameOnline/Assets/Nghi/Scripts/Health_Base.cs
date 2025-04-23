using Fusion;
using TMPro;
using UnityEngine;

public class Health_Base : NetworkBehaviour
{
    //[Networked] public int CurrentHealth { get; set; }
    //public int MaxHealth = 100;

    //protected TMP_Text healthText;
    //[SerializeField] protected Animator animator;

    //public override void Spawned()
    //{
    //    if (Object.HasStateAuthority)
    //    {
    //        CurrentHealth = MaxHealth;
    //    }
    //    InitHealthText();
    //    UpdateHealthUI();
    //}

    //public void TakeDamage(int amount, string attackerTag)
    //{
    //    if (!Object.HasStateAuthority) return;

    //    CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
    //    Debug.Log($"{gameObject.name} bị {attackerTag} đánh, mất {amount} máu. Còn lại: {CurrentHealth}");

    //    RPC_TakeHit();

    //    UpdateHealthUI();

    //    if (CurrentHealth <= 0)
    //    {
    //        Debug.Log($"{gameObject.name} đã chết.");
    //        // Gọi animation chết nếu cần
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //private void RPC_TakeHit()
    //{
    //    if (animator != null)
    //        animator.SetTrigger("isHurt");

    //    UpdateHealthUI();
    //}

    //protected abstract void InitHealthText();

    //protected void UpdateHealthUI()
    //{
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{CurrentHealth}/{MaxHealth}";
    //    }
    //}
}
