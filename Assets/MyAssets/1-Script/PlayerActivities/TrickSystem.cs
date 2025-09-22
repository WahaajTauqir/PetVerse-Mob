using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;

using JetBrains.Annotations;
using UnityEngine.UI;

public enum TrickType
{
    Sit,
    Stay,
    Come
}

public class Trick
{
    public TrickType trickType;
    public int timesPerformed = 0;
    public int timesPerformedLastHour = 0;
    [Range(0, 100)] public float masteryLevel = 0;
    public bool isLearned = false;
    public DateTime lastPerformedTime = DateTime.MinValue;
}

public class TrickSystem : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;

    public List<Trick> tricks = new List<Trick>();

    [SerializeField] GameObject sittingSlider;
    [SerializeField] Slider sittingMasteryLevelSlider;
    [SerializeField] Animator animator;

    private void Awake()
    {
        InitializeTricks();
        //LoadCountsFromPrefs();
    }

    private void InitializeTricks()
    {
        tricks.Clear();
        foreach (TrickType trickType in Enum.GetValues(typeof(TrickType)))
        {
            tricks.Add(new Trick { trickType = trickType });
        }
    }

    // -----------------------------------------------------------------------------------------------
    public void ProcessTrick(TrickType trickType)
    {
        Debug.Log($"Processing trick: {trickType}");
        var trick = tricks.Find(t => t.trickType == trickType);
        if (trick == null) return;

        string userId = gm != null && gm.firebaseAuthManager != null ? gm.firebaseAuthManager.GetCurrentUserId() : PlayerPrefs.GetString("FirebaseUserId", "");
        if (string.IsNullOrEmpty(userId) || gm == null || gm.firebaseDataManager == null)
        {
            Debug.LogError("GeneralManager, FirebaseDataManager, or UserId missing.");
            return;
        }
        DatabaseReference trickRef = gm.firebaseDataManager.GetDatabaseReference().Child(userId).Child("tricks").Child(trickType.ToString());

        // Strategy: attempt to use local cached state; if it's the first time (unknown), perform a one-time load
        // Avoid repeated GetValueAsync on every button press.
        void ApplyAndPersist()
        {
            if (!trick.isLearned)
            {
                trick.masteryLevel += 10f;
                if (trick.masteryLevel >= 100f)
                {
                    trick.masteryLevel = 100f;
                    trick.isLearned = true;
                    gm.firebaseDataManager.FirebaseFirstTrickLearned(trickType);
                }
            }
            else
            {
                trick.timesPerformed++;
            }

            gm.firebaseDataManager.FirebaseSaveTrickData(userId, trickType.ToString(), trick.isLearned, (int)trick.masteryLevel, trick.timesPerformed);
            gm.firebaseDataManager.RequestUpdateFavouriteActivity();

            if (trickType == TrickType.Sit)
            {
                ShowTrick();
            }
        }

        // If we have never loaded this trick from the server and local values are default, perform a one-time fetch.
        bool needsInitialLoad = trick.timesPerformed == 0 && Mathf.Approximately(trick.masteryLevel, 0f) && !trick.isLearned;
        if (needsInitialLoad)
        {
            trickRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled && task.Result.Exists)
                {
                    var existingData = task.Result.Value as Dictionary<string, object>;
                    if (existingData != null)
                    {
                        if (existingData.ContainsKey("timesPerformed"))
                        {
                            try { trick.timesPerformed = Convert.ToInt32(existingData["timesPerformed"]); } catch { }
                        }
                        if (existingData.ContainsKey("masteryLevel"))
                        {
                            try { trick.masteryLevel = Convert.ToSingle(existingData["masteryLevel"]); } catch { }
                        }
                        if (existingData.ContainsKey("learned"))
                        {
                            try { trick.isLearned = Convert.ToBoolean(existingData["learned"]); } catch { }
                        }
                    }
                }
                else if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"Initial trick load failed for {trickType}: " + task.Exception);
                }

                ApplyAndPersist();
            });
        }
        else
        {
            ApplyAndPersist();
        }
    }

    public void ShowTrick()
    {
        var sitTrick = tricks.Find(t => t.trickType == TrickType.Sit);
        if (sitTrick == null) return;

        if (sitTrick.isLearned)
        {
            sittingSlider.SetActive(false);
            animator.SetTrigger("ToSitStart");
        }
        else
        {
            sittingSlider.SetActive(true);
            sittingMasteryLevelSlider.value = sitTrick.masteryLevel;
        }
    }
    // Non Production Code
    // -----------------------------------------------------------------------------------------------

    private void OnApplicationQuit()
    {
        //SaveCountsToPrefs();
    }

    // private void SaveCountsToPrefs()
    // {
    //     foreach (var trick in tricks)
    //     {
    //         string key = $"Trick_{trick.trickType}_Total";
    //         PlayerPrefs.SetInt(key, trick.timesPerformed);
    //     }
    //     PlayerPrefs.Save();
    // }

    // private void LoadCountsFromPrefs()
    // {
    //     foreach (var trick in tricks)
    //     {
    //         string key = $"Trick_{trick.trickType}_Total";
    //         trick.timesPerformed = PlayerPrefs.GetInt(key, 0);
    //     }
    // }

    // public void RecordTrick(TrickType trickType)
    // {
    //     var trick = tricks.Find(t => t.trickType.Equals(trickType));
    //     if (trick == null) return;

    //     trick.timesPerformed++;

    //     if ((DateTime.UtcNow - trick.lastPerformedTime).TotalHours >= 1)
    //         trick.timesPerformedLastHour = 1;
    //     else
    //         trick.timesPerformedLastHour++;

    //     trick.lastPerformedTime = DateTime.UtcNow;

    //     string key = $"Trick_{trick.trickType}_Total";
    //     PlayerPrefs.SetInt(key, trick.timesPerformed);
    //     PlayerPrefs.Save();
    // }

    // public (int total, int lastHour) GetTrickCounts(TrickType trickType)
    // {
    //     var trick = tricks.Find(t => t.trickType.Equals(trickType));

    //     if (trick == null) return (0, 0);

    //     int lastHour = 0;

    //     if ((DateTime.UtcNow - trick.lastPerformedTime).TotalHours < 1)
    //         lastHour = trick.timesPerformedLastHour;
    //     return (trick.timesPerformed, lastHour);
    // }
}
