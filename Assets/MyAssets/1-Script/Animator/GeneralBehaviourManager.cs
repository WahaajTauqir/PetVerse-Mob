using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBehaviourManager : StateMachineBehaviour
{
    [Header("Default Idle")]
    [SerializeField] int defaultIdleCount;
    GeneralAnimatorManager generalAnimatorManager;

    // ------------------------------------------------------------------------------
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        generalAnimatorManager = animator.GetComponent<GeneralAnimatorManager>();

        MainLoop(stateInfo);

        IdleTrigger? processedTrigger = generalAnimatorManager.ProcessNextTrigger();

        string contents = generalAnimatorManager.GetTriggerQueueContents();
        if (processedTrigger.HasValue)
        {
            Debug.Log($"Processed: {processedTrigger.Value} | Queue: {(string.IsNullOrEmpty(contents) ? "empty" : contents)}");
        }
        else
        {
            Debug.Log($"No trigger processed | Queue: {(string.IsNullOrEmpty(contents) ? "empty" : contents)}");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MainLoop(stateInfo);
    }

    // -------------------------- Helper Methods --------------------------
    void MainLoop(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("StandMainLoop"))
        {
            if (generalAnimatorManager.activityTriggered)
            {
                generalAnimatorManager.gm.actionSystem.ProcessAction(generalAnimatorManager.gm.actionSystem.currentActionType);
                generalAnimatorManager.activityTriggered = false;
            }

            generalAnimatorManager.StandMainLoop();

            float energy = generalAnimatorManager.gm.needsSystem.GetEnergy();

            if (energy < 60 && energy >= 30)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToSitStart);
            }
            else if (energy < 30)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToLieStart);
            }

            if (generalAnimatorManager.gm.moodSystem.GetMoodScore() >= 50)
            {
                generalAnimatorManager.animator.SetTrigger("ToStandTailWag");
            }
            else
            {
                generalAnimatorManager.animator.SetTrigger("ToEntryTail");
            }

            if (generalAnimatorManager.gm.moodSystem.GetMoodScore() < 20)
            {

            }
        }

        if (stateInfo.IsName("SitMainLoop"))
        {
            if (generalAnimatorManager.activityTriggered)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
                //generalAnimatorManager.activityTriggered = false; // Reset the flag
                
                // Process the trigger immediately
                IdleTrigger? processedTrigger = generalAnimatorManager.ProcessNextTrigger();
                if (processedTrigger.HasValue)
                {
                    Debug.Log($"Activity triggered: {processedTrigger.Value}");
                    return; // Exit early since we're transitioning states
                }
            }

            generalAnimatorManager.SitMainLoop();

            float energy = generalAnimatorManager.gm.needsSystem.GetEnergy();

            if (energy >= 60)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
            }
            else if (energy < 30)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToLieStart);
            }

            if (generalAnimatorManager.gm.moodSystem.GetMoodScore() >= 50)
            {
                generalAnimatorManager.animator.SetTrigger("ToSitTailWag");
            }
            else
            {
                generalAnimatorManager.animator.SetTrigger("ToEntryTail");
            }
        }

        if (stateInfo.IsName("LieMainLoop"))
        {
            if (generalAnimatorManager.activityTriggered)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
                //generalAnimatorManager.activityTriggered = false; // Reset the flag
                
                // Process the trigger immediately
                IdleTrigger? processedTrigger = generalAnimatorManager.ProcessNextTrigger();
                if (processedTrigger.HasValue)
                {
                    Debug.Log($"Activity triggered: {processedTrigger.Value}");
                    return; // Exit early since we're transitioning states
                }
            }

            generalAnimatorManager.LieMainLoop();

            float energy = generalAnimatorManager.gm.needsSystem.GetEnergy();

            if (energy >= 60)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
            }
            else if (energy < 60 && energy >= 30)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToSitStart);
            }
        }
    }
}
