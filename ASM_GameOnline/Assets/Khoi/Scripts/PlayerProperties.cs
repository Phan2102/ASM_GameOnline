using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float currentHealth { get; set; }
    public float maxHealth { get; set; }

    public TextMeshProUGUI textHealth;
    public NetworkObject networkObject;
    public NetworkRunner networkRunner;


    public void OnHealthChanged()
    {

        textHealth.text = $"{currentHealth}/{maxHealth}";

    }

    [Networked, OnChangedRender(nameof(OnSpeedChanged))]

    public float speed { get; set; }
    public Animator animator;
    public int speedHash = Animator.StringToHash("Speed");
    public void OnSpeedChanged()
    {
        animator.SetFloat(speedHash, speed);
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            currentHealth -= 10;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth <= 0)
            {
                //destroy(GameObject) ko xai
                networkRunner.Despawn(networkObject);
            }
        }
    }
    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        textHealth.text = $"{currentHealth}/{maxHealth}";
    }


}