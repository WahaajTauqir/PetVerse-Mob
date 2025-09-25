using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using System;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GeneralManager gm;
    
    [Header("Notification Settings")]
    [SerializeField] private float lowMoodThreshold = 30f;
    [SerializeField] private float inactivityHours = 3f;
    [SerializeField] private float checkInterval = 300f; // Check every 5 minutes
    
    private string notificationChannelId = "pet_notifications";
    private DateTime lastPlayTime;
    private bool isInitialized = false;
    
    // Notification identifiers
    private const int LOW_MOOD_NOTIFICATION_ID = 1001;
    private const int INACTIVITY_NOTIFICATION_ID = 1002;
    void Start()
    {
        InitializeNotificationSystem();
        
        // Test notification code - commented out
        /*
        Debug.Log("[NotificationManager] Start called.");
        var channel = new AndroidNotificationChannel()
        {
            Id = "pet_notifications",
            Name = "Pet Notifications",
            Importance = Importance.Default,
            Description = "Notifications for pet events",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        Debug.Log("[NotificationManager] Notification channel registered: " + channel.Id);

        var notification = new AndroidNotification();
        notification.Title = "Feed Tommy";
        notification.Text = "Tommy is hungry, feed him!";
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";

        notification.FireTime = System.DateTime.Now.AddSeconds(30);
        Debug.Log($"[NotificationManager] Scheduling notification: Title='{notification.Title}', Text='{notification.Text}', FireTime={notification.FireTime}");

        AndroidNotificationCenter.SendNotification(notification, "pet_notifications");
        Debug.Log("[NotificationManager] Notification sent to channel: pet_notifications");
        */
    }

    private void InitializeNotificationSystem()
    {
        Debug.Log("[NotificationManager] Initializing notification system.");
        
        // Register notification channel
        var channel = new AndroidNotificationChannel()
        {
            Id = notificationChannelId,
            Name = "Pet Care Notifications",
            Importance = Importance.High,
            Description = "Notifications for pet mood and activity reminders",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        Debug.Log($"[NotificationManager] Notification channel registered: {notificationChannelId}");

        // Load last play time from PlayerPrefs
        string lastPlayTimeString = PlayerPrefs.GetString("LastPlayTime", DateTime.Now.ToBinary().ToString());
        long lastPlayTimeBinary = Convert.ToInt64(lastPlayTimeString);
        lastPlayTime = DateTime.FromBinary(lastPlayTimeBinary);
        
        isInitialized = true;
        
        // Start the notification checking coroutine
        StartCoroutine(CheckNotificationConditions());
    }

    private IEnumerator CheckNotificationConditions()
    {
        while (isInitialized)
        {
            yield return new WaitForSeconds(checkInterval);
            
            if (gm != null)
            {
                CheckLowMoodNotification();
                CheckInactivityNotification();
            }
        }
    }

    private void CheckLowMoodNotification()
    {
        float currentMoodScore = gm.moodSystem.GetMoodScore();
        
        if (currentMoodScore <= lowMoodThreshold)
        {
            // Cancel any existing low mood notifications
            AndroidNotificationCenter.CancelNotification(LOW_MOOD_NOTIFICATION_ID);
            
            var notification = new AndroidNotification();
            notification.Title = "Your pet needs attention!";
            notification.Text = $"Your pet's mood is low ({currentMoodScore:F1}). Play with them to cheer them up!";
            notification.SmallIcon = "icon";
            notification.LargeIcon = "logo";
            notification.FireTime = DateTime.Now.AddMinutes(1);
            
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, notificationChannelId, LOW_MOOD_NOTIFICATION_ID);
            Debug.Log($"[NotificationManager] Low mood notification scheduled. Mood: {currentMoodScore:F1}");
        }
    }

    private void CheckInactivityNotification()
    {
        TimeSpan timeSinceLastPlay = DateTime.Now - lastPlayTime;
        double hoursSinceLastPlay = timeSinceLastPlay.TotalHours;
        
        if (hoursSinceLastPlay >= inactivityHours)
        {
            // Cancel any existing inactivity notifications
            AndroidNotificationCenter.CancelNotification(INACTIVITY_NOTIFICATION_ID);
            
            var notification = new AndroidNotification();
            notification.Title = "Your pet misses you!";
            notification.Text = $"It's been {hoursSinceLastPlay:F1} hours since you last played. Your pet is waiting for you!";
            notification.SmallIcon = "icon";
            notification.LargeIcon = "logo";
            notification.FireTime = DateTime.Now.AddMinutes(1);
            
            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, notificationChannelId, INACTIVITY_NOTIFICATION_ID);
            Debug.Log($"[NotificationManager] Inactivity notification scheduled. Hours since last play: {hoursSinceLastPlay:F1}");
        }
    }

    // Call this method when the player interacts with the pet (plays, feeds, etc.)
    public void UpdateLastPlayTime()
    {
        lastPlayTime = DateTime.Now;
        PlayerPrefs.SetString("LastPlayTime", lastPlayTime.ToBinary().ToString());
        PlayerPrefs.Save();
        Debug.Log($"[NotificationManager] Last play time updated: {lastPlayTime}");
        
        // Cancel inactivity notifications since player is active
        AndroidNotificationCenter.CancelNotification(INACTIVITY_NOTIFICATION_ID);
    }

    // Method to schedule a custom notification
    public void ScheduleCustomNotification(string title, string text, int delayInMinutes)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.SmallIcon = "icon";
        notification.LargeIcon = "logo";
        notification.FireTime = DateTime.Now.AddMinutes(delayInMinutes);
        
        AndroidNotificationCenter.SendNotification(notification, notificationChannelId);
        Debug.Log($"[NotificationManager] Custom notification scheduled: {title} in {delayInMinutes} minutes");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App is being paused/minimized
            UpdateLastPlayTime();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // App is losing focus
            UpdateLastPlayTime();
        }
    }

}
