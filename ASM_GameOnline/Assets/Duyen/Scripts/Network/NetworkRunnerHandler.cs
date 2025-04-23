using Fusion;
using UnityEngine;

public class NetworkRunnerHandler : MonoBehaviour
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = GetComponent<NetworkRunner>();
        if (runner == null)
            runner = gameObject.AddComponent<NetworkRunner>();

        runner.ProvideInput = true;
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "PlatformerRoom",
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

   
}
