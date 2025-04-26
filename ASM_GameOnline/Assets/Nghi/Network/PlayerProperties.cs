using Fusion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float currentHealth { get; set; }
    public float maxHealth { get; set; } = 100;

    private TextMeshPro healthText;
    private TextMeshPro nameText;

    [Networked, OnChangedRender(nameof(SyncPosition))]
    public Vector3 NetworkedPosition { get; set; }

    [Networked, OnChangedRender(nameof(SyncAnimation))]
    public int NetworkedAnimation { get; set; }

    public NetworkRunner networkRunner;
    public NetworkObject networkObject;

    public string Name;
    public CinemachineVirtualCamera FollowCamera;

    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public string NetworkedName { get; set; }

    private PlayerController playerController;
    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        FollowCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

        // Health Text
        Transform healthTextTransform = transform.Find("Health Text");
        if (healthTextTransform != null)
        {
            healthText = healthTextTransform.GetComponent<TextMeshPro>();
        }
        else
        {
            GameObject textObject = new GameObject("Health Text");
            textObject.transform.SetParent(transform);
            textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
            healthText = textObject.AddComponent<TextMeshPro>();
            healthText.text = "100/100";
            healthText.fontSize = 3;
            healthText.alignment = TextAlignmentOptions.Center;
            healthText.color = Color.white;
        }

        // Name Text
        Transform nameTextTransform = transform.Find("Name Text");
        if (nameTextTransform != null)
        {
            nameText = nameTextTransform.GetComponent<TextMeshPro>();
        }
        else
        {
            GameObject nameTextObject = new GameObject("Name Text");
            nameTextObject.transform.SetParent(transform);
            nameTextObject.transform.localPosition = new Vector3(-0.3f, 1f, 0);
            nameText = nameTextObject.AddComponent<TextMeshPro>();
            nameText.text = "Player";
            nameText.fontSize = 3;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;
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
            healthText.text = $"{currentHealth}/{maxHealth}";

        Initialize();

        if (Object.HasInputAuthority)
        {
            Name = PlayerPrefs.GetString("PlayerName");
            RPC_SetName(Name);

            if (nameText != null)
                nameText.text = NetworkedName;
        }

        if (Object.HasInputAuthority && FollowCamera != null)
        {
            FollowCamera.Follow = transform;
            FollowCamera.LookAt = transform;
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

        if (FollowCamera != null)
        {
            nameText.transform.LookAt(FollowCamera.transform);
            healthText.transform.LookAt(FollowCamera.transform);
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

    public void OnHealthChanged()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
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
        }
    }

    // Nếu có logic nhặt đồ ăn thì gọi cái này:
    public void HealFromItem(int amount)
    {
        if (!HasStateAuthority) return;

        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.Heal(amount);
        }
    }

    // Nếu Enemy gây damage thì gọi cái này:
    public void TakeDamage(float dmg)
    {
        if (!HasStateAuthority) return;

        HealthSystem healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage((int)dmg);
        }
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //[Networked, OnChangedRender(nameof(OnHealthChanged))]
    //public float currentHealth { get; set; }
    //public float maxHealth { get; set; } = 100;

    //// Đảm bảo  có 2 GameObject con với tên chính xác ("Health Text" và "Name Text")
    //private TextMeshPro healthText; // Dùng cho hiển thị HP (ở dưới)
    //private TextMeshPro nameText;   // Dùng cho hiển thị tên (ở trên)

    //[Networked, OnChangedRender(nameof(SyncPosition))]
    //public Vector3 NetworkedPosition { get; set; }
    //[Networked, OnChangedRender(nameof(SyncAnimation))]
    //public int NetworkedAnimation { get; set; }

    //private PlayerController playerController;
    //private PlayerStateMachine stateMachine;
    //public NetworkRunner networkRunner;
    //public NetworkObject networkObject;

    //public string Name; // Tên người chơi (thường copy từ PlayerPrefs)
    //public CinemachineVirtualCamera FollowCamera; // Dùng Cinemachine Virtual Camera

    //[Networked, OnChangedRender(nameof(OnNameChanged))]
    //public string NetworkedName { get; set; }

    //private void Awake()
    //{
    //    // Tìm Camera theo Cinemachine
    //    FollowCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

    //    // Tự động gán Health Text: Tìm theo tên GameObject "Health Text" nằm trong con của Player
    //    Transform healthTextTransform = transform.Find("Health Text");
    //    if (healthTextTransform != null)
    //    {
    //        healthText = healthTextTransform.GetComponent<TextMeshPro>();
    //        // Đặt lại vị trí cho Health Text (ở dưới)
    //        healthText.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        Debug.Log("Tìm thấy Health Text trong prefab!");
    //    }
    //    else
    //    {
    //        // Nếu không tìm thấy, tự tạo mới
    //        GameObject textObject = new GameObject("Health Text");
    //        textObject.transform.SetParent(transform);
    //        textObject.transform.localPosition = new Vector3(-0.3f, 0.5f, 0);
    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.text = "100/100";
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.white;
    //        Debug.LogWarning("KHÔNG tìm thấy Health Text, đã tự tạo mới!");
    //    }

    //    // Tự động gán Name Text: Tìm theo tên GameObject "Name Text" nằm trong con của Player
    //    Transform nameTextTransform = transform.Find("Name Text");
    //    if (nameTextTransform != null)
    //    {
    //        nameText = nameTextTransform.GetComponent<TextMeshPro>();
    //        // Đặt vị trí cho Name Text (ở trên)
    //        nameText.transform.localPosition = new Vector3(-0.3f, 1f, 0);
    //        Debug.Log("Tìm thấy Name Text trong prefab!");
    //    }
    //    else
    //    {
    //        // Nếu không tìm thấy, tự tạo mới
    //        GameObject nameTextObject = new GameObject("Name Text");
    //        nameTextObject.transform.SetParent(transform);
    //        nameTextObject.transform.localPosition = new Vector3(-0.3f, 1f, 0);
    //        nameText = nameTextObject.AddComponent<TextMeshPro>();
    //        nameText.text = "Player";
    //        nameText.fontSize = 3;
    //        nameText.alignment = TextAlignmentOptions.Center;
    //        nameText.color = Color.white;
    //        Debug.LogWarning("KHÔNG tìm thấy Name Text, đã tự tạo mới!");
    //    }
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

    //    if (FollowCamera != null)
    //    {
    //        nameText.transform.LookAt(FollowCamera.transform);
    //        healthText.transform.LookAt(FollowCamera.transform);
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
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{currentHealth}/{maxHealth}";
    //    }
    //    Initialize();

    //    // Gán tên: Nếu client có quyền nhập (InputAuthority)
    //    if (Object.HasInputAuthority)
    //    {
    //        Name = PlayerPrefs.GetString("PlayerName");
    //        RPC_SetName(Name); // Gọi RPC đồng bộ tên cho các client

    //        if (nameText != null)
    //        {
    //            nameText.text = NetworkedName;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("nameText bị null! Không thể hiển thị tên.");
    //        }
    //    }

    //    // Nếu có InputAuthority, set Camera theo Player
    //    if (Object.HasInputAuthority && FollowCamera != null)
    //    {
    //        FollowCamera.Follow = transform;
    //        FollowCamera.LookAt = transform;
    //    }
    //}

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_SetName(string newName)
    //{
    //    NetworkedName = newName;
    //}

    //public void OnNameChanged()
    //{
    //    if (nameText != null)
    //    {
    //        nameText.text = NetworkedName;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Không tìm thấy nameText để update tên!");
    //    }
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
    //        stateMachine = gameObject.AddComponent<PlayerStateMachine>();
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
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{currentHealth}/{maxHealth}";
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Enemy") && HasStateAuthority)
    //    {
    //        TakeDamage(10);
    //    }
    //}

    //public void TakeDamage(float damage)
    //{
    //    if (!HasStateAuthority) return; // Chỉ chủ sở hữu mới được cập nhật

    //    currentHealth = Mathf.Max(currentHealth - damage, 0);
    //    Debug.Log($"Máu còn lại: {currentHealth}/{maxHealth}");
    //}
    //[Networked, OnChangedRender(nameof(OnHealthChanged))]
    //public float currentHealth { get; set; }
    //public float maxHealth { get; set; } = 100; 

    //private TextMeshPro healthText; // Chỉnh từ TextMeshProUGUI -> TextMeshPro (vì dùng trong thế giới 3D/2D)

    //[Networked, OnChangedRender(nameof(SyncPosition))]
    //public Vector3 NetworkedPosition { get; set; }

    //[Networked, OnChangedRender(nameof(SyncAnimation))]
    //public int NetworkedAnimation { get; set; }

    //private PlayerController playerController;
    //private PlayerStateMachine stateMachine;
    //public NetworkRunner networkRunner;
    //public NetworkObject networkObject;


    //public string Name;
    //public TextMeshPro nameText;
    //public CinemachineCamera FollowCamera;

    //[Networked, OnChangedRender(nameof(OnNameChanged))]
    //public string NetworkedName { get; set; }

    //private void Awake()
    //{
    //    FollowCamera = FindFirstObjectByType<CinemachineCamera>();
    //    // Tìm TextMeshPro trong con của Player
    //    healthText = GetComponentInChildren<TextMeshPro>(true);

    //    // Nếu chưa có thì tự động tạo và gán
    //    if (healthText == null)
    //    {
    //        GameObject textObject = new GameObject("HealthText");
    //        textObject.transform.SetParent(transform); // Gán vào Player
    //        textObject.transform.localPosition = new Vector3(-0.5f, 1f, 0); // Vị trí trên đầu Player

    //        healthText = textObject.AddComponent<TextMeshPro>();
    //        healthText.text = "100/100"; // Giá trị mặc định
    //        healthText.fontSize = 3;
    //        healthText.alignment = TextAlignmentOptions.Center;
    //        healthText.color = Color.white;

    //        Debug.Log("Tự động tạo và gán TextMeshPro thành công!");
    //    }
    //    else
    //    {
    //        Debug.Log("Tìm thấy TextMeshPro trong Player!");
    //        healthText.transform.localPosition = new Vector3(-0.5f, 1.5f, 0); // Đặt vị trí chính xác
    //    }

    //    // ✅ GÁN nameText (nếu có sẵn trong con)
    //    if (nameText == null)
    //    {
    //        nameText = GameObject.Find("Name Text").GetComponent<TextMeshPro>();
    //        if (nameText != null)
    //        {
    //            Debug.Log("Tìm thấy nameText!");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("KHÔNG tìm thấy nameText. Bạn cần tạo sẵn Text hiển thị tên trong Prefab Player!");
    //        }
    //    }
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
    //    if (healthText != null)
    //    { 
    //        healthText.text = $"{currentHealth}/{maxHealth}";
    //    }
    //    Initialize();

    //    // GÁN TÊN
    //    if (Object.HasInputAuthority)
    //    {
    //        Name = PlayerPrefs.GetString("PlayerName");
    //        RPC_SetName(Name); // Gọi RPC để gán tên cho tất cả client
    //        if (nameText != null)
    //        {
    //            nameText.text = Name;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("nameText bị null! Không thể hiển thị tên.");
    //        }
    //    }

    //    if (Object.HasInputAuthority && FollowCamera != null)
    //    {
    //        FollowCamera.Follow = transform;
    //        FollowCamera.LookAt = transform;
    //    }
    //}

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_SetName(string newName)
    //{
    //    NetworkedName = newName;
    //}

    //public void OnNameChanged()
    //{
    //    if (nameText != null)
    //    {
    //        nameText.text = NetworkedName;
    //    }
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
    //        stateMachine = gameObject.AddComponent<PlayerStateMachine>();
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
    //    if (healthText != null)
    //    {
    //        healthText.text = $"{currentHealth}/{maxHealth}";
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Enemy") && HasStateAuthority)
    //    {
    //        TakeDamage(10);
    //    }
    //    //if (other.gameObject.CompareTag("Enemy") && HasStateAuthority)
    //    //{
    //    //    currentHealth -= 10;

    //    //    if (currentHealth < -20)
    //    //    {
    //    //        networkRunner.Despawn(networkObject);
    //    //    }
    //    //}
    //}

    //public void TakeDamage(float damage)
    //{
    //    if (!HasStateAuthority) return; // Chỉ chủ sở hữu mới được cập nhật

    //    currentHealth = Mathf.Max(currentHealth - damage, 0); // Giảm máu nhưng không âm
    //    Debug.Log($"Máu còn lại: {currentHealth}/{maxHealth}");

    //    //if (currentHealth <= 0)
    //    //{
    //    //    networkRunner.Despawn(networkObject); // Hủy Player khi chết
    //    //}
    //}
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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
