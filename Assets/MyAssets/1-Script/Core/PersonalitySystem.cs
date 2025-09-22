using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Personality
{
    [Range(0, 100)] public float shy = 50f;
    [Range(0, 100)] public float curious = 50f;
    [Range(0, 100)] public float playful = 50f;
    [Range(0, 100)] public float affectionate = 50f;
}

public class PersonalitySystem : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;

    [Header("Pet Personality")]
    public string petPersonality = "affectionate";

    private float shyTimer = 0f;
    private float notShyTimer = 0f;
    private float curiousTimer = 0f;
    private float notCuriousTimer = 0f;
    private float playfulTimer = 0f;
    private float notPlayfulTimer = 0f;
    private float affectionateTimer = 0f;
    private float notAffectionateTimer = 0f;

    [SerializeField] private Personality personality = new Personality();

    [Header("Thresholds")]
    [SerializeField] private float threshold = 50f;
    [SerializeField] private float duration = 3f;

    private void Awake()
    {
        LoadPersonality();
    }

    public void SavePersonality()
    {
        PlayerPrefs.SetFloat("Personality_Shy", personality.shy);
        PlayerPrefs.SetFloat("Personality_Curious", personality.curious);
        PlayerPrefs.SetFloat("Personality_Playful", personality.playful);
        PlayerPrefs.SetFloat("Personality_Affectionate", personality.affectionate);
        PlayerPrefs.Save();
    }

    public void LoadPersonality()
    {
        personality.shy = PlayerPrefs.GetFloat("Personality_Shy", personality.shy);
        personality.curious = PlayerPrefs.GetFloat("Personality_Curious", personality.curious);
        personality.playful = PlayerPrefs.GetFloat("Personality_Playful", personality.playful);
        personality.affectionate = PlayerPrefs.GetFloat("Personality_Affectionate", personality.affectionate);
    }

    public void ApplyElapsedPersonality(float elapsedTime)
    {
        float hours = elapsedTime;

        float shyDecay = Mathf.Abs(gm.emotionSystem.GetEmotionValue("Happy") - threshold) * hours;
        float curiousDecay = Mathf.Abs(gm.emotionSystem.GetEmotionValue("Curious") - threshold) * hours;
        float playfulDecay = Mathf.Abs(gm.emotionSystem.GetEmotionValue("Excited") - threshold) * hours;
        float affectionateDecay = Mathf.Abs(gm.emotionSystem.GetEmotionValue("Affectionate") - threshold) * hours;

        personality.shy = Mathf.Clamp(personality.shy - shyDecay, 0f, 100f);
        personality.curious = Mathf.Clamp(personality.curious - curiousDecay, 0f, 100f);
        personality.playful = Mathf.Clamp(personality.playful - playfulDecay, 0f, 100f);
        personality.affectionate = Mathf.Clamp(personality.affectionate - affectionateDecay, 0f, 100f);
        SavePersonality();
    }

    public void UpdatePersonality()
    {
        float dt = Time.deltaTime;

        UpdateShy(dt);
        UpdateCurious(dt);
        UpdatePlayful(dt);
        UpdateAffectionate(dt);

        UpdatePetPersonality();

        SavePersonality();
    }

    private void UpdatePetPersonality()
    {
        float maxValue = Mathf.Max(personality.shy, personality.curious, personality.playful, personality.affectionate);
        if (maxValue == personality.shy)
            petPersonality = "shy";
        else if (maxValue == personality.curious)
            petPersonality = "curious";
        else if (maxValue == personality.playful)
            petPersonality = "playful";
        else if (maxValue == personality.affectionate)
            petPersonality = "affectionate";
    }

    private void UpdateShy(float dt)
    {
        bool allBelowShy = gm.emotionSystem.GetEmotionValue("Happy") < threshold &&
                           gm.emotionSystem.GetEmotionValue("Excited") < threshold; //&&
                                                                                    //emotionSystem.GetEmotionValue("Affectionate") < threshold;

        bool allAboveShy = gm.emotionSystem.GetEmotionValue("Happy") > threshold &&
                           gm.emotionSystem.GetEmotionValue("Excited") > threshold; //&&
                                                                                    //emotionSystem.GetEmotionValue("Affectionate") > threshold;

        if (allBelowShy)
        {
            shyTimer += dt;
            notShyTimer = 0f;
            if (shyTimer >= duration)
            {
                personality.shy = Mathf.Clamp(personality.shy + 1f, 0f, 100f);
                shyTimer = 0f;
            }
        }
        else if (allAboveShy)
        {
            notShyTimer += dt;
            shyTimer = 0f;
            if (notShyTimer >= duration)
            {
                personality.shy = Mathf.Clamp(personality.shy - 1f, 0f, 100f);
                notShyTimer = 0f;
            }
        }
        else
        {
            shyTimer = 0f;
            notShyTimer = 0f;
        }
    }

    private void UpdateCurious(float dt)
    {
        float curiousValue = gm.emotionSystem.GetEmotionValue("Curious");
        if (curiousValue > threshold)
        {
            curiousTimer += dt;
            notCuriousTimer = 0f;
            if (curiousTimer >= duration)
            {
                personality.curious = Mathf.Clamp(personality.curious + 1f, 0f, 100f);
                curiousTimer = 0f;
            }
        }
        else if (curiousValue < threshold)
        {
            notCuriousTimer += dt;
            curiousTimer = 0f;
            if (notCuriousTimer >= duration)
            {
                personality.curious = Mathf.Clamp(personality.curious - 1f, 0f, 100f);
                notCuriousTimer = 0f;
            }
        }
        else
        {
            curiousTimer = 0f;
            notCuriousTimer = 0f;
        }
    }

    private void UpdatePlayful(float dt)
    {
        bool bothAbovePlayful = gm.emotionSystem.GetEmotionValue("Happy") > threshold &&
                                gm.emotionSystem.GetEmotionValue("Excited") > threshold;

        bool bothBelowPlayful = gm.emotionSystem.GetEmotionValue("Happy") < threshold &&
                                gm.emotionSystem.GetEmotionValue("Excited") < threshold;

        if (bothAbovePlayful)
        {
            playfulTimer += dt;
            notPlayfulTimer = 0f;
            if (playfulTimer >= duration)
            {
                personality.playful = Mathf.Clamp(personality.playful + 1f, 0f, 100f);
                playfulTimer = 0f;
            }
        }
        else if (bothBelowPlayful)
        {
            notPlayfulTimer += dt;
            playfulTimer = 0f;
            if (notPlayfulTimer >= duration)
            {
                personality.playful = Mathf.Clamp(personality.playful - 1f, 0f, 100f);
                notPlayfulTimer = 0f;
            }
        }
        else
        {
            playfulTimer = 0f;
            notPlayfulTimer = 0f;
        }
    }

    private void UpdateAffectionate(float dt)
    {
        bool allAboveAffectionate = gm.emotionSystem.GetEmotionValue("Happy") > threshold &&
                                    gm.emotionSystem.GetEmotionValue("Excited") > threshold; //&&
                                                                                             //gm.emotionSystem.GetEmotionValue("Affectionate") > threshold;

        bool allBelowAffectionate = gm.emotionSystem.GetEmotionValue("Happy") < threshold &&
                                    gm.emotionSystem.GetEmotionValue("Excited") < threshold; //&&
                                                                                             //gm.emotionSystem.GetEmotionValue("Affectionate") < threshold;

        if (allAboveAffectionate)
        {
            affectionateTimer += dt;
            notAffectionateTimer = 0f;
            if (affectionateTimer >= duration)
            {
                personality.affectionate = Mathf.Clamp(personality.affectionate + 1f, 0f, 100f);
                affectionateTimer = 0f;
            }
        }
        else if (allBelowAffectionate)
        {
            notAffectionateTimer += dt;
            affectionateTimer = 0f;
            if (notAffectionateTimer >= duration)
            {
                personality.affectionate = Mathf.Clamp(personality.affectionate - 1f, 0f, 100f);
                notAffectionateTimer = 0f;
            }
        }
        else
        {
            affectionateTimer = 0f;
            notAffectionateTimer = 0f;
        }
    }

    public string GetPetPersonality()
    {
        return petPersonality;
    }
}