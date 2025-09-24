using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;

public enum IdleTrigger
{
    ToStandStart,
    ToSitStart,
    ToLieStart,
    //--------------
    StandHygieneLow,
    StandFoodLow,
    StandExcitedLow,
    StandExcitedHighStart_Bark,
    StandExcitedHighCrouchBark_TailWag,
    StandCuriousHigh_WatchLeftRight,
    StandCuriousHigh_HeadTilt,
    StandCuriousHigh_Wonder,
    SitCuriousHigh,
    LieEnergyLow_HeadDown,
    LieCuriousHighStart_HeadUp,
    LieSleepy
}

public class GeneralAnimatorManager : MonoBehaviour
{
    public IdleTrigger lastTrigger;

    public Animator animator;
    public GeneralManager gm;
    public bool[] idleTriggerStates;

    // Queue-based trigger system
    private Queue<IdleTrigger> triggerQueue;
    public int MaxQueueSize = 10; // Prevent infinite queue growth

    void Awake()
    {
        idleTriggerStates = new bool[System.Enum.GetValues(typeof(IdleTrigger)).Length];
        triggerQueue = new Queue<IdleTrigger>();
    }

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
        // StandMainLoop logic

        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.emotionSystem.GetEmotionValue("Excited") > 70)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighStart_Bark);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 80)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighCrouchBark_TailWag);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 30)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedLow);
            }

            // Moderate curiosity threshold
            if (gm.emotionSystem.GetEmotionValue("Curious") > 50)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_WatchLeftRight);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 60)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_HeadTilt);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 70)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_Wonder);
            }

            if (gm.needsSystem.GetFood() < 40)
            {
                SetIdleTrigger(IdleTrigger.StandFoodLow);
            }

            if (gm.needsSystem.GetHygiene() < 35)
            {
                SetIdleTrigger(IdleTrigger.StandHygieneLow);
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.emotionSystem.GetEmotionValue("Excited") > 50)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighStart_Bark);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 65)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighCrouchBark_TailWag);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 35)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedLow);
            }

            // Low curiosity threshold
            if (gm.emotionSystem.GetEmotionValue("Curious") > 30)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_WatchLeftRight);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_HeadTilt);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 50)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_Wonder);
            }

            if (gm.needsSystem.GetFood() < 50)
            {
                SetIdleTrigger(IdleTrigger.StandFoodLow);
            }

            if (gm.needsSystem.GetHygiene() < 45)
            {
                SetIdleTrigger(IdleTrigger.StandHygieneLow);
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.emotionSystem.GetEmotionValue("Excited") > 25)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighStart_Bark);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 40)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighCrouchBark_TailWag);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 20)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedLow);
            }

            // Low-moderate curiosity threshold
            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_WatchLeftRight);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 45)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_HeadTilt);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_Wonder);
            }

            if (gm.needsSystem.GetFood() < 60)
            {
                SetIdleTrigger(IdleTrigger.StandFoodLow);
            }

            if (gm.needsSystem.GetHygiene() < 40)
            {
                SetIdleTrigger(IdleTrigger.StandHygieneLow);
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            if (gm.emotionSystem.GetEmotionValue("Excited") > 40)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighStart_Bark);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") > 55)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedHighCrouchBark_TailWag);
            }
            if (gm.emotionSystem.GetEmotionValue("Excited") < 25)
            {
                SetIdleTrigger(IdleTrigger.StandExcitedLow);
            }

            // Moderate curiosity threshold
            if (gm.emotionSystem.GetEmotionValue("Curious") > 45)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_WatchLeftRight);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_HeadTilt);
            }
            if (gm.emotionSystem.GetEmotionValue("Curious") > 65)
            {
                SetIdleTrigger(IdleTrigger.StandCuriousHigh_Wonder);
            }

            if (gm.needsSystem.GetFood() < 45)
            {
                SetIdleTrigger(IdleTrigger.StandFoodLow);
            }

            if (gm.needsSystem.GetHygiene() < 50)
            {
                SetIdleTrigger(IdleTrigger.StandHygieneLow);
            }
        }
    }

    public void SitMainLoop()
    {
        // SitMainLoop logic

        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.emotionSystem.GetEmotionValue("Curious") > 60)
            {
                SetIdleTrigger(IdleTrigger.SitCuriousHigh);
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.emotionSystem.GetEmotionValue("Curious") > 25)
            {
                SetIdleTrigger(IdleTrigger.SitCuriousHigh);
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                SetIdleTrigger(IdleTrigger.SitCuriousHigh);
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                SetIdleTrigger(IdleTrigger.SitCuriousHigh);
            }
        }
    }

    public void LieMainLoop()
    {
        // LieMainLoop logic

        if (gm.personalitySystem.petPersonality == "shy")
        {
            if (gm.needsSystem.GetEnergy() < 50)
            {
                SetIdleTrigger(IdleTrigger.LieEnergyLow_HeadDown);
            }

            if (gm.emotionSystem.GetEmotionValue("Curious") > 55)
            {
                SetIdleTrigger(IdleTrigger.LieCuriousHighStart_HeadUp);
            }

            if (gm.emotionSystem.GetEmotionValue("Happy") < 45)
            {
                SetIdleTrigger(IdleTrigger.LieSleepy);
            }
        }
        else if (gm.personalitySystem.petPersonality == "curious")
        {
            if (gm.needsSystem.GetEnergy() < 35)
            {
                SetIdleTrigger(IdleTrigger.LieEnergyLow_HeadDown);
            }

            if (gm.emotionSystem.GetEmotionValue("Curious") > 30)
            {
                SetIdleTrigger(IdleTrigger.LieCuriousHighStart_HeadUp);
            }

            if (gm.emotionSystem.GetEmotionValue("Excited") < 30)
            {
                SetIdleTrigger(IdleTrigger.LieSleepy);
            }
        }
        else if (gm.personalitySystem.petPersonality == "playful")
        {
            if (gm.needsSystem.GetEnergy() < 30)
            {
                SetIdleTrigger(IdleTrigger.LieEnergyLow_HeadDown);
            }

            if (gm.emotionSystem.GetEmotionValue("Curious") > 40)
            {
                SetIdleTrigger(IdleTrigger.LieCuriousHighStart_HeadUp);
            }

            if (gm.emotionSystem.GetEmotionValue("Excited") < 25)
            {
                SetIdleTrigger(IdleTrigger.LieSleepy);
            }
        }
        else if (gm.personalitySystem.petPersonality == "affectionate")
        {
            if (gm.needsSystem.GetEnergy() < 40)
            {
                SetIdleTrigger(IdleTrigger.LieEnergyLow_HeadDown);
            }

            if (gm.emotionSystem.GetEmotionValue("Curious") > 35)
            {
                SetIdleTrigger(IdleTrigger.LieCuriousHighStart_HeadUp);
            }

            if (gm.emotionSystem.GetEmotionValue("Excited") < 35)
            {
                SetIdleTrigger(IdleTrigger.LieSleepy);
            }
        }
    }

    public void TriggerReset()
    {
        ClearTriggerQueue();
    }

    public void SetIdleTrigger(IdleTrigger trigger)
    {
        // bool isStateTransition = trigger == IdleTrigger.ToStandStart ||
        //                     trigger == IdleTrigger.ToSitStart ||
        //                     trigger == IdleTrigger.ToLieStart;

        // if (isStateTransition)
        // {
        //     if (triggerQueue.Count >= MaxQueueSize)
        //     {
        //         return;
        //     }

        //     triggerQueue.Enqueue(trigger);

        //     int index = (int)trigger;
        //     idleTriggerStates[index] = true;
        //     return;
        // }

        if (!triggerQueue.Contains(trigger))
        {
            if (triggerQueue.Count > 0)
            {
                IdleTrigger lastTriggerInQueue = triggerQueue.ToArray()[triggerQueue.Count - 1];
                if (lastTriggerInQueue == trigger)
                {
                    return;
                }
            }

            if (lastTrigger == trigger)
            {
                return;
            }

            if (triggerQueue.Count >= MaxQueueSize)
            {
                return;
            }

            triggerQueue.Enqueue(trigger);

            int index = (int)trigger;
            idleTriggerStates[index] = true;
        }
    }
    public IdleTrigger? ProcessNextTrigger()
    {
        if (triggerQueue.Count > 0)
        {
            IdleTrigger nextTrigger = triggerQueue.Dequeue();
            lastTrigger = nextTrigger;
            animator.SetTrigger(nextTrigger.ToString());
            return nextTrigger;
        }
        return null;
    }

    public void ClearTriggerQueue()
    {
        triggerQueue.Clear();
    }

    public int GetTriggerQueueCount()
    {
        return triggerQueue.Count;
    }

    public string GetTriggerQueueContents()
    {
        if (triggerQueue.Count == 0) return string.Empty;
        var arr = triggerQueue.ToArray();
        string[] strs = new string[arr.Length];
        for (int i = 0; i < arr.Length; i++) strs[i] = arr[i].ToString();
        return string.Join(", ", strs);
    }
}