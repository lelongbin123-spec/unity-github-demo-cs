using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - Item diem cao, cong diem theo cong thuc baseValue * multiplier khi cham player.
// - Bien speed hien chua duoc su dung trong script nay.
public class HighValueGem : MonoBehaviour
{
    // Hệ số nhân điểm của item này.
    public int multiplier; // x2, x3, x4
    public int baseValue = 1;
    public float speed = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu player chạm vào thì cộng điểm, còn chạm đất thì huỷ item.
        if (other.CompareTag("Player"))
        {
            int finalValue = baseValue * multiplier;

            ScoreManager.instance.AddScore(finalValue);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
