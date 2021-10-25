using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Notifications;
using static SharedClass.Constant;

namespace SharedClass
{
    public class Notification
    {
        LocalSettings LocalSettings;

        public Notification()
        {
            LocalSettings = new LocalSettings();
        }

        public void ScheduleNotification()
        {
            if (!LocalSettings.IsTimerStarted)
            {
                return;
            }
            int numberOfNotification = 0;
            DateTime lastNotificationScheduledDateTime;
            DateTime targetNotificationScheduledDateTime;
            DateTime generateFromDateTime = DateTime.Now;
            if (LocalSettings.Notifications.Count > 0)
            {
                lastNotificationScheduledDateTime = LocalSettings.Notifications[LocalSettings.Notifications.Count - 1].ScheduledDateTime;
                if (LocalSettings.IntervalMin >= 15)
                {
                    targetNotificationScheduledDateTime = LocalSettings.Notifications[0].ScheduledDateTime.AddMinutes(LocalSettings.IntervalMin * 2);
                }
                else if (LocalSettings.IntervalMin > 0 && LocalSettings.IntervalMin < 15)
                {
                    targetNotificationScheduledDateTime = LocalSettings.Notifications[0].ScheduledDateTime.AddMinutes(15 / LocalSettings.IntervalMin * 2);
                }
                else
                {
                    // IntervalMin <= 0
                    return;
                }
                // decide schedule how many notification
                // by finding out the time of 2 notification ahead
                if (lastNotificationScheduledDateTime.CompareTo(targetNotificationScheduledDateTime) >= 0)
                {
                    // target scheduled met, no need to schedule
                }
                else
                {
                    generateFromDateTime = lastNotificationScheduledDateTime;
                    DateTime temp = lastNotificationScheduledDateTime;
                    while (temp.CompareTo(targetNotificationScheduledDateTime) < 0)
                    {
                        numberOfNotification++;
                        temp = temp.AddMinutes(LocalSettings.IntervalMin);
                    }
                }

            }
            else
            {
                if (LocalSettings.IntervalMin >= 15)
                {
                    // generate 2 notification ahead
                    numberOfNotification = 2;
                }
                else
                {
                    numberOfNotification = 15 / LocalSettings.IntervalMin * 2;
                }
            }

            var notifications = new List<NotificationModel>();
            for (int i = 0; i < numberOfNotification; i++)
            {
                var guid = Guid.NewGuid().ToString();
                generateFromDateTime = generateFromDateTime.AddMinutes(LocalSettings.IntervalMin);
                if (
                    LocalSettings.NotificationMode == NotificationModeEnum.Schedule &&
                    (TimeSpan.Compare(generateFromDateTime.TimeOfDay, (TimeSpan)LocalSettings.StartTime) < 0 ||
                    TimeSpan.Compare(generateFromDateTime.TimeOfDay, (TimeSpan)LocalSettings.EndTime) > 0))
                {
                    continue; // skip schedule when not in schedule period
                }
                notifications.Add(new NotificationModel() { 
                    ScheduledDateTime = generateFromDateTime,
                    Tag = guid
                });
                switch (LocalSettings.Action)
                {
                    case Actions.Notification:
                        SilentNotificationTemplate
                            .AddText(LocalSettings.NotificationText)
                            .Schedule(generateFromDateTime, toast =>
                            {
                                toast.Tag = guid;
                                toast.ExpirationTime = generateFromDateTime.AddSeconds(5);
                            });
                        break;
                    case Actions.NotificationAndSound:
                        DefaultNotificationTemplate
                            .AddText(LocalSettings.NotificationText)
                            .Schedule(generateFromDateTime, toast =>
                            {
                                toast.Tag = guid;
                                toast.ExpirationTime = generateFromDateTime.AddSeconds(5);
                            });
                        break;
                    default:
                        break;
                }
            }
            LocalSettings.Notifications = notifications;
        }

        public void RescheduleNotification()
        {
            RemoveExpiredNotification();
            RemoveScheduledNotification();
            if (LocalSettings.IsTimerStarted)
            {
                ScheduleNotification();
            }
        }

        public void RemoveExpiredNotification()
        {
            var notification = LocalSettings.Notifications;
            foreach (var n in notification.ToList())
            {
                if (DateTime.Now.CompareTo(n.ScheduledDateTime) >= 0)
                {
                    notification.Remove(n);
                }
            }
            LocalSettings.Notifications = notification;
        }

        public void RemoveScheduledNotification()
        {
            // Create the toast notifier
            ToastNotifierCompat notifier = ToastNotificationManagerCompat.CreateToastNotifier();

            // Get the list of scheduled toasts that haven't appeared yet
            IReadOnlyList<ScheduledToastNotification> scheduledToasts = notifier.GetScheduledToastNotifications();

            //foreach (var notification in Notifications.ToList())
            //{
            //    // Find our scheduled toast we want to cancel
            //    var toRemove = scheduledToasts.FirstOrDefault(i => i.Tag == notification.Tag);
            //    if (toRemove != null)
            //    {
            //        // And remove it from the schedule
            //        notifier.RemoveFromSchedule(toRemove);
            //        Notifications.Remove(notification);
            //    }
            //}

            foreach (var item in scheduledToasts)
            {
                notifier.RemoveFromSchedule(item);
            }

            LocalSettings.Notifications = new List<NotificationModel>();
        }
    }
}
