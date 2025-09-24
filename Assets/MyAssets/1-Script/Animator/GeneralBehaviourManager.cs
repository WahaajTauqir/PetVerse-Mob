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
    }

    // -------------------------- Helper Methods --------------------------
    void MainLoop(Animator animator, AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("StandMainLoop"))
        {
            if (generalAnimatorManager.gm.needsSystem.GetEnergy() < 60 && generalAnimatorManager.gm.needsSystem.GetEnergy() >= 30)
            {
                animator.SetTrigger("ToSitStart");
            }
            else if (generalAnimatorManager.gm.needsSystem.GetEnergy() < 30)
            {
                animator.SetTrigger("ToLieStart");
            }

            generalAnimatorManager.StandMainLoop();
        }

        if (stateInfo.IsName("SitMainLoop"))
        {
            if (generalAnimatorManager.gm.needsSystem.GetEnergy() >= 60)
            {
                animator.SetTrigger("ToStandStart");
                animator.SetTrigger("ToSitStart");
            }
            else if (generalAnimatorManager.gm.needsSystem.GetEnergy() < 30)
            {
                animator.SetTrigger("ToStandStart");
            }

            generalAnimatorManager.SitMainLoop();
        }

        if (stateInfo.IsName("LieMainLoop"))
        {
            if (generalAnimatorManager.gm.needsSystem.GetEnergy() >= 60)
            {
                animator.SetTrigger("ToStandStart");
            }
            if (generalAnimatorManager.gm.needsSystem.GetEnergy() < 60 && generalAnimatorManager.gm.needsSystem.GetEnergy() >= 30)
            {
                animator.SetTrigger("ToStandStart");
                animator.SetTrigger("ToLieStart");
            }

            generalAnimatorManager.LieMainLoop();
        }
    }
}
