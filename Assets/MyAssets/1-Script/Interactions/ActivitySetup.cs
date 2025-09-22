using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ActivityStatus
{
    Actions,
    Tricks
}

public class ActivitySetup : MonoBehaviour
{
    ActivityStatus status = ActivityStatus.Actions;

    [SerializeField] GameObject actionsObj;
    [SerializeField] GameObject tricksObj;

    [SerializeField] GameObject clickPreventor;

    Animator actionsAnimator;
    Animator tricksAnimator;

    CanvasGroup actionsCanvasGroup;
    CanvasGroup tricksCanvasGroup;

    void Start()
    {
        clickPreventor.SetActive(false);

        actionsAnimator = actionsObj.GetComponent<Animator>();
        tricksAnimator = tricksObj.GetComponent<Animator>();

        actionsCanvasGroup = actionsObj.GetComponent<CanvasGroup>();
        tricksCanvasGroup = tricksObj.GetComponent<CanvasGroup>();
    }

    public void OnClickModeSwitcher()
    {
        clickPreventor.SetActive(true);

        if (status == ActivityStatus.Actions)
        {
            status = ActivityStatus.Tricks;
            actionsAnimator.Play("hide", 0, 0f);
        }
        else if (status == ActivityStatus.Tricks)
        {
            status = ActivityStatus.Actions;
            tricksAnimator.Play("hide", 0, 0f);
        }
        else
        {
            return;
        } 
    }

    public void OnAnimationEnds()
    {
        if (status == ActivityStatus.Actions)
        {
            actionsCanvasGroup.alpha = 1f;
            actionsCanvasGroup.interactable = true;

            tricksCanvasGroup.alpha = 0f;
            tricksCanvasGroup.interactable = false;

            actionsAnimator.Play("show", 0, 0f); 
        }
        else if (status == ActivityStatus.Tricks)  
        {
            tricksCanvasGroup.alpha = 1f;
            tricksCanvasGroup.interactable = true;

            actionsCanvasGroup.alpha = 0f;
            actionsCanvasGroup.interactable = false;

            tricksAnimator.Play("show", 0, 0f);
        }
        else
        {
            return;
        }

        clickPreventor.SetActive(false);
    }
}