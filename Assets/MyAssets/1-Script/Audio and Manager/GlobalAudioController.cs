using UnityEngine;
using UnityEngine.UI;

public class GlobalAudioController : MonoBehaviour
{
    [Header("UI Elements")]
    public Button muteButton;
    public Image buttonImage;        // optional: for mute/unmute icon
    public Sprite muteIcon;
    public Sprite unmuteIcon;
    public Slider volumeSlider;

    private bool isMuted;

    void Start()
    {
        // Load mute state (default = unmuted)
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;

        // Always reset slider to FULL at game start
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
            volumeSlider.value = 1f; // start full
        }

        ApplyAudioState();

        if (muteButton != null)
            muteButton.onClick.AddListener(ToggleMute);
    }

    public void SetVolume(float value)
    {
        if (!isMuted)
        {
            AudioListener.volume = value;
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);

        if (isMuted)
        {
            AudioListener.volume = 0f;
            if (volumeSlider != null)
                volumeSlider.value = 0f; // move slider down to match mute
        }
        else
        {
            AudioListener.volume = 1f; // reset to full when unmuted
            if (volumeSlider != null)
                volumeSlider.value = 1f; // slider jumps back to full
        }

        UpdateUI();
    }

    private void ApplyAudioState()
    {
        if (isMuted)
        {
            AudioListener.volume = 0f;
            if (volumeSlider != null)
                volumeSlider.value = 0f;
        }
        else
        {
            AudioListener.volume = 1f;
            if (volumeSlider != null)
                volumeSlider.value = 1f;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (buttonImage != null && muteIcon != null && unmuteIcon != null)
            buttonImage.sprite = isMuted ? muteIcon : unmuteIcon;

        if (muteButton.GetComponentInChildren<Text>() != null)
            muteButton.GetComponentInChildren<Text>().text = isMuted ? "Unmute" : "Mute";
    }
}
