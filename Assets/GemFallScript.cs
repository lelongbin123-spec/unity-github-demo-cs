using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemFallScript : MonoBehaviour
{
    // Khai báo biến để chứa prefab của viên ngọc. Đây sẽ là đối tượng mà chúng ta sẽ tạo ra trong trò chơi.
    public GameObject gemPrefab;
    // Biến đếm thời gian kể từ lần sinh viên ngọc cuối cùng.
    public float timer;
    // Khoảng thời gian (tính bằng giây) giữa mỗi lần sinh viên ngọc mới.
    public float spawnInterval = 3f; //tần suất spawn: 3 giây / 1 gem
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Cộng dồn thời gian từ lần cuối cập nhật đến bây giờ vào biến timer.
        timer += Time.deltaTime;
        // Kiểm tra nếu thời gian đã đủ lớn bằng hoặc lớn hơn khoảng thời gian sinh viên ngọc.
        if (timer >= spawnInterval)
        {
            SpawnGem(); // Gọi hàm sinh viên ngọc.
            timer = 0; // Đặt lại biến đếm thời gian.
        }
    }
    void SpawnGem()
    {
        /* Khai báo và tạo một biến có giá trị ngẫu nhiên trong khoảng màn hình trước khi tạo gem mới. 
        * Biến này đóng vai trò là tọa độ X (ngang) mới.
        */
        float randomX = Random.Range(-8f, 8f); //Màn hình rộng 16 point nên lề trái là -8 và biên phải là 8
                                               //Khai báo một biến tọa độ vị trí và lưu giá trị tọa độ trên
        Vector3 spawnPosition = new Vector3(randomX, 6f, 0); // Đưa biến số này vào Vector3, để tạo tọa độ vị trí mới

        //Đưa tọa độ này vào function (hàm) Instantiate để tạo và thả viên gem mới
        Instantiate(gemPrefab, spawnPosition, Quaternion.identity);
    }
}
