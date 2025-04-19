using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    private int direction;
    //***
    [Networked] private float Direction { get; set; }
    public void Initialize(int dir)
    {
        direction = dir;
        Direction = direction;
        Invoke(nameof(DestroyBullet), lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        //transform.position += new Vector3(direction * speed * Runner.DeltaTime, 0, 0);
        //transform.Translate(Vector2.right * direction * speed * Runner.DeltaTime);
        transform.position += transform.right * Direction * speed * Runner.DeltaTime;
    }

    private void DestroyBullet()
    {
        Runner.Despawn(Object);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Runner.Despawn(Object);
            // Enemy nhận sát thương ở đây (nếu có)

        }
    }

    //[SerializeField] private float speed = 10f; // Tốc độ bay của đạn
    //[SerializeField] private float lifeTime = 3f; // Đạn tự hủy sau một thời gian
    //[SerializeField] private int damage = 20;
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    // Đẩy đạn bay về phía trước
    //    GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;

    //    // Hủy đạn sau khi hết thời gian
    //    Destroy(gameObject, lifeTime);
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        HealthSystem enemyHealth = other.GetComponent<HealthSystem>();
    //        if (enemyHealth != null)
    //        {
    //            enemyHealth.TakeDamage(damage);
    //        }

    //        Destroy(gameObject); // Hủy viên đạn sau khi bắn trúng
    //    }
    //}
}
