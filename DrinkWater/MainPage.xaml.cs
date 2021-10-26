using SharedClass;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
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
        Timer timer;
        Notification Notification;
        LocalSettings LocalSettings;
        List<NotificationModel> Notifications;

        public MainPage()
        {
            InitializeComponent();
            Application.Current.Resuming += new EventHandler<object>(App_Resuming);
            LocalSettings = new LocalSettings();
            Notification = new Notification();
            Notifications = LocalSettings.Notifications;
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
            if (LocalSettings.IsFirstTime)
            {
                SetDefaultValue();
                RegisterBackgroundTask();
            }

            Notification.RemoveExpiredNotification();
            if (LocalSettings.IsTimerStarted)
            {
                StopButton.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Collapsed;
                ShowMessageForScheduleMode();
            }
            else
            {
                StopButton.Visibility = Visibility.Collapsed;
                StartButton.Visibility = Visibility.Visible;
            }
        }

        private DateTime? GetNextScheduledNotificationDateTime()
        {
            Notification.RemoveExpiredNotification();
            Notifications = LocalSettings.Notifications;
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
            LocalSettings.IsFirstTime = false;
            LocalSettings.IsTimerStarted = false;
            LocalSettings.Action = Actions.NotificationAndSound;
            LocalSettings.IntervalMin = 60;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            LocalSettings.IsTimerStarted = true;
            StopButton.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Collapsed;
            Notification.ScheduleNotification();
            ShowMessageForScheduleMode();
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
                    TimerCountdown.Text = TimeSpan.FromMinutes(LocalSettings.IntervalMin).ToString(CountdownTimerFormat);
                }
                );
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            LocalSettings.IsTimerStarted = false;
            Notification.RemoveScheduledNotification();

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

        private void ShowMessageForScheduleMode()
        {
            if (LocalSettings.NotificationMode == NotificationModeEnum.Schedule && Notifications.Count == 0)
            {
                NotificationModeMessage.Text = $"You have scheduled to show drink water notification\n" +
                    $"from {DateTime.Today.Add(LocalSettings.StartTime):hh:mm tt} to {DateTime.Today.Add(LocalSettings.EndTime):hh:mm tt}";
            }
        }
    }
}
