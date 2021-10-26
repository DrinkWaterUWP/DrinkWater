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

            var notifications = LocalSettings.Notifications;
            var intervalMin = LocalSettings.IntervalMin;
            var startTime = LocalSettings.StartTime;
            var endTime = LocalSettings.EndTime;
            var notificationMode = LocalSettings.NotificationMode;
            var action = LocalSettings.Action;
            var notificationText = LocalSettings.NotificationText;

            if (notifications.Count > 0)
            {
                lastNotificationScheduledDateTime = notifications[notifications.Count - 1].ScheduledDateTime;
                if (intervalMin >= 15)
                {
                    targetNotificationScheduledDateTime = notifications[0].ScheduledDateTime.AddMinutes(intervalMin * 2);
                }
                else if (intervalMin > 0 && intervalMin < 15)
                {
                    targetNotificationScheduledDateTime = notifications[0].ScheduledDateTime.AddMinutes(15 / intervalMin * 2);
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
                    return;
                }
                else
                {
                    generateFromDateTime = lastNotificationScheduledDateTime;
                    DateTime temp = lastNotificationScheduledDateTime;
                    while (temp.CompareTo(targetNotificationScheduledDateTime) < 0)
                    {
                        numberOfNotification++;
                        temp = temp.AddMinutes(intervalMin);
                    }
                }

            }
            else
            {
                if (intervalMin >= 15)
                {
                    // generate 2 notification ahead
                    numberOfNotification = 2;
                }
                else
                {
                    numberOfNotification = 15 / intervalMin * 2;
                }
            }

            for (int i = 0; i < numberOfNotification; i++)
            {
                var guid = Guid.NewGuid().ToString();
                generateFromDateTime = generateFromDateTime.AddMinutes(intervalMin);
                if (
                    notificationMode == NotificationModeEnum.Schedule &&
                    (TimeSpan.Compare(generateFromDateTime.TimeOfDay, startTime) < 0 ||
                    TimeSpan.Compare(generateFromDateTime.TimeOfDay, endTime) > 0))
                {
                    continue; // skip schedule when not in schedule period
                }
                notifications.Add(new NotificationModel() { 
                    ScheduledDateTime = generateFromDateTime,
                    Tag = guid
                });
                switch (action)
                {
                    case Actions.Notification:
                        SilentNotificationTemplate
                            .AddText(notificationText)
                            .Schedule(generateFromDateTime, toast =>
                            {
                                toast.Tag = guid;
                                toast.ExpirationTime = generateFromDateTime.AddSeconds(5);
                            });
                        break;
                    case Actions.NotificationAndSound:
                        DefaultNotificationTemplate
                            .AddText(notificationText)
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
