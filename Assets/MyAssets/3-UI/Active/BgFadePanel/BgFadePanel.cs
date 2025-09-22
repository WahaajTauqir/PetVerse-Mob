using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgFadePanel : MonoBehaviour
{
    [SerializeField] OverlayPanelSetup overlayPanelSetup;

    public void CallOnAnimationEnds()
    {
        overlayPanelSetup.OnAnimationEnds();
    }
}
