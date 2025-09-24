using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;

// public enum ActionTrigger
// {
//     ToEatStart,
//     ToGroomStart,
//     ToPlayStart,
//     ToPetStart,
//     ToSleepStart,
// }
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

public enum EnergyTrigger
{

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
            // Shy: Needs higher excitement (70+) to get excited due to cautious nature
            if (gm.emotionSystem.GetEmotionValue("Excited") > 70)
            {
                animator.SetTrigger("StandExcitedHighStart(Bark)"); // Bark
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 80)
            {
                animator.SetTrigger("StandExcitedHighCrouchBark(TailWag)"); // TailWag
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 30)
            {
                animator.SetTrigger("StandExcitedLow");
            }

            // Shy: Moderate curiosity threshold (50+) - cautious exploration
            if (gm.emotionSystem.GetEmotionValue("Curious") > 50)
            {
                animator.SetTrigger("StandCuriousHigh(WatchLeftRight)"); // WatchLeftRight
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 60)
            {
                animator.SetTrigger("StandCuriousHigh(HeadTilt)"); // HeadTilt
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 70)
            {
                animator.SetTrigger("StandCuriousHigh(Wonder)"); // Wonder
            }

            if (gm.needsSystem.GetFood() < 40)
            {
                animator.SetTrigger("StandFoodLow");
            }

            if (gm.needsSystem.GetHygiene() < 35)
            {
                animator.SetTrigger("StandHygieneLow");
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            // Curious: Moderate excitement threshold (50+) - balanced explorer
            if (gm.emotionSystem.GetEmotionValue("Excited") > 50)
            {
                animator.SetTrigger("StandExcitedHighStart(Bark)"); // Bark
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 65)
            {
                animator.SetTrigger("StandExcitedHighCrouchBark(TailWag)"); // TailWag
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 35)
            {
                animator.SetTrigger("StandExcitedLow");
            }

            // Curious: Low curiosity threshold (30+) - naturally inquisitive
            if (gm.emotionSystem.GetEmotionValue("Curious") > 30)
            {
                animator.SetTrigger("StandCuriousHigh(WatchLeftRight)"); // WatchLeftRight
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                animator.SetTrigger("StandCuriousHigh(HeadTilt)"); // HeadTilt
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 50)
            {
                animator.SetTrigger("StandCuriousHigh(Wonder)"); // Wonder
            }

            if (gm.needsSystem.GetFood() < 50)
            {
                animator.SetTrigger("StandFoodLow");
            }

            if (gm.needsSystem.GetHygiene() < 45)
            {
                animator.SetTrigger("StandHygieneLow");
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            // Playful: Very low excitement threshold (25+) - gets excited easily
            if (gm.emotionSystem.GetEmotionValue("Excited") > 25)
            {
                animator.SetTrigger("StandExcitedHighStart(Bark)"); // Bark
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 40)
            {
                animator.SetTrigger("StandExcitedHighCrouchBark(TailWag)"); // TailWag
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 20)
            {
                animator.SetTrigger("StandExcitedLow");
            }

            // Playful: Low-moderate curiosity threshold (35+) - playful exploration
            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                animator.SetTrigger("StandCuriousHigh(WatchLeftRight)"); // WatchLeftRight
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 45)
            {
                animator.SetTrigger("StandCuriousHigh(HeadTilt)"); // HeadTilt
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                animator.SetTrigger("StandCuriousHigh(Wonder)"); // Wonder
            }

            if (gm.needsSystem.GetFood() < 60)
            {
                animator.SetTrigger("StandFoodLow");
            }

            if (gm.needsSystem.GetHygiene() < 40)
            {
                animator.SetTrigger("StandHygieneLow");
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            // Affectionate: Moderate-low excitement threshold (40+) - responds well to attention
            if (gm.emotionSystem.GetEmotionValue("Excited") > 40)
            {
                animator.SetTrigger("StandExcitedHighStart(Bark)"); // Bark
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 55)
            {
                animator.SetTrigger("StandExcitedHighCrouchBark(TailWag)"); // TailWag
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 25)
            {
                animator.SetTrigger("StandExcitedLow");
            }

            // Affectionate: Moderate curiosity threshold (45+) - socially curious
            if (gm.emotionSystem.GetEmotionValue("Curious") > 45)
            {
                animator.SetTrigger("StandCuriousHigh(WatchLeftRight)"); // WatchLeftRight
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                animator.SetTrigger("StandCuriousHigh(HeadTilt)"); // HeadTilt
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 65)
            {
                animator.SetTrigger("StandCuriousHigh(Wonder)"); // Wonder
            }

            if (gm.needsSystem.GetFood() < 45)
            {
                animator.SetTrigger("StandFoodLow");
            }

            if (gm.needsSystem.GetHygiene() < 50)
            {
                animator.SetTrigger("StandHygieneLow");
            }
        }
    }

    public void SitMainLoop()
    {
        if (gm.personalitySystem.petPersonality == "shy")
        {
            // Shy: Higher curiosity threshold (60+) when sitting - needs more stimulus to be curious while seated
            if (gm.emotionSystem.GetEmotionValue("Curious") > 60)
            {
                animator.SetTrigger("SitCuriousHigh");
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            // Curious: Low curiosity threshold (25+) when sitting - naturally curious even while sitting
            if (gm.emotionSystem.GetEmotionValue("Curious") > 25)
            {
                animator.SetTrigger("SitCuriousHigh");
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            // Playful: Moderate curiosity threshold (40+) when sitting - wants to play rather than sit
            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                animator.SetTrigger("SitCuriousHigh");
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            // Affectionate: Moderate-low curiosity threshold (35+) when sitting - curious about social interactions
            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                animator.SetTrigger("SitCuriousHigh");
            }
        }
    }

    public void LieMainLoop()
    {
        if (gm.personalitySystem.petPersonality == "shy")
        {
            // Shy: Moderate energy threshold (50) to show low energy - conserves energy
            if (gm.needsSystem.GetEnergy() < 50)
            {
                animator.SetTrigger("LieEnergyLow(HeadDown)"); // HeadDown
            }

            // Shy: Higher curiosity threshold (55+) when lying - needs more stimulus to lift head up
            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                animator.SetTrigger("LieCuriousHighStart(HeadUp)"); // HeadUp
            }

            // Shy: Moderate happy threshold (45) to feel sleepy - gets sad/sleepy easier
            if (gm.emotionSystem.GetEmotionValue("Happy") < 45)
            {
                animator.SetTrigger("LieSleepy");
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            // Curious: Lower energy threshold (35) to show low energy - uses energy exploring
            if (gm.needsSystem.GetEnergy() < 35)
            {
                animator.SetTrigger("LieEnergyLow(HeadDown)"); // HeadDown
            }

            // Curious: Low curiosity threshold (30+) when lying - easily curious even when resting
            if (gm.emotionSystem.GetEmotionValue("Curious") > 30)
            {
                animator.SetTrigger("LieCuriousHighStart(HeadUp)"); // HeadUp
            }

            // Curious: Lower excited threshold (30) to be sleepy - needs excitement to stay awake
            if (gm.emotionSystem.GetEmotionValue("Excited") < 30)
            {
                animator.SetTrigger("LieSleepy");
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            // Playful: Lower energy threshold (30) to show low energy - uses lots of energy playing
            if (gm.needsSystem.GetEnergy() < 30)
            {
                animator.SetTrigger("LieEnergyLow(HeadDown)"); // HeadDown
            }

            // Playful: Moderate curiosity threshold (40+) when lying - still playful even resting
            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                animator.SetTrigger("LieCuriousHighStart(HeadUp)"); // HeadUp
            }

            // Playful: Lower excited threshold (25) to be sleepy - crashes hard after playing
            if (gm.emotionSystem.GetEmotionValue("Excited") < 25)
            {
                animator.SetTrigger("LieSleepy");
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            // Affectionate: Moderate energy threshold (40) to show low energy - balanced energy use
            if (gm.needsSystem.GetEnergy() < 40)
            {
                animator.SetTrigger("LieEnergyLow(HeadDown)"); // HeadDown
            }

            // Affectionate: Moderate-low curiosity threshold (35+) when lying - socially curious
            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                animator.SetTrigger("LieCuriousHighStart(HeadUp)"); // HeadUp
            }

            // Affectionate: Moderate excited threshold (35) to be sleepy - content when resting
            if (gm.emotionSystem.GetEmotionValue("Excited") < 35)
            {
                animator.SetTrigger("LieSleepy");
            }
        }
    }

    public void TriggerReset()
    {
        // Reset TrickTrigger
        animator.ResetTrigger("ToSitStart");
        animator.ResetTrigger("ToStayStart");
        animator.ResetTrigger("ToComeStart");
        animator.ResetTrigger("ToActivity");

        // Reset ReactionTrigger
        animator.ResetTrigger("ToActivity");
        animator.ResetTrigger("ToNeglect");
        animator.ResetTrigger("ToCare");
        animator.ResetTrigger("ToNull");

        // Reset HeadMaskTrigger
        animator.ResetTrigger("ToEntryHead");
        animator.ResetTrigger("ToHeadTilt");
        animator.ResetTrigger("ToHeadWiggle");
        animator.ResetTrigger("ToBark");

        // Reset TailMaskTrigger
        animator.ResetTrigger("ToEntryTail");
        animator.ResetTrigger("ToStandTailWag");
        animator.ResetTrigger("ToSitTailWag");
    }
}