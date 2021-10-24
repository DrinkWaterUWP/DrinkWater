using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static SharedClass.Constant;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DrinkWater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvanceSettingPage : Page
    {
        ApplicationDataContainer localSettings;

        public AdvanceSettingModel ViewModel { get; set; }

        public AdvanceSettingPage()
        {
            InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            ViewModel = new AdvanceSettingModel();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        private void NotificationModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rdoDefault.IsChecked)
            {
                ScheduleGrid.Visibility = Visibility.Collapsed;
                if (localSettings.Values[NotificationMode].ToString() != NotificationModeEnum.Default.ToString())
                {
                    localSettings.Values[NotificationMode] = NotificationModeEnum.Default.ToString();
                    SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                }
            }
            else if ((bool)rdoSchedule.IsChecked)
            {
                ScheduleGrid.Visibility = Visibility.Visible;
                if (localSettings.Values[NotificationMode].ToString() != NotificationModeEnum.Schedule.ToString() && StartSchedule.SelectedTime != null && EndSchedule.SelectedTime != null && TimeSpan.Compare((TimeSpan)StartSchedule.SelectedTime, (TimeSpan)EndSchedule.SelectedTime) < 0)
                {
                    localSettings.Values[NotificationMode] = NotificationModeEnum.Schedule.ToString();
                    SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                }
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
                localSettings.Values[NotificationMode] = NotificationModeEnum.Schedule.ToString();
                localSettings.Values[NotificationScheduleStartTime] = StartSchedule.SelectedTime?.ToString();
                localSettings.Values[NotificationScheduleEndTime] = EndSchedule.SelectedTime?.ToString();
                SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
            }
        }
    }

    public class AdvanceSettingModel
    {
        ApplicationDataContainer localSettings;
        public bool rdoDefaultChecked { get; set; }
        public bool rdoScheduleChecked { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public AdvanceSettingModel()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values[NotificationMode] == null)
            {
                localSettings.Values[NotificationMode] = NotificationModeEnum.Default.ToString();
            }
            Enum.TryParse(typeof(NotificationModeEnum), localSettings.Values[NotificationMode].ToString(), out var mode);
            switch (mode)
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
            if (localSettings.Values[NotificationScheduleStartTime] != null)
            {
                TimeSpan parsed;
                TimeSpan.TryParse(localSettings.Values[NotificationScheduleStartTime].ToString(), out parsed);
                startTime = parsed;
                TimeSpan.TryParse(localSettings.Values[NotificationScheduleEndTime].ToString(), out parsed);
                endTime = parsed;
            }
        }
    }
}
