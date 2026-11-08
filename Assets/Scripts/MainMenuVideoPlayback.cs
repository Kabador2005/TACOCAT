using System.IO;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class MainMenuVideoPlayback : MonoBehaviour
{
    private const string WebVideoFileName = "MainMenuBackground.mp4";
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web builds cannot play an embedded VideoClip. Load it by URL instead.
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, WebVideoFileName);
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
#endif

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.Prepare();
    }

    private void OnDestroy()
    {
        if (videoPlayer == null) return;
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.errorReceived -= OnVideoError;
    }

    private void OnVideoPrepared(VideoPlayer player) => player.Play();

    private void OnVideoError(VideoPlayer player, string message)
    {
        Debug.LogError($"Main menu video failed to load: {message}", this);
    }
}
