using static SharedClass.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json;
using SharedClass;

namespace BackgroundTask
{
    public sealed class ScheduleNotification : IBackgroundTask
    {
        List<Notification> Notifications;
        ApplicationDataContainer localSettings;

        public int IntervalMin
        {
            //#if DEBUG
            //            get
            //            {
            //                return 10;
            //            }
            //#else
            get
            {
                if (localSettings.Values[ReminderIntervalMinKey] != null)
                {
                    int.TryParse(localSettings.Values[ReminderIntervalMinKey].ToString(), out int interval);
                    return interval;
                }
                else
                {
                    return 60;
                }
            }
            //#endif
        }
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values[IsTimerStarted] != null && !(bool)localSettings.Values[IsTimerStarted])
            {
                return;
            }
            Notifications = new List<Notification>();
            if (localSettings.Values[NotificationKey] != null)
            {
                Notifications = JsonConvert.DeserializeObject<List<Notification>>(localSettings.Values[NotificationKey].ToString());
            }
            Notifications = Notification.RemoveExpiredNotification(Notifications);
            Notifications = Notification.ScheduleNotification(Notifications, IntervalMin, Action);
            localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(Notifications);
        }

        private Actions Action
        {
            get
            {
                if (localSettings.Values[ActionKey] != null)
                {
                    Enum.TryParse(localSettings.Values[ActionKey].ToString(), out Actions result);
                    return result;
                }
                return Actions.Notification;
            }
        }
    }
}
