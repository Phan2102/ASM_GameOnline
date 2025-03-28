using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineVirtualCamera FollowCamera;  // Tham chiếu đến CinemachineVirtualCamera
    public float rotationSpeed = 150f;             // Tốc độ xoay camera
    private Transform playerTransform;             // Tham chiếu đến transform của người chơi
    private float verticalAngle = 0f;              // Góc xoay trục X

    private void Start()
    {
        // Ẩn con trỏ chuột khi bắt đầu
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AssignCamera(Transform target)
    {
        if (FollowCamera != null && target != null)
        {
            // Gán mục tiêu theo dõi cho camera
            FollowCamera.LookAt = target;
            FollowCamera.Follow = target;
            playerTransform = target;
        }
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            // Lấy input chuột
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            // Xoay camera quanh đối tượng theo trục Y
            playerTransform.Rotate(Vector3.up, mouseX);

            // Xoay camera quanh trục X với góc giới hạn
            verticalAngle = Mathf.Clamp(verticalAngle, -80f, 80f);

            FollowCamera.transform.localRotation = Quaternion.Euler(verticalAngle, 0f, 0f);
        }

        // Ẩn hoặc hiện con trỏ chuột khi bấm L
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}