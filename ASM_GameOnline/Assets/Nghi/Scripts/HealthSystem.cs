using Fusion;
using System;
using TMPro;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    [Networked] public int Health { get; set; }
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Animator animator;

    private void Start()
    {
        Health = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        if (!HasStateAuthority) return;

        Health -= dmg;
        Health = Mathf.Max(Health, 0);
        RPC_PlayHitAnim();

        if (Health <= 0)
        {
            Die();
        }
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
            Debug.Log("Player hurt!");
            animator.SetTrigger("isHurt");
    }

    public void Heal(int amount)
    {
        if (!HasStateAuthority) return;

        Health += amount;
        Debug.Log("Healed! Current HP: " + Health);
    }

    
    public int GetHealth() => Health;
}
