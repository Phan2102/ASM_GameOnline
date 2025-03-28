using UnityEngine;

public class CinemachineCamera
{
    internal object transform;

    public Transform LookAt { get; internal set; }
    public Transform Follow { get; internal set; }
}