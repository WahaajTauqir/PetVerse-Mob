using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;

public enum ActionTrigger
{
    ToEatStart,
    ToGroomStart,
    ToPlayStart,
    ToPetStart,
    ToSleepStart,
}
public enum TrickTrigger
{
    ToSitStart,
    ToStayStart,
    ToComeStart,
    ToActivity,
}

public enum ReactionTrigger
{
    ToActivity,
    ToNeglect,
    ToCare,
    ToNull
}

public enum HeadMaskTrigger
{
    ToEntryHead,
    ToHeadTilt,
    ToHeadWiggle,
    ToBark
}

public enum TailMaskTrigger
{
    ToEntryTail,
    ToStandTailWag,
    ToSitTailWag
}

public class GeneralAnimatorManager : MonoBehaviour
{
    public Animator animator;
    public GeneralManager gm;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateEmotion()
    {
        float happyValue = gm.emotionSystem.GetEmotionValue("Happy");
        float sadBlendShapeValue = Mathf.Clamp(100 - happyValue, 0, 100);

        int sadBlendShapeIndex = 0;

        SkinnedMeshRenderer skinnedMeshRenderer = animator.GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedMeshRenderer.SetBlendShapeWeight(sadBlendShapeIndex, sadBlendShapeValue);
    }

    public void StandMainLoop()
    {
        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
    }

    public void SitMainLoop()
    {
        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {

            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
    }

    public void LieMainLoop()
    {
        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {

            if (gm.moodSystem.GetMoodScore() >= 50f)
            {

            }
            else
            {

            }
        }
    }
}