using UnityEngine;

// Ghi chu:
// - Quan ly nhan vat duoc chon, tai sprite/animation tu Resources va apply len player.
// - Luu y: nen validate characterId truoc khi luu de tranh load thieu asset.
public static class CharacterAppearanceManager
{
    // Key lưu nhân vật đã chọn và danh sách ID hợp lệ.
    private const string SelectedCharacterKey = "selected_character_id";
    private static readonly string[] CharacterIds = { "Player1", "Player2", "Player3", "Player4" };

    public static string[] GetCharacterIds()
    {
        return CharacterIds;
    }

    public static string GetSelectedCharacterId()
    {
        return PlayerPrefs.GetString(SelectedCharacterKey, CharacterIds[0]);
    }

    public static void SetSelectedCharacter(string characterId)
    {
        PlayerPrefs.SetString(SelectedCharacterKey, characterId);
        PlayerPrefs.Save();
    }

    public static Sprite GetPreviewSprite(string characterId)
    {
        return Resources.Load<Sprite>($"CharacterSelect/{characterId}/Idle");
    }

    public static void ApplyToPlayer(GameObject player)
    {
        // Load sprite/animation theo nhân vật đã chọn rồi gán vào player hiện tại.
        if (player == null)
        {
            return;
        }

        string characterId = GetSelectedCharacterId();
        Sprite idleSprite = Resources.Load<Sprite>($"CharacterSelect/{characterId}/Idle");
        AnimationClip idleClip = Resources.Load<AnimationClip>($"CharacterSelect/{characterId}/{characterId}Idle");
        AnimationClip runClip = Resources.Load<AnimationClip>($"CharacterSelect/{characterId}/{characterId}Run");

        if (idleSprite == null || idleClip == null || runClip == null)
        {
            Debug.LogWarning($"Missing character assets for {characterId}.");
            return;
        }

        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        Animator animator = player.GetComponent<Animator>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            overrideController["SlowAnimation"] = idleClip;
            overrideController["FastAnimation"] = runClip;
            animator.runtimeAnimatorController = overrideController;
            animator.Rebind();

            if (player.activeInHierarchy)
            {
                animator.Update(0f);
            }
        }
    }
}
