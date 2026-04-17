using UnityEngine;

// Ghi chu:
// - Dieu khien vat pham thuong di chuyen theo huong duoc truyen tu spawner.
// - Khi cham player thi cong 1 diem, khi ra khoi camera thi tu huy.
public class GemMover : MonoBehaviour
{
    // Tốc độ bay của gem thường.
    public float speed = 5f;

    private Vector2 moveDirection = Vector2.down;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }
    public void Init(Vector2 dir)
    {
        // Nhận hướng di chuyển từ spawner.
        moveDirection = dir.normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Player chạm thì cộng điểm, chạm ground thì huỷ.
        if (other.CompareTag("Player"))
        {
            BackgroundMusic.PlayCollectableSfx();
            ScoreManager.instance.AddScore(1);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
