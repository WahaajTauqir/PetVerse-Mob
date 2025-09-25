using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum ActionType
{
    Feed,
    Play,
    Groom,
    Sleep,
    Pet
}

public class Action
{
    public ActionType actionType;
    public int timesPerformed = 0;
    public int timesPerformedLastHour = 0;
    public DateTime lastPerformedTime = DateTime.MinValue;
}

public class ActionSystem : MonoBehaviour
{

    [SerializeField] GeneralManager gm;

    private List<Action> actions = new List<Action>();

    public ActionType currentActionType;

    public GameObject sleepSequence;
    public GameObject playMiddleSequence;
    public GameObject petSequence;
    public GameObject feedSequence;
    public GameObject groomSequence;

    [SerializeField] GameObject menuPanel;

    private void Awake()
    {
        InitializeActions();
        LoadCountsFromPrefs();
    }

    private void InitializeActions()
    {
        actions.Clear();
        foreach (ActionType actionType in Enum.GetValues(typeof(ActionType)))
        {
            actions.Add(new Action { actionType = actionType });
        }
    }

    public void ProcessAction(ActionType action)
    {
        if (action != ActionType.Feed)
        {
            gm.needsSystem.ProcessNeeds(action);
            gm.emotionSystem.ProcessEmotion(action);
        }
        
        if (action == ActionType.Feed)
        {
            gm.firebaseDataManager.CheckAndSaveFirstFeed();
            //feedSequence.SetActive(true);

            menuPanel.SetActive(true);
        }

        if (action == ActionType.Play)
        {
            gm.playSequenceHandler.PlayRandomSequence();
            // playMiddleSequence.SetActive(true);
        }

        if (action == ActionType.Groom)
        {
            groomSequence.SetActive(true);
        }

        if (action == ActionType.Sleep)
        {
            sleepSequence.SetActive(true);
        }

        if (action == ActionType.Pet)
        {
            petSequence.SetActive(true);
        }

        gm.firebaseDataManager.FirebaseSaveActionData(action);
        gm.firebaseDataManager.RequestUpdateFavouriteActivity();
    }

    // -----------------------------------------------------------------------------------------------
    private void OnApplicationQuit()
    {
        SaveCountsToPrefs();
    }


    private void SaveCountsToPrefs()
    {
        foreach (var action in actions)
        {
            string key = $"Action_{action.actionType}_Total";
            PlayerPrefs.SetInt(key, action.timesPerformed);
        }
        PlayerPrefs.Save();
    }


    private void LoadCountsFromPrefs()
    {
        foreach (var action in actions)
        {
            string key = $"Action_{action.actionType}_Total";
            action.timesPerformed = PlayerPrefs.GetInt(key, 0);
        }
    }


    public void RecordAction(ActionType actionType)
    {
        var action = actions.Find(a => a.actionType == actionType);

        if (action == null) return;

        action.timesPerformed++;

        if ((DateTime.UtcNow - action.lastPerformedTime).TotalHours >= 1)
            action.timesPerformedLastHour = 1;
        else
            action.timesPerformedLastHour++;
        action.lastPerformedTime = DateTime.UtcNow;

        string key = $"Action_{action.actionType}_Total";
        PlayerPrefs.SetInt(key, action.timesPerformed);
        PlayerPrefs.Save();
    }

    public (int total, int lastHour) GetActionCounts(ActionType actionType)
    {
        var action = actions.Find(a => a.actionType == actionType);

        if (action == null) return (0, 0);

        int lastHour = 0;

        if ((DateTime.UtcNow - action.lastPerformedTime).TotalHours < 1)
            lastHour = action.timesPerformedLastHour;
        return (action.timesPerformed, lastHour);
    }
}
