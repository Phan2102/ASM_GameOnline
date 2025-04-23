using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef[] enemyPrefabs;

    private NetworkObject spawnedEnemy;

    // Chỉ Server được gọi hàm này
    public void SpawnEnemy()
    {
        if (!Runner.IsServer)
        {
            Debug.LogWarning("Chỉ server mới được phép spawn!");
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("Chưa gán enemyPrefabs!");
            return;
        }

        Vector3 spawnPos = new Vector3(Random.Range(-15, -5), 0, Random.Range(-15, -5));
        NetworkPrefabRef enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        spawnedEnemy = Runner.Spawn(enemyPrefab, spawnPos, Quaternion.identity, null, (runner, obj) =>
        {
            Debug.Log("Enemy được server spawn thành công: " + obj.name);
        });
    }

    // RPC cho client yêu cầu server spawn enemy
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_AskServerToSpawnEnemy()
    {
        if (!Runner.IsServer) return;
        SpawnEnemy(); // Server thực hiện
    }

    //[SerializeField] private NetworkPrefabRef enemyPrefab;
    //[SerializeField] private List<Transform> spawnPoints;
    //[SerializeField] private int spawnCount = 5;

    //private void Start()
    //{
    //    Debug.Log("Enemy Spawner Script chạy Start()");

    //    StartCoroutine(DelayedSpawn());
    //}

    //IEnumerator DelayedSpawn()
    //{
    //    // Delay nhẹ để Fusion setup xong
    //    yield return new WaitForSeconds(1f);

    //    var runner = FindObjectOfType<NetworkRunner>();
    //    if (runner != null && runner.IsServer)
    //    {
    //        Debug.Log("Fusion đã sẵn sàng và là Server. Tiến hành Spawn!");
    //        Vector3 spawnPos = new Vector3(Random.Range(-5, 5), 0, 0);
    //        runner.Spawn(enemyPrefab, spawnPos, Quaternion.identity);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Không phải Server hoặc chưa có NetworkRunner");
    //    }
    //}

    //*****************************

    //[SerializeField] private NetworkPrefabRef enemyPrefab;
    //[SerializeField] private int numberToSpawn = 1;
    //[SerializeField] private Vector2 spawnAreaMin = new Vector2(-5, -5);
    //[SerializeField] private Vector2 spawnAreaMax = new Vector2(5, 5);

    //public override void Spawned()
    //{
    //    if (Runner.IsServer)
    //    {
    //        Debug.Log("Là Server! Spawn thôi!");
    //        for (int i = 0; i < numberToSpawn; i++)
    //        {
    //            Vector2 pos = new Vector2(
    //                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
    //                Random.Range(spawnAreaMax.y, spawnAreaMax.y)
    //            );

    //            Runner.Spawn(enemyPrefab, pos, Quaternion.identity);
    //            Debug.Log("Spawned Enemy tại: " + pos);
    //        }
    //    }
    //}

    //[SerializeField] private NetworkPrefabRef enemyPrefab;
    //[SerializeField] private Vector3 enemySpawnPosition = new Vector3(0f, 0f, 0f);

    //private bool hasSpawned = false;

    //public override void Spawned()
    //{
    //    if (!Runner.IsServer || hasSpawned) return;

    //    Runner.Spawn(enemyPrefab, enemySpawnPosition, Quaternion.identity, null);
    //    Debug.Log("EnemySpawner: Enemy spawned!");

    //    hasSpawned = true;
    //}
}
