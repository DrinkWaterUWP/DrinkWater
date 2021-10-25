using SharedClass;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using static SharedClass.Constant;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DrinkWater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if ((bool)rdoSchedule.IsChecked && TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) < 0 &&
                (LocalSettings.NotificationMode.ToString() != NotificationModeEnum.Schedule.ToString() ||
                TimeSpan.Compare((TimeSpan)LocalSettings.StartTime, (TimeSpan)StartSchedule.SelectedTime) != 0 ||
                TimeSpan.Compare((TimeSpan)LocalSettings.EndTime, (TimeSpan)EndSchedule.SelectedTime) != 0))
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
                    LocalSettings.NotificationMode = NotificationModeEnum.Schedule;
                    LocalSettings.StartTime = StartSchedule.SelectedTime;
                    LocalSettings.EndTime = EndSchedule.SelectedTime;
                    SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
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

        private void NotificationModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rdoDefault.IsChecked)
            {
                ScheduleGrid.Visibility = Visibility.Collapsed;
                if (LocalSettings.NotificationMode.ToString() != NotificationModeEnum.Default.ToString())
                {
                    LocalSettings.NotificationMode = NotificationModeEnum.Default;
                    SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                    Notification.RescheduleNotification();
                }
            }
            else if ((bool)rdoSchedule.IsChecked)
            {
                ScheduleGrid.Visibility = Visibility.Visible;
                //if (LocalSettings.NotificationMode.ToString() != NotificationModeEnum.Schedule.ToString() && StartSchedule.SelectedTime != null && EndSchedule.SelectedTime != null && TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) < 0)
                //{
                //    LocalSettings.NotificationMode = NotificationModeEnum.Schedule;
                //    SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                //    Notification.RescheduleNotification();
                //}
            }
        }

        private void SaveScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartSchedule.SelectedTime == null)
            {
                FlyoutBase.ShowAttachedFlyout(StartSchedule);
                return;
            }
            if (EndSchedule.SelectedTime == null)
            {
                FlyoutBase.ShowAttachedFlyout(EndSchedule);
                return;
            }
            if (TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) >= 0)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            else
            {
                LocalSettings.NotificationMode = NotificationModeEnum.Schedule;
                LocalSettings.StartTime = StartSchedule.SelectedTime;
                LocalSettings.EndTime = EndSchedule.SelectedTime;
                SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                Notification.RescheduleNotification();
            }
        }
    }

    public class AdvanceSettingModel
    {
        LocalSettings LocalSettings;
        public bool rdoDefaultChecked { get; set; }
        public bool rdoScheduleChecked { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public AdvanceSettingModel()
        {
            LocalSettings = new LocalSettings();
            switch (LocalSettings.NotificationMode)
            {
                case NotificationModeEnum.Default:
                    rdoDefaultChecked = true;
                    break;
                case NotificationModeEnum.Schedule:
                    rdoScheduleChecked = true;
                    break;
                default:
                    break;
            }
            startTime = (TimeSpan)LocalSettings.StartTime;
            endTime = (TimeSpan)LocalSettings.EndTime;
        }
    }
}
