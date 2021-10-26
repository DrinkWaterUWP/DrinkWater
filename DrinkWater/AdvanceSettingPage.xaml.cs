using SharedClass;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using static SharedClass.Constant;

namespace DrinkWater
{
    public sealed partial class AdvanceSettingPage : Page
    {
        Notification Notification;
        LocalSettings LocalSettings;

        public AdvanceSettingModel ViewModel { get; set; }

        public AdvanceSettingPage()
        {
            InitializeComponent();
            LocalSettings = new LocalSettings();
            Notification = new Notification();
            ViewModel = new AdvanceSettingModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.scheduleToggleSwitchOn)
            {
                ScheduleGrid.Visibility = Visibility.Collapsed;
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (ScheduleToggleSwitch.IsOn && TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) < 0 &&
                (LocalSettings.NotificationMode.ToString() != NotificationModeEnum.Schedule.ToString() ||
                TimeSpan.Compare(LocalSettings.StartTime, (TimeSpan)StartSchedule.SelectedTime) != 0 ||
                TimeSpan.Compare(LocalSettings.EndTime, (TimeSpan)EndSchedule.SelectedTime) != 0))
            {
                ContentDialog unsavedSettingDialog = new ContentDialog
                {
                    Title = "Setting modified",
                    Content = "Do you want to save it?",
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Discard"
                };

                ContentDialogResult result = await unsavedSettingDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    SaveScheduleSetting();
                    await Task.Delay(1000);
                    rootFrame.GoBack();
                }
                else
                {
                    rootFrame.GoBack();
                }
            }
            else
            {
                rootFrame.GoBack();
            }
        }

        private void ScheduleToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!ScheduleToggleSwitch.IsOn)
            {
                ScheduleGrid.Visibility = Visibility.Collapsed;
                if (LocalSettings.NotificationMode.ToString() != NotificationModeEnum.Default.ToString())
                {
                    LocalSettings.NotificationMode = NotificationModeEnum.Default;
                    SaveSuccessfullyFlyout.ShowAt(SaveScheduleButton);
                    Notification.RescheduleNotification();
                }
            }
            else if (ScheduleToggleSwitch.IsOn)
            {
                ScheduleGrid.Visibility = Visibility.Visible;
            }
        }

        private void SaveScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            SaveScheduleSetting();
        }

        private void SaveScheduleSetting()
        {
            if (TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) >= 0)
            {
                FlyoutBase.ShowAttachedFlyout(SaveScheduleButton);
            }
            else
            {
                LocalSettings.NotificationMode = NotificationModeEnum.Schedule;
                LocalSettings.StartTime = (TimeSpan)StartSchedule.SelectedTime;
                LocalSettings.EndTime = (TimeSpan)EndSchedule.SelectedTime;
                SaveSuccessfullyFlyout.ShowAt(SaveScheduleButton);
                Notification.RescheduleNotification();
            }
        }
    }

    public class AdvanceSettingModel
    {
        LocalSettings LocalSettings;
        public bool scheduleToggleSwitchOn { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public AdvanceSettingModel()
        {
            LocalSettings = new LocalSettings();
            switch (LocalSettings.NotificationMode)
            {
                case NotificationModeEnum.Default:
                    scheduleToggleSwitchOn = false;
                    break;
                case NotificationModeEnum.Schedule:
                    scheduleToggleSwitchOn = true;
                    break;
                default:
                    break;
            }
            startTime = LocalSettings.StartTime;
            endTime = LocalSettings.EndTime;
        }
    }
}
