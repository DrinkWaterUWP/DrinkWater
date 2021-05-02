using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using static SharedClass.Constant;

namespace SharedClass
{
    public class Notification
    {
        public string Tag { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Notification(string tag, DateTime scheduledDateTime)
        {
            Tag = tag;
            ScheduledDateTime = scheduledDateTime;
        }

        public static List<Notification> ScheduleNotification(List<Notification> notifications, int intervalMin, Actions action)
        {
            int numberOfNotification = 0;
            DateTime lastNotificationScheduledDateTime;
            DateTime targetNotificationScheduledDateTime;
            DateTime generateFromDateTime = DateTime.Now;
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
                    return notifications;
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
                    var temp = lastNotificationScheduledDateTime;
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
                notifications.Add(new Notification(guid, generateFromDateTime));
                switch (action)
                {
                    case Actions.Notification:
                        SilentNotificationTemplate
                            .Schedule(generateFromDateTime, toast =>
                            {
                                toast.Tag = guid;
                                toast.ExpirationTime = generateFromDateTime.AddSeconds(5);
                            });
                        break;
                    case Actions.NotificationAndSound:
                        DefaultNotificationTemplate
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
            return notifications;
        }

        public static List<Notification> RemoveExpiredNotification(List<Notification> notifications)
        {
            foreach (var n in notifications.ToList())
            {
                if (DateTime.Now.CompareTo(n.ScheduledDateTime) >= 0)
                {
                    notifications.Remove(n);
                }
            }
            return notifications;
        }

        public static List<Notification> RemoveScheduledNotification()
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

            //#if DEBUG
            foreach (var item in scheduledToasts)
            {
                notifier.RemoveFromSchedule(item);
            }
            //#endif
            return new List<Notification>();
        }
    }
}
