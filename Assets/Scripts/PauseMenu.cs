using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    [Range(0f, 1f)] public float pausedVolume = 0.3f;

    public Image muteImage;
    public Sprite soundOnSprite;
    public Sprite mutedSprite;

    bool isPaused = false;
    bool isMuted = false;

    CursorLockMode previousLockState;
    bool previousCursorVisible;

    void Start()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PauseMenu: Pause Panel is not assigned");
            enabled = false;
            return;
        }
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        ApplyVolume();
        UpdateMuteImage();
    }

    void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        // Remember the game's cursor state, then free the mouse for the menu
        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        ApplyVolume();
    }

    public void Resume()
    {
        Debug.Log("Resume called");

        // Put the cursor back the way the game had it
        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisible;

        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        ApplyVolume();
    }

    public void ToggleMute()
    {
        Debug.Log("ToggleMute called");
        isMuted = !isMuted;
        ApplyVolume();
        UpdateMuteImage();
    }

    void ApplyVolume()
    {
        if (isMuted) AudioListener.volume = 0f;
        else if (isPaused) AudioListener.volume = pausedVolume;
        else AudioListener.volume = 1f;
    }

    void UpdateMuteImage()
    {
        if (muteImage != null)
            muteImage.sprite = isMuted ? mutedSprite : soundOnSprite;
    }

    public void LoadMainMenu()
    {
        Debug.Log("LoadMainMenu called");
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called");
        Application.Quit();
    }
}
