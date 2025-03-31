using Unity.Cinemachine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"Player joined: {player.PlayerId}, Total players: {Runner.ActivePlayers.Count()}");

        if (Runner.IsServer || Runner.LocalPlayer == player)
        {
            var position = new Vector2(-5, 0);
            Runner.Spawn(playerPrefab, position, Quaternion.identity, player, (runner, obj) =>
            {
                Debug.Log($"Spawned player for {player.PlayerId}");

                var playerSetup = obj.GetComponent<PlayerSetup>();
                if (playerSetup != null)
                {
                    playerSetup.Spawned();
                }

                var playerProperties = obj.GetComponent<PlayerProperties>();
                if (playerProperties != null)
                {
                    playerProperties.networkRunner = runner;
                    playerProperties.Initialize();
                }
            });
        }
    }

    //public void PlayerJoined(PlayerRef player)
    //{
    //    if (player == Runner.LocalPlayer)
    //    {
    //        var position = new Vector2(-5, 0);
    //        Runner.Spawn(playerPrefab, position, Quaternion.identity,
    //            Runner.LocalPlayer,
    //            (runner, obj) =>
    //            {
    //                var playerSetup = obj.GetComponent<PlayerSetup>();
    //                if (playerSetup != null)
    //                {
    //                    playerSetup.SetupCamera();
    //                }


    //                var playerProperties = obj.GetComponent<PlayerProperties>();
    //                if (playerProperties != null)
    //                {
    //                    playerProperties.networkRunner = runner;
    //                    playerProperties.Initialize(); // Gán các thành phần cần thiết
    //                }

    //                //var playerGun = obj.GetComponent<Gun>();
    //                //if (playerGun != null) playerGun.networkRunner = runner; 
    //            }
    //        );
    //    }
    //}





}
