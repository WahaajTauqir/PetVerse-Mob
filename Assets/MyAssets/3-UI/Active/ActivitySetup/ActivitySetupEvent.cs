using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitySetupEvent : MonoBehaviour
{
    public bool actionPlaying = false;
    [SerializeField] ActivitySetup activitySetup;
    public Animator activitySetupAnimator;

    public void CallOnAnimationEnds()
    {
        if (!actionPlaying)
        {
            activitySetup.OnAnimationEnds();
        }
    }
}
