using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using System.Threading.Tasks;
using System.ComponentModel.Design;

public class FirebaseDataManager : MonoBehaviour
{
    private DatabaseReference databaseReference;
    [SerializeField] GeneralManager gm;
    public string databaseUrl;
    private bool firebaseReady = false;
    
    // Throttling/Debounce controls
    [Header("Performance")]
    [SerializeField] private float favouriteUpdateDebounceSeconds = 1.0f;
    [SerializeField] private float trickSaveDebounceSeconds = 0.5f;
    [SerializeField] private bool verboseLogging = false;
    private Coroutine favUpdateCoroutine;
    private bool firstFeedCheckedThisSession = false;
    private readonly Dictionary<TrickType, float> lastFirstLearnCheckTime = new Dictionary<TrickType, float>();
    private bool favUpdateInProgress = false;

    // Debounced trick save state
    private class TrickPending
    {
        public string UserId;
        public string TrickType;
        public bool Learned;
        public int MasteryLevel;
        public int TimesPerformed;
    }
    private readonly Dictionary<string, TrickPending> pendingTrickSaves = new Dictionary<string, TrickPending>();
    private readonly Dictionary<string, Coroutine> trickSaveCoroutines = new Dictionary<string, Coroutine>();
    private void Awake()
    {
        if (!Application.isPlaying) return;

        if (gm == null)
        {
            Debug.LogError("GeneralManager reference not set in FirebaseDataManager.");
            return;
        }

        StartCoroutine(WaitForFirebaseAndInit());
    }

    private IEnumerator WaitForFirebaseAndInit()
    {

        float timeout = 10f;
        float timer = 0f;
        while ((gm.firebaseAuthManager == null || gm.firebaseAuthManager.auth == null) && timer < timeout)
        {
            gm.firebaseAuthManager = FindObjectOfType<FirebaseAuthManager>();
            timer += Time.deltaTime;
            yield return null;
        }

        if (gm.firebaseAuthManager == null || gm.firebaseAuthManager.auth == null)
        {
            Debug.LogError("FirebaseAuthManager or its auth is not initialized after waiting.");
            yield break;
        }

        FirebaseApp app = FirebaseApp.DefaultInstance;
        databaseUrl = "https://petverse-1ed94-default-rtdb.asia-southeast1.firebasedatabase.app/";
        databaseReference = FirebaseDatabase.GetInstance(app, databaseUrl).RootReference;
        firebaseReady = true;
    }

    private void Start()
    {

    }

    private string GetUserId()
    {
        if (gm != null && gm.firebaseAuthManager != null)
        {
            return gm.firebaseAuthManager.GetCurrentUserId();
        }
        else
        {
            return PlayerPrefs.GetString("FirebaseUserId", "");
        }
    }

    public void FirebaseSaveMemoryJournalEntry(string date, string data)
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot save memory journal entry.");
            return;
        }

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot save memory journal entry.");
            return;
        }

        DatabaseReference journalRef = databaseReference.Child(userId).Child("memoryJournal");

        journalRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get memoryJournal entries: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            int nextEntry = 1;
            if (snapshot.Exists && snapshot.ChildrenCount > 0)
            {
                foreach (var child in snapshot.Children)
                {
                    if (int.TryParse(child.Key, out int entryNum))
                    {
                        if (entryNum >= nextEntry)
                            nextEntry = entryNum + 1;
                    }
                }
            }

            var entryData = new Dictionary<string, object>
            {
                { "date", date },
                { "data", data }
            };
            journalRef.Child(nextEntry.ToString()).SetValueAsync(entryData).ContinueWithOnMainThread(setTask =>
            {
                if (setTask.IsFaulted || setTask.IsCanceled)
                {
                    Debug.LogError("Failed to save memory journal entry: " + setTask.Exception);
                }
                else
                {
                    Debug.Log($"Memory journal entry {nextEntry} saved for user {userId}.");
                }
            });
        });
    }

    public void CheckAndSaveFirstFeed()
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot check first feed.");
            return;
        }

    // Prevent duplicate checks in a single session to avoid spamming
    if (firstFeedCheckedThisSession) return;
    firstFeedCheckedThisSession = true;

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot check first feed.");
            return;
        }

        DatabaseReference feedRef = databaseReference.Child(userId).Child("actions").Child("feed").Child("timesPerformed");

        feedRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get feed timesPerformed: " + task.Exception);
                return;
            }

            int timesPerformed = 0;
            if (task.Result.Exists && task.Result.Value != null)
            {
                int.TryParse(task.Result.Value.ToString(), out timesPerformed);
            }

            if (timesPerformed < 1)
            {
                string entryDate = System.DateTime.Now.ToString("o");
                string entryData = "First feed event";
                FirebaseSaveMemoryJournalEntry(entryDate, entryData);
            }
        });
    }

    public void FirebaseSaveTrickData(string userId, string trickType, bool learned, int masteryLevel, int timesPerformed)
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot save trick data.");
            return;
        }
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot save trick data.");
            return;
        }

        string key = userId + ":" + trickType;
        pendingTrickSaves[key] = new TrickPending
        {
            UserId = userId,
            TrickType = trickType,
            Learned = learned,
            MasteryLevel = masteryLevel,
            TimesPerformed = timesPerformed
        };

        if (!trickSaveCoroutines.ContainsKey(key) || trickSaveCoroutines[key] == null)
        {
            trickSaveCoroutines[key] = StartCoroutine(DebouncedTrickSave(key));
        }
    }

    public void FirebaseSaveActionData(ActionType actionType)
    {
        string userId = GetUserId();

        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot save action data.");
            return;
        }
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot save action data.");
            return;
        }

        DatabaseReference actionRef = databaseReference.Child(userId).Child("actions").Child(actionType.ToString().ToLower()).Child("timesPerformed");
        // Use a transaction to avoid separate get+set and to be robust under rapid calls
    actionRef.RunTransaction(mutableData =>
        {
            try
            {
                int current = 0;
                if (mutableData.Value != null)
                {
                    // Firebase stores numbers as long/double - handle both
                    if (mutableData.Value is long l) current = (int)l;
                    else if (mutableData.Value is double d) current = (int)d;
                    else int.TryParse(mutableData.Value.ToString(), out current);
                }
                current++;
                mutableData.Value = current;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Transaction parse error: " + ex.Message);
                mutableData.Value = 1;
            }
            return TransactionResult.Success(mutableData);
        }).ContinueWithOnMainThread(txTask =>
        {
            if (txTask.IsFaulted || txTask.IsCanceled)
            {
                Debug.LogError($"Failed to save action data for {actionType} via transaction: " + txTask.Exception);
            }
            else
            {
        if (verboseLogging)
            Debug.Log($"Action data for {actionType} incremented for user {userId}.");
            }
        });
    }

    public void FirebaseFirstTrickLearned(TrickType trickType)
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot check first trick learned.");
            return;
        }

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot check first trick learned.");
            return;
        }

        // Throttle checks per trick to avoid repeated reads when spamming the action
        float now = Time.realtimeSinceStartup;
        if (lastFirstLearnCheckTime.TryGetValue(trickType, out float last) && (now - last) < 2f)
        {
            return;
        }
        lastFirstLearnCheckTime[trickType] = now;

        DatabaseReference trickRef = databaseReference.Child(userId).Child("tricks").Child(trickType.ToString()).Child("learned");

        trickRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get trick learned status: " + task.Exception);
                return;
            }

            bool isLearned = false;
            if (task.Result.Exists && task.Result.Value != null)
            {
                bool.TryParse(task.Result.Value.ToString(), out isLearned);
            }

            if (!isLearned)
            {
                string entryDate = System.DateTime.Now.ToString("o");
                string entryData = $"First trick learned: {trickType}";
                FirebaseSaveMemoryJournalEntry(entryDate, entryData);
            }
        });
    }

    public void FirebaseUpdateBondStage(float bondLevel)
    {
        if (!firebaseReady)
        {
            Debug.Log("Firebase is not ready. Cannot update bond level.");
            return;
        }

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.Log("User ID is null or empty. Cannot update bond level.");
            return;
        }

        DatabaseReference bondRef = databaseReference.Child(userId).Child("bondLevel");
        bondRef.SetValueAsync((int)bondLevel).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to update bond level: " + task.Exception);
            }
            else
            {
                Debug.Log($"Bond level updated to {bondLevel} for user {userId}.");
            }
        });
    }

    public void FirebaseAddBondChangeInJournal(BondStage newStage)
    {
        if (!firebaseReady)
        {
            Debug.Log("Firebase is not ready. Cannot add bond change to journal.");
            return;
        }

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.Log("User ID is null or empty. Cannot add bond change to journal.");
            return;
        }

        string entryDate = System.DateTime.Now.ToString("o");
        string entryData = $"Bond stage changed to: {newStage}";
        FirebaseSaveMemoryJournalEntry(entryDate, entryData);
    }

    public void FirebaseUpdateFavouriteActivity()
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase is not ready. Cannot update favourite activity.");
            return;
        }

        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot update favourite activity.");
            return;
        }

        if (favUpdateInProgress) return;
        favUpdateInProgress = true;

        DatabaseReference actionsRef = databaseReference.Child(userId).Child("actions");
        DatabaseReference tricksRef = databaseReference.Child(userId).Child("tricks");

        Task<DataSnapshot> actionsTask = actionsRef.GetValueAsync();
        Task<DataSnapshot> tricksTask = tricksRef.GetValueAsync();

        Task.WhenAll(actionsTask, tricksTask).ContinueWithOnMainThread(task =>
        {
            favUpdateInProgress = false;
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get actions/tricks for favourite activity: " + task.Exception);
                return;
            }
            DataSnapshot actionsSnap = actionsTask.Result;
            DataSnapshot tricksSnap = tricksTask.Result;
            string favName = "";
            int favCount = -1;

            // Check actions
            if (actionsSnap != null && actionsSnap.Exists)
            {
                foreach (var action in actionsSnap.Children)
                {
                    if (action.HasChild("timesPerformed"))
                    {
                        int count = 0;
                        int.TryParse(action.Child("timesPerformed").Value?.ToString(), out count);

                        if (count > favCount)
                        {
                            favCount = count;
                            favName = action.Key;
                        }
                    }
                }
            }

            // Check tricks
            if (tricksSnap != null && tricksSnap.Exists)
            {
                foreach (var trick in tricksSnap.Children)
                {
                    if (trick.HasChild("timesPerformed"))
                    {
                        int count = 0;
                        int.TryParse(trick.Child("timesPerformed").Value?.ToString(), out count);

                        if (count > favCount)
                        {
                            favCount = count;
                            favName = trick.Key;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(favName) && favCount >= 0)
            {
                DatabaseReference favActivityRef = databaseReference.Child(userId).Child("favouriteActivity");
                favActivityRef.GetValueAsync().ContinueWithOnMainThread(getTask =>
                {
                    if (getTask.IsFaulted || getTask.IsCanceled)
                    {
                        Debug.LogError("Failed to retrieve current favourite activity: " + getTask.Exception);
                        return;
                    }

                    string currentFavActivity = getTask.Result?.Value?.ToString();

                    if (currentFavActivity != favName)
                    {
                        favActivityRef.SetValueAsync(favName).ContinueWithOnMainThread(setTask =>
                        {
                            if (setTask.IsFaulted || setTask.IsCanceled)
                            {
                                Debug.LogError("Failed to update favourite activity: " + setTask.Exception);
                            }
                            else
                            {
                                if (verboseLogging)
                                    Debug.Log($"Favourite activity updated to {favName} with {favCount} times performed for user {userId}.");

                                // Log the change in the memory journal
                                string entryDate = System.DateTime.Now.ToString("o");
                                string entryData = $"Favourite activity changed to: {favName}";
                                FirebaseSaveMemoryJournalEntry(entryDate, entryData);
                            }
                        });
                    }
                });
            }
            else
            {
                Debug.LogWarning("No valid favourite activity found to update.");
            }
        });
    }

    // Debounced entry point to update favourite activity. Coalesces rapid calls.
    public void RequestUpdateFavouriteActivity()
    {
        if (!firebaseReady) return;
        if (favUpdateCoroutine != null) return; // already scheduled
        favUpdateCoroutine = StartCoroutine(DebouncedFavouriteUpdate());
    }

    private IEnumerator DebouncedFavouriteUpdate()
    {
        yield return new WaitForSeconds(favouriteUpdateDebounceSeconds);
        favUpdateCoroutine = null;
        FirebaseUpdateFavouriteActivity();
    }

    private IEnumerator DebouncedTrickSave(string key)
    {
        yield return new WaitForSeconds(trickSaveDebounceSeconds);
        if (!pendingTrickSaves.TryGetValue(key, out var pending))
        {
            trickSaveCoroutines[key] = null;
            yield break;
        }

        var trickRef = databaseReference.Child(pending.UserId).Child("tricks").Child(pending.TrickType);
        var trickData = new Dictionary<string, object>
        {
            { "learned", pending.Learned },
            { "masteryLevel", pending.MasteryLevel },
            { "timesPerformed", pending.TimesPerformed }
        };

        var setTask = trickRef.SetValueAsync(trickData);
        yield return new WaitUntil(() => setTask.IsCompleted);
        if (setTask.IsFaulted || setTask.IsCanceled)
        {
            Debug.LogError($"Failed to save trick data for {pending.TrickType}: " + setTask.Exception);
        }
        else if (verboseLogging)
        {
            Debug.Log($"Trick data for {pending.TrickType} saved for user {pending.UserId}.");
        }

        pendingTrickSaves.Remove(key);
        trickSaveCoroutines[key] = null;
    }

    public DatabaseReference GetDatabaseReference()
    {
        return databaseReference;
    }
}