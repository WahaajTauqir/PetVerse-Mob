using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    void Start()
    {
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
    }

}
