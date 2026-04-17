using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

// Script điều khiển menu và gameplay:
// mở/đóng panel, chọn nhân vật, đổi setting, pause game và chạy animation bằng DOTween.
public class MenuUIController : MonoBehaviour
{
    public GameObject startMenuPanel;
    public GameObject characterSelectPanel;
    public GameObject settingsPanel;
    public GameObject pausePanel;
    public GameObject instructionPanel;
    public GameObject gameplayButtonsPanel;
    public GameObject pauseButton;
    public GameObject exitButton;
    public GameObject instructionButton;
    public GameObject scoreText;
    public GameObject timerText;
    public GameObject player;
    public GameObject gemSpawner;

    public CanvasGroup startMenuCanvasGroup;
    public CanvasGroup characterSelectCanvasGroup;
    public CanvasGroup settingsCanvasGroup;
    public CanvasGroup pauseCanvasGroup;
    public CanvasGroup instructionCanvasGroup;

    public Image player1ButtonImage;
    public Image player2ButtonImage;
    public Image player3ButtonImage;
    public Image player4ButtonImage;
    public TextMeshProUGUI soundStatusText;
    public TextMeshProUGUI screenShakeStatusText;

    [SerializeField] private Vector2 outlineSize = new Vector2(8f, 8f);
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float dimmedScale = 0.94f;

    private void Start()
    {
        Time.timeScale = 0f;

        CacheCanvasGroups();
        ConfigureGameplayButtonsContainer();

        if (player != null) player.SetActive(false);
        if (gemSpawner != null) gemSpawner.SetActive(false);
        if (scoreText != null) scoreText.SetActive(false);
        if (timerText != null) timerText.SetActive(false);

        HidePanelInstant(characterSelectPanel, characterSelectCanvasGroup);
        HidePanelInstant(settingsPanel, settingsCanvasGroup);
        HidePanelInstant(pausePanel, pauseCanvasGroup);
        HidePanelInstant(instructionPanel, instructionCanvasGroup);
        ShowPanelInstant(startMenuPanel, startMenuCanvasGroup);

        SetGameplayButtonsVisible(false);

        CharacterAppearanceManager.ApplyToPlayer(player);
        RefreshCharacterSelectionVisuals();
        RefreshSoundStatus();
        RefreshScreenShakeStatus();

        PlayIntroAnimation();
    }

    private void OnDisable()
    {
        KillAllUiTweens();
    }

    private void OnDestroy()
    {
        KillAllUiTweens();
    }

    public void OpenCharacterSelect()
    {
        if (instructionPanel != null) HidePanelInstant(instructionPanel, instructionCanvasGroup);
        if (settingsPanel != null) HidePanelInstant(settingsPanel, settingsCanvasGroup);
        ShowPanelAnimated(characterSelectPanel, characterSelectCanvasGroup, PanelAnimation.SlideFromRight);
        HidePanelAnimated(startMenuPanel, startMenuCanvasGroup);

        SetGameplayButtonsVisible(false);
        RefreshCharacterSelectionVisuals();
    }

    public void BackToStartMenu()
    {
        if (characterSelectPanel != null) HidePanelAnimated(characterSelectPanel, characterSelectCanvasGroup);
        if (settingsPanel != null) HidePanelAnimated(settingsPanel, settingsCanvasGroup);
        if (instructionPanel != null) HidePanelInstant(instructionPanel, instructionCanvasGroup);
        ShowPanelAnimated(startMenuPanel, startMenuCanvasGroup, PanelAnimation.SlideFromLeft);

        SetGameplayButtonsVisible(false);
    }

    public void OpenSettings()
    {
        if (characterSelectPanel != null) HidePanelAnimated(characterSelectPanel, characterSelectCanvasGroup);
        ShowPanelAnimated(settingsPanel, settingsCanvasGroup, PanelAnimation.Popup);
        HidePanelAnimated(startMenuPanel, startMenuCanvasGroup);
        RefreshSoundStatus();
        RefreshScreenShakeStatus();
    }

    public void CloseSettings()
    {
        HidePanelAnimated(settingsPanel, settingsCanvasGroup);
        ShowPanelAnimated(startMenuPanel, startMenuCanvasGroup, PanelAnimation.SlideFromLeft);
    }

    public void ToggleSound()
    {
        BackgroundMusic.ToggleAudio();
        RefreshSoundStatus();
        PunchPanel(settingsPanel);
    }

    public void ToggleScreenShake()
    {
        ScreenShakeManager.ToggleScreenShake();
        RefreshScreenShakeStatus();
        PunchPanel(settingsPanel);
    }

    public void ConfirmCharacterAndPlay()
    {
        HidePanelAnimated(characterSelectPanel, characterSelectCanvasGroup);
        HidePanelInstant(settingsPanel, settingsCanvasGroup);
        HidePanelInstant(pausePanel, pauseCanvasGroup);
        HidePanelInstant(instructionPanel, instructionCanvasGroup);

        SetGameplayButtonsVisible(true);

        if (player != null) player.SetActive(true);
        if (gemSpawner != null) gemSpawner.SetActive(true);
        if (scoreText != null) scoreText.SetActive(true);
        if (timerText != null) timerText.SetActive(true);

        CharacterAppearanceManager.ApplyToPlayer(player);

        Time.timeScale = 1f;
        ScoreManager.instance.BeginRun();
    }

    public void PauseGame()
    {
        HidePanelInstant(instructionPanel, instructionCanvasGroup);
        ShowPanelAnimated(pausePanel, pauseCanvasGroup, PanelAnimation.Popup);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        HidePanelAnimated(pausePanel, pauseCanvasGroup);
        Time.timeScale = 1f;
    }

    public void OpenInstructionPanel()
    {
        HidePanelInstant(pausePanel, pauseCanvasGroup);
        ShowPanelAnimated(instructionPanel, instructionCanvasGroup, PanelAnimation.Popup);
        Time.timeScale = 0f;
    }

    public void CloseInstructionPanel()
    {
        HidePanelAnimated(instructionPanel, instructionCanvasGroup);
        Time.timeScale = 1f;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SelectPlayer1()
    {
        SelectCharacter("Player1");
    }

    public void SelectPlayer2()
    {
        SelectCharacter("Player2");
    }

    public void SelectPlayer3()
    {
        SelectCharacter("Player3");
    }

    public void SelectPlayer4()
    {
        SelectCharacter("Player4");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SelectCharacter(string characterId)
    {
        CharacterAppearanceManager.SetSelectedCharacter(characterId);
        CharacterAppearanceManager.ApplyToPlayer(player);
        RefreshCharacterSelectionVisuals();
    }

    private void RefreshCharacterSelectionVisuals()
    {
        string selectedCharacterId = CharacterAppearanceManager.GetSelectedCharacterId();

        SetCharacterImageState(player1ButtonImage, selectedCharacterId == "Player1");
        SetCharacterImageState(player2ButtonImage, selectedCharacterId == "Player2");
        SetCharacterImageState(player3ButtonImage, selectedCharacterId == "Player3");
        SetCharacterImageState(player4ButtonImage, selectedCharacterId == "Player4");
    }

    private void SetCharacterImageState(Image targetImage, bool isSelected)
    {
        if (targetImage == null)
        {
            return;
        }

        Color color = targetImage.color;
        color.a = isSelected ? 1f : 0.42f;
        targetImage.color = color;

        targetImage.rectTransform.DOKill();
        targetImage.rectTransform
            .DOScale(isSelected ? selectedScale : dimmedScale, 0.18f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .SetLink(targetImage.gameObject, LinkBehaviour.KillOnDestroy);

        Outline outline = targetImage.GetComponent<Outline>();
        if (outline == null)
        {
            outline = targetImage.gameObject.AddComponent<Outline>();
        }

        outline.effectDistance = outlineSize;
        outline.enabled = isSelected;
    }

    private void SetGameplayButtonsVisible(bool isVisible)
    {
        if (gameplayButtonsPanel != null)
        {
            gameplayButtonsPanel.SetActive(isVisible);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(isVisible);
        }

        if (exitButton != null)
        {
            exitButton.SetActive(isVisible);
        }

        if (instructionButton != null)
        {
            instructionButton.SetActive(isVisible);
        }
    }

    private void ConfigureGameplayButtonsContainer()
    {
        if (gameplayButtonsPanel == null)
        {
            return;
        }

        Image panelImage = gameplayButtonsPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            Color color = panelImage.color;
            color.a = 0f;
            panelImage.color = color;
            panelImage.raycastTarget = false;
        }
    }

    private void RefreshSoundStatus()
    {
        if (soundStatusText == null)
        {
            return;
        }

        bool isEnabled = BackgroundMusic.IsAudioEnabled();
        soundStatusText.text = isEnabled ? "Audio ON" : "Audio OFF";
    }

    private void RefreshScreenShakeStatus()
    {
        if (screenShakeStatusText == null)
        {
            return;
        }

        bool isEnabled = ScreenShakeManager.IsScreenShakeEnabled();
        screenShakeStatusText.text = isEnabled ? "Screen Shake ON" : "Screen Shake OFF";
    }

    private void CacheCanvasGroups()
    {
        startMenuCanvasGroup = GetCanvasGroup(startMenuPanel, startMenuCanvasGroup);
        characterSelectCanvasGroup = GetCanvasGroup(characterSelectPanel, characterSelectCanvasGroup);
        settingsCanvasGroup = GetCanvasGroup(settingsPanel, settingsCanvasGroup);
        pauseCanvasGroup = GetCanvasGroup(pausePanel, pauseCanvasGroup);
        instructionCanvasGroup = GetCanvasGroup(instructionPanel, instructionCanvasGroup);
    }

    private CanvasGroup GetCanvasGroup(GameObject targetPanel, CanvasGroup currentGroup)
    {
        if (currentGroup != null)
        {
            return currentGroup;
        }

        if (targetPanel == null)
        {
            return null;
        }

        return targetPanel.GetComponent<CanvasGroup>();
    }

    private void PlayIntroAnimation()
    {
        ShowPanelAnimated(startMenuPanel, startMenuCanvasGroup, PanelAnimation.SlideFromTop);
    }

    private void ShowPanelAnimated(GameObject targetPanel, CanvasGroup canvasGroup, PanelAnimation animation)
    {
        if (targetPanel == null)
        {
            return;
        }

        if (canvasGroup == null)
        {
            targetPanel.SetActive(true);
            return;
        }

        RectTransform rectTransform = targetPanel.GetComponent<RectTransform>();
        targetPanel.SetActive(true);
        canvasGroup.DOKill();
        rectTransform?.DOKill();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        Vector2 endPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
        Vector2 startPosition = endPosition;
        Vector3 startScale = Vector3.one;

        switch (animation)
        {
            case PanelAnimation.SlideFromTop:
                startPosition = endPosition;
                break;
            case PanelAnimation.SlideFromLeft:
                startPosition = endPosition + new Vector2(-120f, 0f);
                break;
            case PanelAnimation.SlideFromRight:
                startPosition = endPosition + new Vector2(120f, 0f);
                break;
            case PanelAnimation.Popup:
                startScale = new Vector3(0.88f, 0.88f, 1f);
                break;
        }

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition;
            rectTransform.localScale = startScale;
        }

        Sequence sequence = DOTween.Sequence().SetUpdate(true).SetLink(targetPanel, LinkBehaviour.KillOnDestroy);
        sequence.Join(canvasGroup.DOFade(1f, 0.24f));

        if (rectTransform != null)
        {
            sequence.Join(rectTransform.DOAnchorPos(endPosition, 0.28f).SetEase(Ease.OutCubic).SetLink(targetPanel, LinkBehaviour.KillOnDestroy));
            sequence.Join(rectTransform.DOScale(Vector3.one, 0.28f).SetEase(Ease.OutBack).SetLink(targetPanel, LinkBehaviour.KillOnDestroy));
        }
    }

    private void HidePanelAnimated(GameObject targetPanel, CanvasGroup canvasGroup)
    {
        if (targetPanel == null)
        {
            return;
        }

        if (canvasGroup == null)
        {
            targetPanel.SetActive(false);
            return;
        }

        RectTransform rectTransform = targetPanel.GetComponent<RectTransform>();
        canvasGroup.DOKill();
        rectTransform?.DOKill();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Sequence sequence = DOTween.Sequence().SetUpdate(true).SetLink(targetPanel, LinkBehaviour.KillOnDestroy);
        sequence.Join(canvasGroup.DOFade(0f, 0.18f));
        if (rectTransform != null)
        {
            sequence.Join(rectTransform.DOScale(0.96f, 0.18f).SetEase(Ease.OutQuad).SetLink(targetPanel, LinkBehaviour.KillOnDestroy));
        }
        sequence.OnComplete(() =>
        {
            if (targetPanel == null)
            {
                return;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
            }

            targetPanel.SetActive(false);
        });
    }

    private void ShowPanelInstant(GameObject targetPanel, CanvasGroup canvasGroup)
    {
        if (targetPanel == null)
        {
            return;
        }

        targetPanel.SetActive(true);
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HidePanelInstant(GameObject targetPanel, CanvasGroup canvasGroup)
    {
        if (targetPanel == null)
        {
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        targetPanel.SetActive(false);
    }

    private void PunchPanel(GameObject targetPanel)
    {
        if (targetPanel == null)
        {
            return;
        }

        RectTransform rectTransform = targetPanel.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.DOKill();
        rectTransform
            .DOPunchScale(new Vector3(0.04f, 0.04f, 0f), 0.22f, 3, 0.5f)
            .SetUpdate(true)
            .SetLink(targetPanel, LinkBehaviour.KillOnDestroy);
    }

    private void KillAllUiTweens()
    {
        KillPanelTweens(startMenuPanel, startMenuCanvasGroup);
        KillPanelTweens(characterSelectPanel, characterSelectCanvasGroup);
        KillPanelTweens(settingsPanel, settingsCanvasGroup);
        KillPanelTweens(pausePanel, pauseCanvasGroup);
        KillPanelTweens(instructionPanel, instructionCanvasGroup);

        KillGameObjectTweens(gameplayButtonsPanel);
        KillGameObjectTweens(pauseButton);
        KillGameObjectTweens(exitButton);
        KillGameObjectTweens(instructionButton);
        KillGameObjectTweens(scoreText);
        KillGameObjectTweens(timerText);

        KillImageTween(player1ButtonImage);
        KillImageTween(player2ButtonImage);
        KillImageTween(player3ButtonImage);
        KillImageTween(player4ButtonImage);
    }

    private void KillPanelTweens(GameObject panel, CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
        }

        KillGameObjectTweens(panel);
    }

    private void KillGameObjectTweens(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        target.transform.DOKill();

        RectTransform rectTransform = target.transform as RectTransform;
        rectTransform?.DOKill();

        Graphic[] graphics = target.GetComponentsInChildren<Graphic>(true);
        foreach (Graphic graphic in graphics)
        {
            if (graphic != null)
            {
                graphic.DOKill();
            }
        }
    }

    private void KillImageTween(Image image)
    {
        if (image == null)
        {
            return;
        }

        image.DOKill();
        image.rectTransform.DOKill();
    }

    private enum PanelAnimation
    {
        SlideFromTop,
        SlideFromLeft,
        SlideFromRight,
        Popup
    }
}
