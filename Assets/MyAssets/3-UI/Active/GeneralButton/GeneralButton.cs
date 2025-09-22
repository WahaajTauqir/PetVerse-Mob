using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneralButton : MonoBehaviour
{
    GeneralManager gm;
    Animator animator;
    [SerializeField] UnityEvent onClick;
    
    void Start()
    {
        gm = GameObject.FindWithTag("gm").GetComponent<GeneralManager>();
        animator = GetComponent<Animator>();
    }
    
    public void OnClick()
    {
        gm.interactionMaster.uiAudioSource.clip = gm.interactionMaster.buttonClickAudio;
        gm.interactionMaster.uiAudioSource.Play();

        animator.speed = 1f;
        animator.Play("click", 0, 0f);

        onClick.Invoke();
    }

    public void OnClickAnimationEnds()
    {
        animator.speed = 0f;
    }
}
