using Fusion;
using UnityEngine;

public class NetworkPlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    public GameObject npcPrefab;
    public float npcSpawnOffset = 1.5f; // Khoảng cách spawn NPC so với player

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var spawnPos = new Vector3(Random.Range(-5f, -4f), 0, 0);

            // Spawn player
            var playerObj = Runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);

            // Spawn NPC cho player này (vị trí bên phải player)
            var npcPos = new Vector3(spawnPos.x + npcSpawnOffset, spawnPos.y, spawnPos.z);
            Runner.Spawn(npcPrefab, npcPos, Quaternion.identity, player, (runner, o) =>
            {
                // Thiết lập player mà NPC sẽ theo
                o.GetComponent<NPC>().SetTarget(playerObj.transform);
            });
        }
    }
}