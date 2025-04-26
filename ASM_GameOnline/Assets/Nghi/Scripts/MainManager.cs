using Fusion.Sockets;
using Fusion;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MainManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner _runner;
    public NetworkSceneManagerDefault _sceneManager;

    public NetworkPrefabRef _malePlayerPrefabs;
    public NetworkPrefabRef _femalePlayerPrefab;

    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private NetworkObject spawnedEnemy;
    [SerializeField] private NetworkPrefabRef[] enemyPrefabs;
    public void SpawnEnemy()
    {
        //if (_runner == null || !_runner.IsRunning || !_runner.IsServer)
        if (_runner == null || !_runner.IsRunning || !_runner.IsSharedModeMasterClient)

        {
            Debug.LogWarning("Runner not ready yet, cannot spawn enemy.");
            Debug.Log($"_runner: {_runner}, IsRunning: {_runner?.IsRunning}, IsServer: {_runner?.IsServer}");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("enemyPrefabs is null or empty!");
            return;
        }

        var enemyPrefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
        var spawnPos = new Vector3(UnityEngine.Random.Range(-6, 6), -2, 0);
        
        spawnedEnemy = _runner.Spawn(
            enemyPrefab,
            spawnPos,
            Quaternion.identity,
            null,
            (runner, obj) =>
            {
                Debug.Log("Enemy Spawned: " + obj.name);
            }
        );

        
    }
    //Khởi tạo các biến
    private void Awake()
    {
        if (_runner == null)
        {
            GameObject runnerObj = new GameObject("NetworkRunner");
            _runner = runnerObj.AddComponent<NetworkRunner>();
            _runner.AddCallbacks(this);
            _sceneManager = runnerObj.AddComponent<NetworkSceneManagerDefault>();
        }

        ConnectToFusion();
    }

    async void ConnectToFusion()
    {
        Debug.Log("Connecting to Fusion Network...");
        _runner.ProvideInput = true;//Cho phép người chơi nhập Input 
        string sessionName = "MyGameSession";//Tên phiên

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,//Chế độ Shared Mode
            SceneManager = _sceneManager,
            SessionName = sessionName,
            PlayerCount = 5,//Số lượng người chơi tối đa
            IsVisible = true,//Có hiển thị phiên hay không
            IsOpen = true,//Có cho phép người chơi khác tham gia hay không
        };

        //Kết nối đến Fusion Network
        var result = await _runner.StartGame(startGameArgs);
        if (result.Ok)
        {
            Debug.Log("Connect to Fusion Network successfully!");
        }
        else
        {
            Debug.LogError($"Failed to connect to Fusion Network: {result.ShutdownReason}");
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCharacter), 5, 5);
        InvokeRepeating(nameof(SpawnEnemy), 20f, 20f); // Enemy sẽ spawn sau 7s và cách nhau 15s
    }

    public NetworkPrefabRef[] characterPrefabRefs;
    private NetworkObject spawnCharacter;
    public void SpawnCharacter()
    {
        if (_runner == null || !_runner.IsRunning)
        {
            Debug.LogWarning("Runner not ready yet, cannot spawn.");
            return;
        }

        if (characterPrefabRefs == null || characterPrefabRefs.Length == 0)
        {
            Debug.LogError("characterPrefabRefs is null or empty!");
            return;
        }

        var characterPrefab = characterPrefabRefs[UnityEngine.Random.Range(0, characterPrefabRefs.Length)];
        var position = new Vector3(UnityEngine.Random.Range(-5, 10), 0, 0);
        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        spawnCharacter = _runner.Spawn(
            characterPrefab,
            position,
            rotation,
            null,
            (r, o) =>
            {
                Debug.Log("Character Spawned: " + o);
            }
            );

        Invoke(nameof(DespawnCharacter), 5f);
    }

    public void DespawnCharacter()
    {
        if (spawnCharacter != null)
        {
            _runner.Despawn(spawnCharacter);
        }
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    //Hàm này được gọi sau khi kết nối mạng thành công
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("...Player joined: " + player);
        if (_runner.LocalPlayer != player) return;
        //Xử lý Spawn nhân vật
        Debug.Log("OnPlayerJoined: " + player.PlayerId);
        //Lấy thông tin người chơi từ PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName");
        string playerClass = PlayerPrefs.GetString("PlayerClass");
        //Tạo người chơi
        var prefab = playerClass.Equals("Male") ? _malePlayerPrefabs : _femalePlayerPrefab;
        //Spawn nguoi choi
        var playerObj = runner.Spawn(
            prefab,
            Vector3.zero,
            Quaternion.identity,
            player,
            (r, o) =>
            {
                Debug.Log("Player spawned: " + o.Id);

                //KHỞI TẠO NETWORK TRONG GUN
                
            });





    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
}
