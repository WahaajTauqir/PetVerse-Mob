using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Firebase.Database;
using Firebase.Extensions;
using System;

public class MemoryJournal : MonoBehaviour
{
    [SerializeField] GeneralManager gm;
    [SerializeField] GameObject journalEntryPrefab;
    [SerializeField] GameObject memoryJournalContent;

    public string DateFormateConverter(string isoDate)
    {
        if (string.IsNullOrEmpty(isoDate)) return "";
        if (DateTime.TryParse(isoDate, out DateTime dt))
        {
            return dt.ToString("dd MMM yyyy, HH:mm");
        }
        return isoDate;
    }

    public void LoadMemoryJournalEntries()
    {
        string userId = PlayerPrefs.GetString("FirebaseUserId", "");
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is null or empty. Cannot load memory journal entries.");
            return;
        }

        string databaseUrl = gm.firebaseDataManager.databaseUrl;
        var app = Firebase.FirebaseApp.DefaultInstance;
        DatabaseReference journalRef = FirebaseDatabase.GetInstance(app, databaseUrl)
            .GetReference(userId)
            .Child("memoryJournal");

        journalRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to get memory journal entries: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            List<DataSnapshot> entries = new List<DataSnapshot>();
            foreach (var entry in snapshot.Children)
            {
                entries.Add(entry);
            }

            entries.Sort((a, b) => int.Parse(b.Key).CompareTo(int.Parse(a.Key)));

            foreach (var entry in entries)
            {
                string date = entry.HasChild("date") ? entry.Child("date").Value?.ToString() : "";
                string data = entry.HasChild("data") ? entry.Child("data").Value?.ToString() : "";
                string formattedDate = DateFormateConverter(date);

                GameObject go = Instantiate(journalEntryPrefab, memoryJournalContent.transform);
                Transform dateTime = go.transform.Find("DateTime");
                Transform dataObj = go.transform.Find("Data");
                if (dateTime != null)
                {
                    Transform dateTimeText = dateTime.Find("DateTimeText");
                    if (dateTimeText != null)
                    {
                        var tmp = dateTimeText.GetComponent<TMPro.TMP_Text>();
                        if (tmp != null) tmp.text = formattedDate;
                    }
                }
                if (dataObj != null)
                {
                    Transform dataText = dataObj.Find("DataText");
                    if (dataText != null)
                    {
                        var tmp = dataText.GetComponent<TMPro.TMP_Text>();
                        if (tmp != null) tmp.text = data;
                    }
                }
            }
        });
    }
}
