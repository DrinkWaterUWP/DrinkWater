using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace SharedClass
{
    public sealed class Constant
    {
        public enum Actions
        {
            Notification,
            NotificationAndSound,
            NotificationAndCustomSound
        }
        public static string ActionKey { get { return "Action"; } }
        public static string NotificationKey { get { return "Notification"; } }
        public static string NotificationTextKey { get { return "NotificationText"; } }
        public static string ReminderIntervalMinKey { get { return "RemindIntervalMin"; } }
        public static string IsTimerStarted { get { return "IsTimerStarted"; } }
        public static string ScheduleNotificationTask { get { return "ScheduleNotificationTask"; } }
        public static string ScheduleNotificationTaskEntry { get { return "BackgroundTask.ScheduleNotification"; } }
        public static string ScheduleNotificationTask1 { get { return "ScheduleNotificationTask1"; } }
        public static string ScheduleNotificationTaskEntry1 { get { return "BackgroundTask.ScheduleNotification"; } }
        public static string IsFirstTimeKey { get { return "IsFirstTime"; } }
        public static ToastContentBuilder DefaultNotificationTemplate
        {
            get
            {
                return new ToastContentBuilder()
                        .AddText("Drink Water")
                        .SetToastDuration(ToastDuration.Short)
                        .SetToastScenario(ToastScenario.Default);
            }
        }
        public static ToastContentBuilder SilentNotificationTemplate
        {
            get
            {
                return new ToastContentBuilder()
                        .AddText("Drink Water")
                        .AddAudio(new Uri("ms-appx:///Assets/Audio/no_sound.mp3"))
                        .SetToastDuration(ToastDuration.Short)
                        .SetToastScenario(ToastScenario.Default);
            }
        }
    }
}
