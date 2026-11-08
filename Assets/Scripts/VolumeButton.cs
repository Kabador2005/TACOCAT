using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    // fuck my variables
    public Sprite volumeOn;
    public Sprite volumeOff;
    private Image buttonImage;
    private bool isMuted = false;

    void Start()
    {
        //get component and update button image
        buttonImage = GetComponent<Image>();
        UpdateButton();
    }

    public void ToggleVolume()
    {
        // toggle the mute state
        isMuted = !isMuted;

        // set the audio listener volume based on the mute state
        AudioListener.volume = isMuted ? 0f : 1f;

        UpdateButton();
    }

    void UpdateButton()
    {
        // update the button image based on the mute state
        buttonImage.sprite = isMuted ? volumeOff : volumeOn;
    }
}