using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - Script di chuyen don gian bang transform theo input ngang/doc.
// - Co dau hieu la script cu hoac script hoc tap, de tranh trung lap voi CharacterMovement.
public class mover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Biến speed quyết định tốc độ của nhân vật là một số thực, và có thể được truy cập từ giao diện Unity Editor.
    public float speed = 5.0f;
    // Update is called once per frame
    void Update()
    {
        // Lấy đầu vào từ bàn phím, ở đây là phím chiều ngang, phím A/D hoặc mũi tên trái/phải
        float horizontalInput = Input.GetAxis("Horizontal");
        // Lấy đầu vào từ bàn phím, ở đây là phím chiều dọc, phím W/S hoặc mũi tên lên/xuống
        float verticalInput = Input.GetAxis("Vertical");

       
        // Tính vị trí mới của đối tượng dựa trên đầu vào và tốc độ
        float moveX = horizontalInput * speed * Time.deltaTime;
        float moveY = verticalInput * speed * Time.deltaTime;

        // Cập nhật ví trí mới của đối tượng chuẩn bị để được render ở khung hình tiếp theo
        transform.position = new Vector2(transform.position.x + moveX, transform.position.y + moveY);
    }
}
