using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ghi chu:
// - Them feedback hover/press cho button bang scale va color tween.
// - Cac tween duoc kill khi object tat/huy de tranh state UI bi treo.
public class UIButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // Các thông số scale và thời lượng tween khi tương tác với button.
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float tweenDuration = 0.14f;

    private RectTransform rectTransform;
    private Graphic targetGraphic;
    private Color baseColor = Color.white;
    private Color hoverColor = Color.white;
    private Color pressedColor = Color.white;
    private Vector3 initialScale = Vector3.one;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            initialScale = rectTransform.localScale;
        }
    }

    public void Configure(Graphic graphic, Color sourceColor)
    {
        // Khởi tạo màu gốc, màu hover và màu pressed cho button hiện tại.
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                initialScale = rectTransform.localScale;
            }
        }

        targetGraphic = graphic;
        baseColor = sourceColor;
        hoverColor = Color.Lerp(sourceColor, Color.white, 0.18f);
        pressedColor = Color.Lerp(sourceColor, Color.black, 0.12f);

        if (targetGraphic != null)
        {
            targetGraphic.color = baseColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(initialScale * hoverScale, Ease.OutBack);
        AnimateColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(initialScale, Ease.OutCubic);
        AnimateColor(baseColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateScale(initialScale * pressedScale, Ease.OutQuad);
        AnimateColor(pressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateScale(initialScale * hoverScale, Ease.OutBack);
        AnimateColor(hoverColor);
    }

    private void AnimateScale(Vector3 targetScale, Ease ease)
    {
        // Tween scale của button khi hover/press.
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.DOKill();
        rectTransform.DOScale(targetScale, tweenDuration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void AnimateColor(Color targetColor)
    {
        // Tween màu của graphic khi trạng thái button thay đổi.
        if (targetGraphic == null)
        {
            return;
        }

        targetGraphic.DOKill();
        targetGraphic.DOColor(targetColor, tweenDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.DOKill();
        }

        if (targetGraphic != null)
        {
            targetGraphic.DOKill();
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}
