using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralBehaviourManager : StateMachineBehaviour
{
    [Header("Default Idle")]
    int idleChoice;
    [SerializeField] int defaultIdleCount;

    GeneralAnimatorManager generalAnimatorManager;

    // ------------------------------------------------------------------------------
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        generalAnimatorManager = animator.GetComponent<GeneralAnimatorManager>();
        
        MainLoop(animator, stateInfo);

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

    // -------------------------- Helper Methods --------------------------
    void MainLoop(Animator animator, AnimatorStateInfo stateInfo)
    {
        // MainLoop logic (no debug output)
        if (stateInfo.IsName("StandMainLoop"))
        {
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
        }

        if (stateInfo.IsName("SitMainLoop"))
        {
            generalAnimatorManager.SitMainLoop();

            float energy = generalAnimatorManager.gm.needsSystem.GetEnergy();

            if (energy >= 60)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToSitStart);
            }
            else if (energy < 30)
            {
                generalAnimatorManager.TriggerReset();
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
            }
        }

        if (stateInfo.IsName("LieMainLoop"))
        {
            generalAnimatorManager.LieMainLoop();

            float energy = generalAnimatorManager.gm.needsSystem.GetEnergy();

            if (energy >= 60)
            {
                generalAnimatorManager.TriggerReset();
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
            }
            if (energy < 60 && energy >= 30)
            {
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToStandStart);
                generalAnimatorManager.SetIdleTrigger(IdleTrigger.ToLieStart);
            }
        }
    }
}
