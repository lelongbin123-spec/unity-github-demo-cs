using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighValueGem : MonoBehaviour
{
    public int multiplier; // x2, x3, x4
    public int baseValue = 1;
    public float speed = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int finalValue = baseValue * multiplier;

            ScoreManager.AddScore(finalValue);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
