using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float currentHealth { get; set; }
    public float maxHealth { get; set; } = 100;

    [SerializeField] private Animator anim;

    [Networked, OnChangedRender(nameof(SyncPosition))]
    public Vector3 NetworkedPosition { get; set; }
    [Networked, OnChangedRender(nameof(SyncAnimation))]
    public int NetworkedAnimation { get; set; }

    [Networked, OnChangedRender(nameof(OnDeathChanged))]
    public NetworkBool IsDead { get; set; }

    [Networked, OnChangedRender(nameof(OnReviveChanged))]
    public NetworkBool IsRevived { get; set; }


    [SerializeField] private TextMeshProUGUI nameText; 

    [Header("UI")]
    [SerializeField] private Image healthBarFill;

    private PlayerController playerController;
    private PlayerStateMachine stateMachine;
    public NetworkRunner networkRunner;
    public NetworkObject networkObject;

    public string Name; // Tên người chơi (thường copy từ PlayerPrefs)

    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public string NetworkedName { get; set; }

    private Vector3 deathPosition;

    private void Start()
    {
        // Nếu đã có tên mạng thì cập nhật luôn
        if (!string.IsNullOrEmpty(NetworkedName) && nameText != null)
        {
            nameText.text = NetworkedName;
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            NetworkedPosition = transform.position;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, NetworkedPosition, Runner.DeltaTime * 10f);
        }

    }

    private void SyncPosition()
    {
        transform.position = NetworkedPosition;
    }

    private void SyncAnimation()
    {
        int animHash = NetworkedAnimation;
        if (animHash != 0)
        {
            GetComponent<Animator>().Play(animHash);
        }
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            maxHealth = 100;
            currentHealth = maxHealth;
        }

        Initialize();

        // Gán tên: Nếu client có quyền nhập (InputAuthority)
        if (Object.HasInputAuthority)
        {
            Name = PlayerPrefs.GetString("PlayerName");
            RPC_SetName(Name); // Gọi RPC đồng bộ tên cho các client

            CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
            if (camFollow != null)
            {
                camFollow.SetTarget(transform); // Gán camera follow Player
            }

        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetName(string newName)
    {
        NetworkedName = newName;
    }

    public void OnNameChanged()
    {

        if (nameText != null)
        {
            nameText.text = NetworkedName;
        }
    }

    public void Initialize()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController chưa được tìm thấy!");
            return;
        }

        stateMachine = GetComponent<PlayerStateMachine>();
        if (stateMachine == null)
        {
            stateMachine = gameObject.AddComponent<PlayerStateMachine>();
            Debug.Log("PlayerStateMachine đã được tự động thêm vào!");
        }
        else
        {
            Debug.Log("PlayerStateMachine đã có sẵn.");
        }
    }

    public void OnHealthChanged()
    {
        if (healthBarFill != null)
        {
            float fillAmount = currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;
        }

    }

    public void Heal(float amount)
    {
        if (!HasStateAuthority || currentHealth == 100 || currentHealth <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // không vượt quá max

        Debug.Log("Player hồi máu: " + amount);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && HasStateAuthority)
        {
            TakeDamage(3);
        }

    }

    public void TakeDamage(float damage)
    {
        if (!HasStateAuthority) return; // Chỉ chủ sở hữu mới được cập nhật

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            IsDead = true;
            IsRevived = false;
            deathPosition = transform.position; 

            StartCoroutine(ReviveAfterDelay(3f));
        }
    }

    public void Respawn()
    {
        if (!HasStateAuthority) return;

        currentHealth = maxHealth;
        IsDead = false;
        IsRevived = true;

        // Teleport lại vị trí spawn nếu muốn
        transform.position = deathPosition; // hoặc vị trí spawn nào đó
    }
    private void OnDeathChanged()
    {
        if (IsDead)
        {
            if (anim != null)
            {
                anim.SetTrigger("isHurt");
            }

            // Vô hiệu hóa input, movement,...
            GetComponent<PlayerController>().enabled = false;
        }
    }

    private void OnReviveChanged()
    {
        if (IsRevived)
        {
            if (anim != null)
            {
                anim.SetTrigger("isRevive");
            }

            // Bật lại điều khiển
            GetComponent<PlayerController>().enabled = true;

        }
    }
    IEnumerator ReviveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

}
