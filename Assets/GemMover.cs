using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Text Mesh Pro 

public class GemMover : MonoBehaviour
{
    /*
    * khai báo biến tốc độ là một số thực có giá trị bằng 5.
    * Public sẽ cho phép ta truy cập giá trị speed từ UnityEditor
    */
    public float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime); //tạo chuyển động theo phương thẳng đứng hướng xuống với tốc độ trên theo thời gian
    }
    void OnTriggerEnter2D(Collider2D other) //other chính là thông tin của bất kỳ collider nào va chạm với collider này
    {
        // thiết lập điều kiện kiểm tra thông tin của OTHER
        if (other.gameObject.CompareTag("Player"))
        // nếu, phương thức so sánh gameobject tag của other với nhãn "Player" là đúng
        { // thì

            AudioSource audioSource = other.GetComponent<AudioSource>();
            ScoreManager.AddScore(1);
            // ...existing code...
            //play âm thanh từ component đó
            audioSource.Play();
            Destroy(gameObject); //xóa gameObject đang gắn collider này. (Không phải là other)
                                 //nghĩa là xóa viên ngọc này, không phải xóa người chơi đang va chạm
        }
        else if (other.gameObject.CompareTag("Ground")) // còn không thì, nếu là mặt đất,
        { // thì
            Destroy(gameObject); //xóa gameObject đang gắn collider này. (Không phải là other)
                                 //nghĩa là xóa viên ngọc này, không phải xóa mặt đất
        }
    }
}
