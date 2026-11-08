using UnityEngine;
using UnityEngine.UI;

public class GameSfx : MonoBehaviour
{
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip failedOrderClip;
    [SerializeField] private AudioClip successfulOrderClip;
    [SerializeField] private AudioClip buttonClickClip;

    private static GameSfx instance;
    private AudioSource sfxSource;

    private void Awake()
    {
        instance = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        // Include inactive buttons too, such as the pause and game-over menus.
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (button == null || !button.gameObject.scene.IsValid()) continue;
            if (button.GetComponent<ButtonClickSfx>() == null)
                button.gameObject.AddComponent<ButtonClickSfx>();
        }
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static void PlayExplosion() => instance?.Play(instance.explosionClip);

    public static void PlayOrderResult(bool success)
    {
        if (instance == null) return;
        instance.Play(success ? instance.successfulOrderClip : instance.failedOrderClip);
    }

    public static void PlayButtonClick() => instance?.Play(instance.buttonClickClip);

    private void Play(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }
}
