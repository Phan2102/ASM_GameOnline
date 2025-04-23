using UnityEngine;

public class autoMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển trái/phải
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        // Đổi hướng nếu vượt quá khoảng cách cho phép
        if (Vector3.Distance(startPosition, transform.position) >= moveDistance)
        {
            direction *= -1; // Đổi hướng
        }
    }

}
