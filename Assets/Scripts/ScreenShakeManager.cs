using System.Collections;
using UnityEngine;

// Ghi chu:
// - Cung cap rung man hinh theo kieu singleton va luu setting bat/tat vao PlayerPrefs.
// - Neu scene khong co instance san, script tu gan vao Camera.main khi can rung.
public class ScreenShakeManager : MonoBehaviour
{
    // Khóa lưu trạng thái rung màn hình.
    private const string ScreenShakeEnabledKey = "screen_shake_enabled";

    private static ScreenShakeManager instance;

    [SerializeField] private float defaultDuration = 0.18f;
    [SerializeField] private float defaultMagnitude = 0.18f;

    private Vector3 originalLocalPosition;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        originalLocalPosition = transform.localPosition;
    }

    public static bool IsScreenShakeEnabled()
    {
        return PlayerPrefs.GetInt(ScreenShakeEnabledKey, 1) == 1;
    }

    public static void SetScreenShakeEnabled(bool isEnabled)
    {
        PlayerPrefs.SetInt(ScreenShakeEnabledKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ToggleScreenShake()
    {
        SetScreenShakeEnabled(!IsScreenShakeEnabled());
    }

    public static void Shake(float? duration = null, float? magnitude = null)
    {
        // Điểm gọi chung để rung màn hình từ bất kỳ gameplay object nào.
        if (!IsScreenShakeEnabled())
        {
            return;
        }

        ScreenShakeManager target = instance;
        if (target == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            target = mainCamera.GetComponent<ScreenShakeManager>();
            if (target == null)
            {
                target = mainCamera.gameObject.AddComponent<ScreenShakeManager>();
            }
        }

        target.StartShake(duration ?? target.defaultDuration, magnitude ?? target.defaultMagnitude);
    }

    private void StartShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalLocalPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        // Rung camera bằng offset ngẫu nhiên trong một khoảng thời gian ngắn.
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = originalLocalPosition + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }
}
