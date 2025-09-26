using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OpenPanel
{
    Journal,
    Settings,
    Profile
}

public class OverlayPanelSetup : MonoBehaviour
{
    GeneralManager gm;
    OpenPanel openPanel;

    [SerializeField] GameObject bgFadePanel;
    [SerializeField] GameObject journalObj;
    [SerializeField] GameObject settingsObj;
    [SerializeField] GameObject profileObj;

    Animator bgFadePanelAnimator;
    CanvasGroup bgFadePanelCanvasGroup;

    void Start()
    {
        gm = GameObject.FindWithTag("gm").GetComponent<GeneralManager>();

        bgFadePanelAnimator = bgFadePanel.GetComponent<Animator>();

        bgFadePanelCanvasGroup = bgFadePanel.GetComponent<CanvasGroup>();

        bgFadePanelCanvasGroup.interactable = false;
    }

    public void OpenJournal()
    {
        openPanel = OpenPanel.Journal;
        bgFadePanelAnimator.Play("show", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = true;
    }

    public void OpenSettings()
    {
        openPanel = OpenPanel.Settings;
        bgFadePanelAnimator.Play("show", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = true;
    }

    public void OpenProfile()
    {
        openPanel = OpenPanel.Profile;
        bgFadePanelAnimator.Play("show", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = true;
    }

    public void OnAnimationEnds()
    {
        if (openPanel == OpenPanel.Journal)
        {
            journalObj.SetActive(true);

            gm.memoryJournal.LoadMemoryJournalEntries();
        }
        else if (openPanel == OpenPanel.Settings)
        {
            settingsObj.SetActive(true);
        }
        else if (openPanel == OpenPanel.Profile)
        {
            profileObj.SetActive(true);
        }
    }

    public void CloseJournal()
    {
        journalObj.SetActive(false);
        bgFadePanelAnimator.Play("hide", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = false;
    }

    public void CloseSettings()
    {
        settingsObj.SetActive(false);
        bgFadePanelAnimator.Play("hide", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = false;
    }

    public void CloseProfile()
    {
        profileObj.SetActive(false);
        bgFadePanelAnimator.Play("hide", 0, 0f);
        bgFadePanelCanvasGroup.blocksRaycasts = false;
    }
}
