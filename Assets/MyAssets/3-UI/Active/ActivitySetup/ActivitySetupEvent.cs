using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivitySetupEvent : MonoBehaviour
{
    [SerializeField] ActivitySetup activitySetup;

    public void CallOnAnimationEnds()
    {
        activitySetup.OnAnimationEnds();
    }
}
