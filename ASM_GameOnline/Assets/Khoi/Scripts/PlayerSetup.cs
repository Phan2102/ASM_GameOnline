using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : NetworkBehaviour
{
    public void SetupCamera()
    {
        if (!Object.HasStateAuthority) return;
        var cameraFollow = FindFirstObjectByType<CameraFollow>();
        if (cameraFollow != null) cameraFollow.AssignCamera(transform);
    }
}