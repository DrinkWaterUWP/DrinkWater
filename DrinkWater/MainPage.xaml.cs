using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using static SharedClass.Constant;

namespace DrinkWater
{
    public sealed partial class MainPage : Page
    {
        private const string CountdownTimerFormat = @"hh\:mm\:ss";
        ApplicationDataContainer localSettings;
        Timer timer;
        List<SharedClass.Notification> Notifications;

        public int IntervalMin
        {
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

        private string NotificationText
        {
            get
            {
                if (localSettings.Values[NotificationTextKey] != null)
                {
                    return localSettings.Values[NotificationTextKey].ToString();
                }
                return "Keep calm and drink water.";
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Application.Current.Resuming += new EventHandler<object>(App_Resuming);
            localSettings = ApplicationData.Current.LocalSettings;
        }

        private async void App_Resuming(object sender, object e)
        {
            await StartTimer();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await StartTimer();
        }

        private async Task StartTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            await Task.Run(() =>
            {
                timer = new Timer(CheckStatus, null, 0, 1000);
            }
            );
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            timer.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (localSettings.Values[IsFirstTimeKey] == null)
            {
                SetDefaultValue();
                RegisterBackgroundTask();
            }

            HandleVersionChange();

            Notifications = new List<SharedClass.Notification>();
            if (localSettings.Values[NotificationKey] != null)
            {
                Notifications = JsonConvert.DeserializeObject<List<SharedClass.Notification>>(localSettings.Values[NotificationKey].ToString());
            }
            Notifications = SharedClass.Notification.RemoveExpiredNotification(Notifications);
            if (Notifications.Count > 0)
            {
                localSettings.Values[IsTimerStarted] = true;
                StopButton.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                localSettings.Values[IsTimerStarted] = false;
                StopButton.Visibility = Visibility.Collapsed;
                StartButton.Visibility = Visibility.Visible;
            }
        }

        private void HandleVersionChange()
        {
            if (localSettings.Values[NotificationTextKey] == null)
            {
                localSettings.Values[NotificationTextKey] = "Keep calm and drink water.";
            }
        }

        private DateTime? GetNextScheduledNotificationDateTime()
        {
            Notifications = SharedClass.Notification.RemoveExpiredNotification(Notifications);
            if (Notifications.Count > 0)
            {
                return Notifications[0].ScheduledDateTime;
            }
            return null;
        }

        private IBackgroundTaskRegistration RegisterBackgroundTask()
        {
            var registeredTask = IsBackgroundTaskRegistered();
            if (registeredTask == null)
            {
                var builder = new BackgroundTaskBuilder();
                builder.Name = ScheduleNotificationTask;
                builder.TaskEntryPoint = ScheduleNotificationTaskEntry;
                builder.SetTrigger(new TimeTrigger(15, false));
                builder.AddCondition(new SystemCondition(SystemConditionType.SessionConnected));
                builder.Register();

                builder = new BackgroundTaskBuilder();
                builder.Name = ScheduleNotificationTask1;
                builder.TaskEntryPoint = ScheduleNotificationTaskEntry1;
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.SessionConnected, false));
                return builder.Register();
            }
            return registeredTask;
        }

        private IBackgroundTaskRegistration IsBackgroundTaskRegistered()
        {
            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == ScheduleNotificationTask)
                {
                    return t.Value;
                }
            }
            return null;
        }

        private void SetDefaultValue()
        {
            localSettings.Values[IsFirstTimeKey] = false;
            localSettings.Values[IsTimerStarted] = false;
            localSettings.Values[ActionKey] = Actions.NotificationAndSound.ToString();
            localSettings.Values[ReminderIntervalMinKey] = 60;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values[IsTimerStarted] = true;
            StopButton.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Collapsed;
            Notifications = SharedClass.Notification.RemoveExpiredNotification(Notifications);
            Notifications = SharedClass.Notification.ScheduleNotification(Notifications, IntervalMin, Action, NotificationText);
            localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(Notifications);
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        public async void CheckStatus(Object stateInfo)
        {
            var scheduledDateTime = GetNextScheduledNotificationDateTime();
            if (scheduledDateTime != null)
            {
                var remainingSecond = (double)(scheduledDateTime?.Subtract(DateTime.Now).TotalSeconds);
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    TimerCountdown.Text = TimeSpan.FromSeconds(remainingSecond).ToString(CountdownTimerFormat);
                }
                );
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    TimerCountdown.Text = TimeSpan.FromMinutes(IntervalMin).ToString(CountdownTimerFormat);
                }
                );
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values[IsTimerStarted] = false;

            Notifications = SharedClass.Notification.RemoveScheduledNotification();
            localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(Notifications);

            StopButton.Visibility = Visibility.Collapsed;
            StartButton.Visibility = Visibility.Visible;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage), null);
        }

        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotificationPage), null);
        }
    }
}
