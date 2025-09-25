using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneralManager : MonoBehaviour
{
    public EmotionSystem emotionSystem;
    public NeedsSystem needsSystem;
    public PersonalitySystem personalitySystem;
    public BondingSystem bondingSystem;
    public MoodSystem moodSystem;
    public FirebaseAuthManager firebaseAuthManager; 
    public FirebaseDataManager firebaseDataManager;
    public FirebaseDataManager firebaseManager;
    public InteractionMaster interactionMaster;
    public ActionSystem actionSystem;
    public TrickSystem tricksSystem;
    public MemoryJournal memoryJournal;
    public GeneralAnimatorManager generalAnimationManager;
    public PlaySequenceHandler playSequenceHandler;

    // THE MAIN FLOW OF THE PET
    // ---------------------------------------------------------------------------------------------------+
    private void ApplyTimePassedEffects(TimeSpan elapsedTime)
    {
        emotionSystem.ApplyElapsedEmotions((float)elapsedTime.TotalHours);
        needsSystem.ApplyElapsedNeeds((float)elapsedTime.TotalHours);
        personalitySystem.ApplyElapsedPersonality((float)elapsedTime.TotalHours);
        bondingSystem.ApplyElapsedBonding((float)elapsedTime.TotalHours);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        needsSystem.UpdateNeeds(deltaTime);

        emotionSystem.UpdateEmotions(deltaTime);

        moodSystem.CalculateMoodScore();

        personalitySystem.UpdatePersonality();

        bondingSystem.UpdateBondLevel();

        generalAnimationManager.UpdateEmotion();

        interactionMaster.UpdateSliders();
    }

    // THE MAIN FLOW OF THE PET'S LIFE CYCLE
    // ---------------------------------------------------------------------------------------------------+
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            emotionSystem.SaveEmotions();
            needsSystem.SaveNeeds();

            personalitySystem.SavePersonality();
            PlayerPrefs.SetString("LastActiveTime", DateTime.Now.ToString("o"));
            PlayerPrefs.Save();
        }
        else
        {
            ProcessElapsedTime();
        }
    }

    private void OnApplicationQuit()
    {
        emotionSystem.SaveEmotions();
        needsSystem.SaveNeeds();
        personalitySystem.SavePersonality();

        PlayerPrefs.SetString("LastActiveTime", DateTime.Now.ToString("o"));
        PlayerPrefs.Save();
    }

    private void ProcessElapsedTime()
    {
        if (PlayerPrefs.HasKey("LastActiveTime"))
        {
            string lastActiveTimeStr = PlayerPrefs.GetString("LastActiveTime");
            DateTime lastActiveTime = DateTime.Parse(lastActiveTimeStr);
            TimeSpan elapsedTime = DateTime.Now - lastActiveTime;

            ApplyTimePassedEffects(elapsedTime);
        }
    }

    public float GetHunger() { return needsSystem.GetFood(); }
    public float GetEnergy() { return needsSystem.GetEnergy(); }
    public float GetHygiene() { return needsSystem.GetHygiene(); }
    public float GetHappiness() { return needsSystem.GetWellbeing(); }
    public float GetMoodScore() { return moodSystem.GetMoodScore(); }
}
