using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // Tốc độ bay của đạn
    [SerializeField] private float lifeTime = 3f; // Đạn tự hủy sau một thời gian
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Đẩy đạn bay về phía trước
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;

        // Hủy đạn sau khi hết thời gian
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
