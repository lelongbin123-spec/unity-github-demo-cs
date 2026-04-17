using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - Item phat, khi cham player se tru diem va kich hoat rung man hinh.
// - Gia tri mac dinh hien tai la tru 1 diem, co the override trong Inspector/prefab.
public class BadItem : MonoBehaviour
{
    // Số điểm bị trừ khi player chạm phải vật phẩm xấu.
    public int penaltyScore = 1;
    public float speed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player chạm thì trừ điểm, chạm đất thì tự huỷ.
        if (collision.CompareTag("Player"))
        {
            ScoreManager.instance.AddScore(-penaltyScore);
            ScreenShakeManager.Shake();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
