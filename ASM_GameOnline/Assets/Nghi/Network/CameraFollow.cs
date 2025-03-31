using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = FindObjectOfType<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Không tìm thấy CinemachineVirtualCamera trong Scene!");
        }
    }

    public void AssignCamera(Transform playerTransform)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
    }
    //public CinemachineVirtualCamera virtualCamera;
    //public void AssignCamera(Transform playerTransform)
    //{
    //    virtualCamera.Follow = playerTransform;
    //    virtualCamera.LookAt = playerTransform;
    //}
}
