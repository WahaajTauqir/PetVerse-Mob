using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionMaster : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;

    [Header("Sliders")]
    [SerializeField] Slider hungerSlider;
    [SerializeField] Slider energySlider;
    [SerializeField] Slider hygieneSlider;
    [SerializeField] Slider happinessSlider;
    [SerializeField] Slider bondLevelSlider;

    [Header("Mood Meter")]
    [SerializeField] private Transform pointerBase;

    public AudioSource uiAudioSource;
    public AudioClip buttonClickAudio;

    public GameObject playBall;

    private void Start()
    {
        gm.emotionSystem = FindObjectOfType<EmotionSystem>();
        gm.bondingSystem = FindObjectOfType<BondingSystem>();
    }
    // ---------------------------------------------------------------------------

    public void UpdateSliders()
    {
        hungerSlider.value = gm.needsSystem.GetFood();

        energySlider.value = gm.needsSystem.GetEnergy();

        hygieneSlider.value = gm.needsSystem.GetHygiene();

        happinessSlider.value = gm.needsSystem.GetWellbeing();

        float moodScore = Mathf.Clamp(gm.GetMoodScore(), 0f, 100f);
        SetPointerRotationFromMood(moodScore);
    }
    // ---------------------------------------------------------------------------

    // Player Activity
    public void Feed()
    {
        gm.actionSystem.currentActionType = ActionType.Feed;
        gm.generalAnimationManager.activityTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);


    }

    public void Play()
    {
        gm.actionSystem.currentActionType = ActionType.Play;
        gm.generalAnimationManager.activityTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    public void Groom()
    {
        gm.actionSystem.currentActionType = ActionType.Groom;
        gm.generalAnimationManager.activityTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    public void Sleep()
    {
        gm.actionSystem.currentActionType = ActionType.Sleep;
        gm.generalAnimationManager.activityTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    public void Pet()
    {
        gm.actionSystem.currentActionType = ActionType.Pet;
        gm.generalAnimationManager.activityTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    // ------------------------------------------------

    public void PerformSitTrick()
    {
        gm.tricksSystem.currentTrickType = TrickType.Sit;
        gm.generalAnimationManager.trickTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    public void PerformStayTrick()
    {
        gm.tricksSystem.currentTrickType = TrickType.Stay;
        gm.generalAnimationManager.trickTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    public void PerformComeTrick()
    {
        gm.tricksSystem.currentTrickType = TrickType.Come;
        gm.generalAnimationManager.trickTriggered = true;

        gm.activitySetupEvent.actionPlaying = true;
        gm.activitySetupEvent.activitySetupAnimator.Play("hide", 0, 0f);
    }

    private void SetPointerRotationFromMood(float moodScore)
    {
        if (pointerBase == null) return;

        float t = Mathf.Clamp01(moodScore / 100f);
        float angle = Mathf.Lerp(20f, -140f, t);

        Vector3 e = pointerBase.localEulerAngles;
        pointerBase.localEulerAngles = new Vector3(e.x, e.y, angle);
    }
}