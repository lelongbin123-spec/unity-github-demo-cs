using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Text Mesh Pro 

public class ScoreManager : MonoBehaviour
{
    // tạo một biến số điểm bắt đầu bằng 0 để lưu giá trị điểm của người chơi
    public static int score = 0; //static sẽ được giải thích sau trong chương c#
    public TextMeshProUGUI scoreText; // tạo một biến thuộc kiểu TextMeshProUGUI tên scoreText và có thể truy cập từ Unity Editor
    private float remainingTime;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    private bool isGameOver = false;
    public GameObject player;
    public GameObject gemSpawner;

    public static ScoreManager instance;
    // khai báo một hàm dành cho lớp ScoreManager nhằm tăng số điểm của người chơi
    private void Start()
    {
        score = 0; // reset điểm khi bắt đầu game
        
        remainingTime = 60f; //thời gian còn lại tại thời điểm bắt đầu bằng 30s (thời lượng của trò chơi)
        gameOverPanel.SetActive(false);
        StartCoroutine(CountdownTimer());
        // là một phương thức nâng cao để gọi hàm CountdownTimer
        // nhằm cho phép đồng hồ chạy song song, tiếp tục đếm khi chuyển qua frame mới và kết thúc ở frame mới khi đạt đúng thời gian
    }
    void Awake()
    {
        instance = this;
    }
    public static void AddScore(int amount) //(int amount) nghĩa là hàm sẽ chỉ nhận giá trị là integer, và giá trị này sẽ được gán vào biến có tên amount
    {
        score += amount; //tăng điểm theo giá trị của amount được truyền vào tại sự kiện gọi AddScore
        if (score < 0)
            score = 0;
    }

    void Update()
    {
        // cập nhật điểm hiển thị trên UI
        scoreText.text = "Score: " + score + " | Time: " + Mathf.CeilToInt(remainingTime); //Mathf.CeilToInt(remainingTime) làm tròn số nguyên dương
       
        if (remainingTime <= 0 && !isGameOver)
        {
            GameOver();
        }
    }
    private IEnumerator CountdownTimer()
    {
        while (remainingTime > 0) //nếu remainingTime vẫn lớn hơn 0 thì liên tục lập lại logic bên dưới
        {
            yield return new WaitForSeconds(1f); //Mỗi 1 giây trôi qua
            remainingTime--; //trừ 1 của remainingTime
        }
    }
    private void GameOver()
    {
       
        isGameOver = true;
        gameOverText.text = $"Game Over!\nScore: {score}";
        gameOverPanel.SetActive(true);
        // Xóa nhân vật
        Destroy(player);

        // Tắt GemSpawner
        gemSpawner.SetActive(false);

        // Dừng game
        Time.timeScale = 0f;

    }
}
