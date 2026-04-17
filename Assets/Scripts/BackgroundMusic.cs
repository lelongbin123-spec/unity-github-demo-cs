using UnityEngine;

// Ghi chu:
// - Quan ly nhac nen global va trang thai bat/tat audio qua PlayerPrefs.
// - Singleton nay duoc giu lai giua cac scene bang DontDestroyOnLoad.
public class BackgroundMusic : MonoBehaviour
{
    // Khóa dùng để lưu trạng thái bật/tắt âm thanh.
    private const string AudioEnabledKey = "audio_enabled";

    private static BackgroundMusic instance;
    private AudioSource audioSource;
    [SerializeField] private AudioClip collectableClip;
    [SerializeField] private AudioClip spikeHitClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] [Range(0f, 1f)] private float collectableVolume = 0.8f;
    [SerializeField] [Range(0f, 1f)] private float spikeHitVolume = 0.9f;
    [SerializeField] [Range(0f, 1f)] private float jumpVolume = 0.75f;

    void Awake()
    {
        // Đảm bảo chỉ có một object nhạc nền tồn tại xuyên suốt các scene.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        ApplySavedAudioState();
    }

    public static bool IsAudioEnabled()
    {
        return PlayerPrefs.GetInt(AudioEnabledKey, 1) == 1;
    }

    public static void SetAudioEnabled(bool isEnabled)
    {
        // Đồng bộ cả PlayerPrefs, AudioListener và AudioSource hiện tại.
        PlayerPrefs.SetInt(AudioEnabledKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();

        AudioListener.volume = isEnabled ? 1f : 0f;

        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.mute = !isEnabled;
        }
    }

    public static void ToggleAudio()
    {
        SetAudioEnabled(!IsAudioEnabled());
    }

    public static void PlayCollectableSfx()
    {
        if (instance == null)
        {
            return;
        }

        instance.PlaySfx(instance.collectableClip, instance.collectableVolume);
    }

    public static void PlaySpikeHitSfx()
    {
        if (instance == null)
        {
            return;
        }

        instance.PlaySfx(instance.spikeHitClip, instance.spikeHitVolume);
    }

    public static void PlayJumpSfx()
    {
        if (instance == null)
        {
            return;
        }

        instance.PlaySfx(instance.jumpClip, instance.jumpVolume);
    }

    private void ApplySavedAudioState()
    {
        SetAudioEnabled(IsAudioEnabled());
    }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (!IsAudioEnabled() || clip == null || audioSource == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }
}
