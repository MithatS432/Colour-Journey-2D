using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public AudioListener audioListener;

    public Button mainSoundButton;
    public Sprite onMainSoundSprite;
    public Sprite offMainSoundSprite;

    public Button SoundFXButton;
    public Sprite onSoundFXSprite;
    public Sprite offSoundFXSprite;

    public void UpdateMainSoundButton()
    {
        audioListener.enabled = !audioListener.enabled;

        mainSoundButton.image.sprite =
            audioListener.enabled ? onMainSoundSprite : offMainSoundSprite;
    }

    public void UpdateSoundFXButton()
    {
        SFXManager.IsSFXOn = !SFXManager.IsSFXOn;

        SoundFXButton.image.sprite =
            SFXManager.IsSFXOn ? onSoundFXSprite : offSoundFXSprite;
    }
}