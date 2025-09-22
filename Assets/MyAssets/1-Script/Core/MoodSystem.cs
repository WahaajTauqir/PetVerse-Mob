using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodSystem : MonoBehaviour
{
    [SerializeField] GeneralManager gm;

    [Header("Mood Score")]
    [Range(0, 100)] public float moodScore = 50f;

    // -------------------------------------------------------------------------------------------------
    public void CalculateMoodScore()
    {
        float happyWeight = 0.2f;
        float excitedWeight = 0.25f;
        float curiousWeight = 0.05f;

        float happy = gm.emotionSystem.emotions.happy;
        float excited = gm.emotionSystem.emotions.excited;
        float curious = gm.emotionSystem.emotions.curious;

        float foodWeight = 0.125f;
        float energyWeight = 0.125f;
        float hygieneWeight = 0.05f;
        float wellbeingWeight = 0.2f;

        float food = gm.needsSystem.needs.food;
        float energy = gm.needsSystem.needs.energy;
        float hygiene = gm.needsSystem.needs.hygiene;
        float wellbeing = gm.needsSystem.needs.wellbeing;

        float weightedSum = (happy * happyWeight) + (excited * excitedWeight)
        + (curious * curiousWeight) + (food * foodWeight) + (energy * energyWeight) + (hygiene * hygieneWeight) + (wellbeing * wellbeingWeight);
        moodScore = Mathf.Clamp(weightedSum, 0f, 100f);
    }
    
    public float GetMoodScore()
    {
        return moodScore;
    }
}
