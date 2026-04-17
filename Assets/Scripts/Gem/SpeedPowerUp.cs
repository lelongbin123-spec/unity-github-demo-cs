using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ghi chu:
// - Power-up tang toc tam thoi cho player thong qua CharacterMovement.
// - Sau khi kich hoat hoac cham dat, vat pham se bi huy.
public class SpeedPowerUp : MonoBehaviour
{
    // Hệ số tăng tốc và thời gian hiệu lực của buff.
    public float speedMultiplier = 2f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi player ăn power-up thì kích hoạt buff tốc độ.
        if (other.CompareTag("Player"))
        {
            CharacterMovement player = other.GetComponent<CharacterMovement>();

            if (player != null)
            {
                player.ActivateSpeedBoost(speedMultiplier, duration);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
