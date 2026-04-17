using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Text Mesh Pro 

// Ghi chu:
// - Quan ly score, countdown timer, game over panel va feedback UI cho diem/thoi gian.
// - Luu y: GameOver() dat Time.timeScale = 0, nen luong choi lai can khoi phuc ve 1.
public class ScoreManager : MonoBehaviour
{
    // Điểm hiện tại của người chơi.
    public static int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    private float remainingTime;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    private bool isGameOver = false;
    public GameObject player;
    public GameObject gemSpawner;

    public static ScoreManager instance;
    private Coroutine timerCoroutine;
    private readonly Color normalTimerColor = new Color(0.92f, 0.96f, 1f, 1f);
    private readonly Color warningTimerColor = new Color(1f, 0.78f, 0.33f, 1f);
    private readonly Color dangerTimerColor = new Color(1f, 0.45f, 0.45f, 1f);
    private readonly int warningThreshold = 10;
    private readonly int criticalThreshold = 5;
    private Vector3 timerBaseScale = Vector3.one;
    private Vector3 scoreBaseScale = Vector3.one;
    private Coroutine scorePulseCoroutine;
    private Coroutine scorePopupCoroutine;
    private GameObject activeScorePopup;

    private void Start()
    {
        // Thiết lập HUD về trạng thái mặc định khi scene bắt đầu.
        score = 0;
        remainingTime = 60f;
        CacheHudDefaults();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreText();
        UpdateTimerText();
    }

    void Awake()
    {
        instance = this;
    }

    public void BeginRun()
    {
        // Bắt đầu một lượt chơi mới: reset điểm, thời gian và UI.
        score = 0;
        remainingTime = 60f;
        isGameOver = false;
        CacheHudDefaults();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreText();
        UpdateTimerText();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(CountdownTimer());
    }

    public void AddScore(int amount)
    {
        // Cộng hoặc trừ điểm, sau đó phát feedback tương ứng trên UI.
        int previousScore = score;
        score += amount;
        if (score < 0)
            score = 0;

        UpdateScoreText();

        int gainedScore = score - previousScore;
        if (gainedScore > 0)
        {
            PlayScoreGainFeedback(gainedScore);
        }
        else if (amount < 0 && previousScore > score)
        {
            PlayScoreLossFeedback(previousScore - score);
        }
    }

    void Update()
    {
        UpdateTimerWarningVisuals();

        if (remainingTime <= 0 && !isGameOver)
        {
            GameOver();
        }
    }

    private IEnumerator CountdownTimer()
    {
        while (remainingTime > 0)
        {
            UpdateTimerText();
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        UpdateTimerText();
    }

    private void GameOver()
    {
        // Dừng gameplay và hiển thị bảng game over cùng điểm cuối.
        isGameOver = true;
        if (gameOverText != null)
        {
            gameOverText.text = $"Time Up\n<size=36>Final Score <color=#FFD36B>{score:00}</color></size>\n<size=24>{GetPerformanceLine()}</size>";
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (player != null)
            Destroy(player);

        if (gemSpawner != null)
            gemSpawner.SetActive(false);

        Time.timeScale = 0f;
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            return;
        }

        scoreText.SetText($"Score <color=#FFD36B>{score:00}</color>");
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int displayTime = Mathf.Max(0, Mathf.CeilToInt(remainingTime));
        int minutes = displayTime / 60;
        int seconds = displayTime % 60;
        bool isWarning = displayTime <= warningThreshold;
        bool isCritical = displayTime <= criticalThreshold;

        timerText.color = isCritical ? dangerTimerColor : isWarning ? warningTimerColor : normalTimerColor;

        if (isCritical)
        {
            timerText.SetText($"Time <color=#F2F7FF>{minutes:00}:{seconds:00}</color>\n<size=58%><color=#FF8B8B>FINAL SECONDS</color></size>");
            return;
        }

        if (isWarning)
        {
            timerText.SetText($"Time <color=#F2F7FF>{minutes:00}:{seconds:00}</color>\n<size=58%><color=#FFD36B>HURRY UP</color></size>");
            return;
        }

        timerText.SetText($"Time <color=#F2F7FF>{minutes:00}:{seconds:00}</color>");
    }

    private void CacheHudDefaults()
    {
        if (scoreText != null)
        {
            scoreBaseScale = scoreText.rectTransform.localScale;
        }

        if (timerText != null)
        {
            timerBaseScale = timerText.rectTransform.localScale;
        }
    }

    private void PlayScoreGainFeedback(int gainedScore)
    {
        if (scoreText == null)
        {
            return;
        }

        if (scorePulseCoroutine != null)
        {
            StopCoroutine(scorePulseCoroutine);
            scoreText.rectTransform.localScale = scoreBaseScale;
        }

        if (scorePopupCoroutine != null)
        {
            StopCoroutine(scorePopupCoroutine);
        }

        if (activeScorePopup != null)
        {
            Destroy(activeScorePopup);
            activeScorePopup = null;
        }

        scorePulseCoroutine = StartCoroutine(AnimateScorePulse());
        scorePopupCoroutine = StartCoroutine(AnimateScorePopup(gainedScore));
    }

    private void PlayScoreLossFeedback(int lostScore)
    {
        if (scoreText == null)
        {
            return;
        }

        if (scorePopupCoroutine != null)
        {
            StopCoroutine(scorePopupCoroutine);
        }

        if (activeScorePopup != null)
        {
            Destroy(activeScorePopup);
            activeScorePopup = null;
        }

        scorePopupCoroutine = StartCoroutine(AnimateScorePopup(-lostScore));
    }

    private IEnumerator AnimateScorePulse()
    {
        RectTransform scoreRect = scoreText.rectTransform;
        float duration = 0.22f;
        float elapsed = 0f;
        Vector3 peakScale = scoreBaseScale * 1.18f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.Sin(progress * Mathf.PI);
            scoreRect.localScale = Vector3.LerpUnclamped(scoreBaseScale, peakScale, eased);
            yield return null;
        }

        scoreRect.localScale = scoreBaseScale;
        scorePulseCoroutine = null;
    }

    private IEnumerator AnimateScorePopup(int scoreDelta)
    {
        // Tạo popup cộng/trừ điểm bay lên gần vị trí player.
        GameObject popupObject = new GameObject("ScoreGainPopup", typeof(RectTransform));
        activeScorePopup = popupObject;
        popupObject.transform.SetParent(scoreText.transform.parent, false);

        RectTransform popupRect = popupObject.GetComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.5f, 0.5f);
        popupRect.anchorMax = new Vector2(0.5f, 0.5f);
        popupRect.pivot = new Vector2(0.5f, 0.5f);
        popupRect.anchoredPosition = GetScorePopupStartPosition();
        popupRect.localScale = Vector3.one * 0.8f;

        TextMeshProUGUI popupText = popupObject.AddComponent<TextMeshProUGUI>();
        popupText.font = scoreText.font;
        popupText.fontSize = scoreText.fontSize * 0.95f;
        popupText.alignment = TextAlignmentOptions.Center;
        popupText.raycastTarget = false;
        bool isPositive = scoreDelta > 0;
        popupText.text = isPositive ? $"+{scoreDelta}" : scoreDelta.ToString();
        popupText.color = isPositive
            ? new Color(1f, 0.92f, 0.45f, 1f)
            : new Color(1f, 0.4f, 0.4f, 1f);

        float duration = 0.55f;
        float elapsed = 0f;
        Vector2 startPosition = popupRect.anchoredPosition;
        Vector2 endPosition = startPosition + new Vector2(0f, 35f);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);

            popupRect.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, eased);
            popupRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.8f, Vector3.one, eased);

            Color popupColor = popupText.color;
            popupColor.a = 1f - progress;
            popupText.color = popupColor;

            yield return null;
        }

        Destroy(popupObject);
        activeScorePopup = null;
        scorePopupCoroutine = null;
    }

    private Vector2 GetScorePopupStartPosition()
    {
        // Chuyển vị trí world của player sang toạ độ UI để đặt popup đúng chỗ.
        if (scoreText == null)
        {
            return Vector2.zero;
        }

        RectTransform parentRect = scoreText.transform.parent as RectTransform;
        if (parentRect == null)
        {
            return scoreText.rectTransform.anchoredPosition;
        }

        if (player == null)
        {
            return scoreText.rectTransform.anchoredPosition + new Vector2(0f, 45f);
        }

        Canvas canvas = scoreText.canvas;
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        Vector3 worldPosition = player.transform.position + new Vector3(0f, 1.8f, 0f);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, uiCamera, out Vector2 localPoint))
        {
            return localPoint;
        }

        return scoreText.rectTransform.anchoredPosition + new Vector2(0f, 45f);
    }

    private void UpdateTimerWarningVisuals()
    {
        if (timerText == null)
        {
            return;
        }

        int displayTime = Mathf.Max(0, Mathf.CeilToInt(remainingTime));

        if (isGameOver || displayTime > warningThreshold)
        {
            timerText.rectTransform.localScale = timerBaseScale;
            return;
        }

        float speed = displayTime <= criticalThreshold ? 11f : 8f;
        float amplitude = displayTime <= criticalThreshold ? 0.16f : 0.09f;
        float pulse = 1f + Mathf.Abs(Mathf.Sin(Time.unscaledTime * speed)) * amplitude;
        timerText.rectTransform.localScale = timerBaseScale * pulse;
    }

    private string GetPerformanceLine()
    {
        if (score >= 80)
        {
            return "Legendary reflexes.";
        }

        if (score >= 50)
        {
            return "Great run. You were on fire.";
        }

        if (score >= 25)
        {
            return "Nice pace. One more try for a bigger combo.";
        }

        return "Warm-up round. You can push this much higher.";
    }
}
