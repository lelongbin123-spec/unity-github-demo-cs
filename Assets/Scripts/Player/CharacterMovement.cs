using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - Dieu khien di chuyen trai/phai, nhay doi, quay huong nhan vat va buff speed.
// - Luu y: logic hien tai chi set velocity khi co input, nen can xem lai de player dung han khi tha phim.
public class CharacterMovement : MonoBehaviour
{

    // Tốc độ di chuyển và lực nhảy cơ bản.
    public float speed = 5.0f;
    public float jumpForce = 8f;

    private int jumpCount = 0;
    public int maxJump = 2; // double jump = 2 lần

    private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private float originalSpeed;
    private Coroutine speedCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // Lấy component cần thiết khi bắt đầu scene.
        originalSpeed = speed;
        animator = GetComponent<Animator>(); //bắt đầu animation khép mở chân
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Đọc input và điều khiển chuyển động theo từng frame.
        float moveHorizontal = Input.GetAxis("Horizontal");

       
        // Di chuyển trái phải
        float move = Input.GetAxis("Horizontal");
        bool isMoving = moveHorizontal != 0; // khai báo biến isMoving
        animator.SetBool("isMoving", isMoving);

        if (isMoving) // nếu nhân vật đang di chuyển
        {
            rb.velocity = new Vector2(move * speed, rb.velocity.y);
            //transform.position += new Vector3(moveHorizontal * speed * Time.deltaTime, 0f, 0f);

            //quay mặt hướng chạy
            if (moveHorizontal > 0)
                transform.localScale = new Vector3(5, 5, 5);
            else if (moveHorizontal < 0)
                transform.localScale = new Vector3(-5, 5, 5);
        }


        // check ground
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // reset jump khi chạm đất
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // double jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
        {
            BackgroundMusic.PlayJumpSfx();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }
    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        // Nếu buff cũ còn chạy thì dừng lại rồi bắt đầu buff mới.
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

   // boot speed player
    IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        // Tăng tốc tạm thời rồi trả lại tốc độ gốc sau khi hết thời gian.
        speed = originalSpeed * multiplier;

        Debug.Log("Speed Boost ON");

        yield return new WaitForSecondsRealtime(duration);

        speed = originalSpeed;

        Debug.Log("Speed Boost OFF");
    }
}
