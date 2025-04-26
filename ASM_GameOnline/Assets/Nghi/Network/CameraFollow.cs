using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Player để theo dõi
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }


}
