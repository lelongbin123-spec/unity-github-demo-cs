using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script bẫy gai:
// nếu player va chạm thì bị trừ điểm và rung màn hình.
public class Spike : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Spike dùng collision thay vì trigger để bắt va chạm vật lý.
        if (collision.gameObject.CompareTag("Player"))
        {
            BackgroundMusic.PlaySpikeHitSfx();
            ScoreManager.instance.AddScore(-2);
            ScreenShakeManager.Shake();
        }
    }
}
