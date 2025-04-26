using Fusion;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef npcPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(1.5f, 0, 0);

    private NetworkObject spawnedNPC;

    public void SpawnNPCForPlayer(NetworkObject playerObject)
    {
        if (npcPrefab == null || playerObject == null)
        {
            Debug.LogError("NPC Prefab hoặc Player Object null!");
            return;
        }

        Vector3 spawnPosition = playerObject.transform.position + spawnOffset;

        spawnedNPC = Runner.Spawn(
            npcPrefab,
            spawnPosition,
            Quaternion.identity,
            inputAuthority: null, // NPC không cần input control
            onBeforeSpawned: (runner, obj) =>
            {
                // Gán Target cho NPC follow player
                NPC npcScript = obj.GetComponent<NPC>();
                if (npcScript != null)
                {
                    npcScript.SetTarget(playerObject.transform);
                }
            }
        );
    }
}
