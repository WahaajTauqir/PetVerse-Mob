using System;
using System.Collections.Generic;
using UnityEngine;

public enum EmotionType
{
    Happy, 
    Excited,
    Curious
}

// Individual emotion with properties for managing its behavior over time
// --------------------------------------------------------------------------------------------------
[System.Serializable]
public class Emotion
{
    [Range(0, 100)] public float happy = 0f;
    [Range(0, 100)] public float excited = 0f;
    [Range(0, 100)] public float curious = 0f;

    [Header("Emotions Decay Rates")]
    public float happyDecay = 3f;
    public float excitedDecay = 5f;
    public float curiousDecay = 6f;

    public void DecayEmotions(float deltaTime, float factor)
    {
        happy = Mathf.Max(happy - (happyDecay * deltaTime * factor), 0f);
        excited = Mathf.Max(excited - (excitedDecay * deltaTime * factor), 0f);
        curious = Mathf.Max(curious - (curiousDecay * deltaTime * factor), 0f);
        ClampAll();
    }

    public void DecayEmotionsForElapsed(float hours, float factor)
    {
        happy = Mathf.Max(happy - (happyDecay * hours * factor), 0f); 
        excited = Mathf.Max(excited - (excitedDecay * hours * factor), 0f);
        curious = Mathf.Max(curious - (curiousDecay * hours * factor), 0f); 
        ClampAll();
    }

    public void ClampAll()
    {
        happy = Mathf.Clamp(happy, 0f, 100f);
        excited = Mathf.Clamp(excited, 0f, 100f);
        curious = Mathf.Clamp(curious, 0f, 100f);
    }
}
// ---------------------------------------------------------------------------------------------------

public class EmotionSystem : MonoBehaviour
{
    [SerializeField] GeneralManager gm;

    [Header("Dominant Emotion")]
    [SerializeField] private EmotionType dominantEmotion = EmotionType.Happy;

    [Header("Emotions")]
    public Emotion emotions = new Emotion();

    [Header("Decay Factors")]
    [SerializeField] private float activeGameFactor = 1.0f;
    [SerializeField] private float inactiveGameFactor = 1.0f;

    [Header("Interaction Factors")]
    [SerializeField] private float interactionFactor = 1.0f;


    private void Awake()
    {
        LoadEmotions();
    }

    public void SaveEmotions()
    {
        PlayerPrefs.SetFloat("Emotion_Happy", emotions.happy);
        PlayerPrefs.SetFloat("Emotion_Excited", emotions.excited);
        PlayerPrefs.SetFloat("Emotion_Curious", emotions.curious);
        PlayerPrefs.Save();
    }

    public void ApplyElapsedEmotions(float elapsedHours)
    {
        emotions.DecayEmotionsForElapsed(elapsedHours, inactiveGameFactor);

        SaveEmotions();
    }

    private void LoadEmotions()
    {
        emotions.happy = PlayerPrefs.GetFloat("Emotion_Happy", emotions.happy);
        emotions.excited = PlayerPrefs.GetFloat("Emotion_Excited", emotions.excited);
        emotions.curious = PlayerPrefs.GetFloat("Emotion_Curious", emotions.curious);
        PlayerPrefs.Save();
    }
    // --------------------------------------------------------------------------------------------------

    public void UpdateEmotions(float deltaTime)
    {
        emotions.DecayEmotions(deltaTime, activeGameFactor);
        CalculateDominantEmotion();
    }
    // --------------------------------------------------------------------------------------------------

    public void CalculateDominantEmotion()
    {
        float highestValue = float.MinValue;
        EmotionType highest = dominantEmotion;
        if (emotions.happy > highestValue)
        {
            highestValue = emotions.happy;
            highest = EmotionType.Happy;
        }
        if (emotions.excited > highestValue)
        {
            highestValue = emotions.excited;
            highest = EmotionType.Excited;
        }
        if (emotions.curious > highestValue)
        {
            highestValue = emotions.curious;
            highest = EmotionType.Curious;
        }
        dominantEmotion = highest;
    }
    // --------------------------------------------------------------------------------------------------

    public EmotionType GetDominantEmotion()
    {
        return dominantEmotion;
    }
    // --------------------------------------------------------------------------------------------------

    public void ProcessEmotion(ActionType type)
    {
        switch (type)
        {
            case ActionType.Feed:
                emotions.happy += 1f * interactionFactor;
                emotions.excited += 1f * interactionFactor;
                emotions.curious += 0f * interactionFactor;
                break;

            case ActionType.Play:
                emotions.happy += 4f * interactionFactor;
                emotions.excited += 5f * interactionFactor;
                emotions.curious += 1f * interactionFactor;
                break;

            case ActionType.Groom:
                emotions.happy += 1f * interactionFactor;
                emotions.excited -= 1f * interactionFactor;
                emotions.curious += 0f * interactionFactor;
                break;

            case ActionType.Sleep:
                emotions.happy += 1f * interactionFactor;
                emotions.excited -= 2f * interactionFactor;
                emotions.curious += 2f * interactionFactor;
                break;

            case ActionType.Pet:
                emotions.happy += 4f * interactionFactor;
                emotions.excited += 2f * interactionFactor;
                emotions.curious += 0f * interactionFactor;
                break;
        }

        emotions.ClampAll();
        CalculateDominantEmotion();
        gm.moodSystem.CalculateMoodScore();
    }

    // --------------------------------------------------------------------------------------------------
    public float GetEmotionValue(string name)
    {
        switch (name)
        {
            case "Happy": return emotions.happy;
            case "Excited": return emotions.excited;
            case "Curious": return emotions.curious;
            default: return 0f;
        }
    }
}