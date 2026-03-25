using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadItem : MonoBehaviour
{
    public int penaltyScore = 5;
    public float speed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreManager.AddScore(-penaltyScore);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground")) // còn không thì, nếu là mặt đất,
        { // thì
            Destroy(gameObject); //xóa gameObject đang gắn collider này. (Không phải là other)
                                 //nghĩa là xóa viên ngọc này, không phải xóa mặt đất
        }
    }
}
