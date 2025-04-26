    using Fusion;
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Cinemachine;
    using UnityEngine;

    public class PlayerSetup : NetworkBehaviour
    {
    //public void SetupCamera()
    //{
    //    if (!Object.HasStateAuthority) return;
    //    var cameraFollow = FindFirstObjectByType<CameraFollow>();
    //    if (cameraFollow != null) cameraFollow.AssignCamera(transform);
    //}

    //Setup HP, Score,...
    public override void Spawned()
    {
        /*if (!Object.HasStateAuthority) return;

        // Tìm CameraFollow trong Scene
        var cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.AssignCamera(transform);
        }
        else
        {
            Debug.LogError("Không tìm thấy CameraFollow trong Scene!");
        }*/
    }
}
