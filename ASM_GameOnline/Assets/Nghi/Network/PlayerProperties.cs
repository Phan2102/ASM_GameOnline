using Fusion;
using TMPro;
using UnityEngine;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float currentHealth { get; set; }
    public float maxHealth { get; set; } = 100; 

    private TextMeshPro healthText; // Chỉnh từ TextMeshProUGUI -> TextMeshPro (vì dùng trong thế giới 3D/2D)

    [Networked, OnChangedRender(nameof(SyncPosition))]
    public Vector3 NetworkedPosition { get; set; }

    [Networked, OnChangedRender(nameof(SyncAnimation))]
    public int NetworkedAnimation { get; set; }

    private PlayerController playerController;
    private PlayerStateMachine stateMachine;
    public NetworkRunner networkRunner;
    public NetworkObject networkObject;

    private void Awake()
    {
        // Tìm TextMeshPro trong con của Player
        healthText = GetComponentInChildren<TextMeshPro>(true);

        // Nếu chưa có thì tự động tạo và gán
        if (healthText == null)
        {
            GameObject textObject = new GameObject("HealthText");
            textObject.transform.SetParent(transform); // Gán vào Player
            textObject.transform.localPosition = new Vector3(0, 1.5f, 0); // Vị trí trên đầu Player

            healthText = textObject.AddComponent<TextMeshPro>();
            healthText.text = "100/100"; // Giá trị mặc định
            healthText.fontSize = 3;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;

            Debug.Log("Tự động tạo và gán TextMeshPro thành công!");
        }
        else
        {
            Debug.Log("Tìm thấy TextMeshPro trong Player!");
            healthText.transform.localPosition = new Vector3(0, 1.5f, 0); // Đặt vị trí chính xác
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
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
        Initialize();
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
        Debug.Log($"Health cập nhật: {currentHealth}/{maxHealth}");
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && HasStateAuthority)
        {
            TakeDamage(10);
        }
        //if (other.gameObject.CompareTag("Enemy") && HasStateAuthority)
        //{
        //    currentHealth -= 10;

        //    if (currentHealth < -20)
        //    {
        //        networkRunner.Despawn(networkObject);
        //    }
        //}
    }

    public void TakeDamage(float damage)
    {
        if (!HasStateAuthority) return; // Chỉ chủ sở hữu mới được cập nhật

        currentHealth = Mathf.Max(currentHealth - damage, 0); // Giảm máu nhưng không âm
        Debug.Log($"Máu còn lại: {currentHealth}/{maxHealth}");

        //if (currentHealth <= 0)
        //{
        //    networkRunner.Despawn(networkObject); // Hủy Player khi chết
        //}
    }

    //[Networked, OnChangedRender(nameof(OnHealthChanged))]
    //public float currentHealth { get; set; }
    //public float maxHealth { get; set; }
    //private TextMeshProUGUI healthText;

    //[Networked, OnChangedRender(nameof(SyncPosition))]
    //public Vector3 NetworkedPosition { get; set; }

    //[Networked, OnChangedRender(nameof(SyncAnimation))]
    //public int NetworkedAnimation { get; set; }

    //private PlayerController playerController;
    //private PlayerStateMachine stateMachine;
    //public NetworkRunner networkRunner;
    //public NetworkObject networkObject;

    //private void Awake()
    //{
    //    healthText = GetComponentInChildren<TextMeshProUGUI>();
    //}

    //public override void FixedUpdateNetwork()
    //{
    //    if (HasStateAuthority)
    //    {
    //        NetworkedPosition = transform.position;
    //    }
    //    else
    //    {
    //        transform.position = Vector3.Lerp(transform.position, NetworkedPosition, Runner.DeltaTime * 10f);
    //    }
    //}

    //private void SyncPosition()
    //{
    //    transform.position = NetworkedPosition;
    //}

    //private void SyncAnimation()
    //{
    //    int animHash = NetworkedAnimation;
    //    if (animHash != 0)
    //    {
    //        GetComponent<Animator>().Play(animHash);
    //    }
    //}

    //public override void Spawned()
    //{
    //    if (HasStateAuthority)
    //    {
    //        maxHealth = 100;
    //        currentHealth = maxHealth;
    //    }
    //    healthText.text = $"{currentHealth}/{maxHealth}";
    //    Initialize(); // Gọi Initialize() để tự động gán PlayerStateMachine
    //}

    //public void Initialize()
    //{
    //    playerController = GetComponent<PlayerController>();
    //    if (playerController == null)
    //    {
    //        Debug.LogError("PlayerController chưa được tìm thấy!");
    //        return;
    //    }

    //    stateMachine = GetComponent<PlayerStateMachine>();
    //    if (stateMachine == null)
    //    {
    //        stateMachine = gameObject.AddComponent<PlayerStateMachine>(); // Tự động gán nếu chưa có
    //        Debug.Log("PlayerStateMachine đã được tự động thêm vào!");
    //    }
    //    else
    //    {
    //        Debug.Log("PlayerStateMachine đã có sẵn.");
    //    }
    //}

    //public void OnHealthChanged()
    //{
    //    Debug.Log($"Health cập nhật: {currentHealth}/{maxHealth}");
    //    healthText.text = $"{currentHealth}/{maxHealth}";
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        currentHealth -= 10;

    //        if (currentHealth < -20)
    //        {
    //            networkRunner.Despawn(networkObject);
    //        }
    //    }
    //}

    //[Networked, OnChangedRender(nameof(OnHealthChanged))]
    //public float currentHealth { get; set; }
    //public float maxHealth { get; set; }

    //[Networked, OnChangedRender(nameof(SyncPosition))]
    //public Vector3 NetworkedPosition { get; set; }

    ////[Networked, OnChangedRender(nameof(SyncRotation))]
    //public Quaternion NetworkedRotation { get; set; }

    //private CharacterController characterController;
    //private TextMeshProUGUI healthText;

    //public NetworkRunner networkRunner;
    //public NetworkObject networkObject;


    //private void Awake()
    //{
    //    characterController = GetComponent<CharacterController>();
    //    healthText = GetComponentInChildren<TextMeshProUGUI>();
    //}

    //public override void Spawned()
    //{
    //    if (HasStateAuthority)
    //    {
    //        maxHealth = 100;
    //        currentHealth = maxHealth;
    //    }
    //    healthText.text = $"{currentHealth}/{maxHealth}";
    //}

    //public void OnHealthChanged()
    //{
    //    Debug.Log($"Health cập nhật: {currentHealth}/{maxHealth}");
    //    healthText.text = $"{currentHealth}/{maxHealth}";
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Enemy"))
    //    {
    //        currentHealth -= 10;

    //        if (currentHealth < -20)
    //        {
    //            networkRunner.Despawn(networkObject);
    //        }
    //    }
    //}

    //public void SyncPosition()
    //{

    //    if (!HasStateAuthority)
    //    {
    //        // Đồng bộ vị trí từ server xuống client
    //        float pingCompensation = Mathf.Clamp(Runner.DeltaTime * 20f, 0.1f, 5f);

    //        if (Vector3.Distance(transform.position, NetworkedPosition) > 2f) // Nếu vị trí chênh lệch quá lớn -> cập nhật ngay lập tức
    //        {
    //            transform.SetPositionAndRotation(NetworkedPosition, NetworkedRotation);
    //        }
    //        else // Nếu vị trí không lệch quá xa -> dùng Lerp để đồng bộ mượt hơn
    //        {
    //            transform.position = Vector3.Lerp(transform.position, NetworkedPosition, pingCompensation);
    //            transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, pingCompensation);
    //        }

    //        //networkObject.transform.SetPositionAndRotation(NetworkedPosition, NetworkedRotation);
    //        //transform.position = Vector3.Lerp(transform.position, NetworkedPosition, Runner.DeltaTime * 15f);
    //    }
    //}

    //public void SyncRotation()
    //{
    //    if (!HasStateAuthority)
    //    {
    //        transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, Time.deltaTime * 10f);
    //    }
    //}

    //public void UpdateNetworkPosition()
    //{
    //    if (HasStateAuthority)
    //    {
    //        NetworkedPosition = transform.position;
    //        NetworkedRotation = transform.rotation;
    //        //RPC_SyncPosition(NetworkedPosition, NetworkedRotation);
    //    }
    //}

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //public void RPC_SyncPosition(Vector3 position, Quaternion rotation)
    //{
    //    if (!HasStateAuthority)
    //    {
    //        transform.position = position;
    //        transform.rotation = rotation;
    //    }
    //}


}
