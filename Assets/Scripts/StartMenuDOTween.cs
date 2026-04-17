using UnityEngine;
using DG.Tweening;

// Ghi chu:
// - Tao hieu ung slide-in cho cac button menu bang DOTween.
// - Vi tri goc duoc cache lai de co the phat lai animation moi lan panel duoc bat.
public class StartMenuDOTween : MonoBehaviour
{
    // Danh sách các button sẽ được trượt vào khi menu xuất hiện.
    public RectTransform[] buttons;
    public float slideDuration = 0.6f;
    public float delayBetweenButtons = 0.2f;
    public float startOffsetX = -500f;

    private Vector2[] originalPositions;

    private void Awake()
    {
        CacheOriginalPositions();
    }

    private void OnEnable()
    {
        CacheOriginalPositions();
        SlideInButtons();
    }

    private void CacheOriginalPositions()
    {
        if (buttons == null || buttons.Length == 0)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        if (originalPositions == null || originalPositions.Length != buttons.Length)
        {
            originalPositions = new Vector2[buttons.Length];
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                continue;
            }

            originalPositions[i] = buttons[i].anchoredPosition;
        }
    }

    private void SlideInButtons()
    {
        // Mỗi button bắt đầu lệch sang trái rồi tween về vị trí gốc.
        if (buttons == null || originalPositions == null)
        {
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform btn = buttons[i];
            if (btn == null)
            {
                continue;
            }

            Vector2 targetPos = originalPositions[i];
            btn.DOKill();
            btn.anchoredPosition = targetPos + new Vector2(startOffsetX, 0f);
            btn.DOAnchorPos(targetPos, slideDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * delayBetweenButtons)
                .SetUpdate(true)
                .SetLink(btn.gameObject, LinkBehaviour.KillOnDestroy);
        }
    }

    private void OnDisable()
    {
        if (buttons == null)
        {
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].DOKill();
            }
        }
    }
}
