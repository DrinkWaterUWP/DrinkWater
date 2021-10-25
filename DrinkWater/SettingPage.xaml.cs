using SharedClass;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using static SharedClass.Constant;

namespace DrinkWater
{
    public sealed partial class SettingPage : Page
    {
        ApplicationDataContainer localSettings;
        Notification Notification;
        LocalSettings LocalSettings;


        public List<ComboBoxPairs> ActionsItem { get; set; } = new List<ComboBoxPairs>
            {
                new ComboBoxPairs("Off", Actions.Notification.ToString()),
                new ComboBoxPairs("On", Actions.NotificationAndSound.ToString()),
                //new ComboBoxPairs("Notification with custom sound", Actions.NotificationAndCustomSound.ToString())
            };

        public SettingPage()
        {
            InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            LocalSettings = new LocalSettings();
            Notification = new Notification();

            DataContext = this;
            ActionComboBox.DisplayMemberPath = "Key";
            ActionComboBox.SelectedValuePath = "Value";
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (localSettings.Values[ReminderIntervalMinKey] != null)
            {
                double.TryParse(localSettings.Values[ReminderIntervalMinKey].ToString(), out double interval);
                RemindInterval.SelectedTime = TimeSpan.FromMinutes(interval);
            }

            if (localSettings.Values[ActionKey] != null)
            {
                Enum.TryParse(localSettings.Values[ActionKey].ToString(), out Actions savedAction);
                ActionComboBox.SelectedIndex = (int)savedAction;
            }

            if (localSettings.Values[NotificationTextKey] != null)
            {
                NotificationTextBox.Text = localSettings.Values[NotificationTextKey].ToString();
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionComboBox.SelectedValue == null)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                return;
            }
            Enum.TryParse(ActionComboBox.SelectedValue.ToString(), out Actions action);
            switch (action)
            {
                case Actions.Notification:
                    SilentNotificationTemplate
                        .AddText(LocalSettings.NotificationText)
                        .Show(toast =>
                        {
                            toast.ExpirationTime = DateTime.Now.AddSeconds(5);
                        });
                    break;
                case Actions.NotificationAndSound:
                    DefaultNotificationTemplate
                        .AddText(LocalSettings.NotificationText)
                        .Show(toast =>
                        {
                            toast.ExpirationTime = DateTime.Now.AddSeconds(5);
                        });
                    break;
                default:
                    break;
            }
        }

        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActionComboBox.SelectedValue == null)
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                return;
            }
            if (localSettings.Values[ActionKey].ToString() != ActionComboBox.SelectedValue.ToString())
            {
                localSettings.Values[ActionKey] = ActionComboBox.SelectedValue.ToString();
                SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                Notification.RescheduleNotification();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        private void RemindInterval_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args)
        {
            if (RemindInterval.SelectedTime == null || RemindInterval.SelectedTime?.TotalMinutes <= 0)
            {
                FlyoutBase.ShowAttachedFlyout(sender);
                return;
            }
            if (localSettings == null)
            {
                localSettings = ApplicationData.Current.LocalSettings;
            }
            if (LocalSettings.IntervalMin != RemindInterval.SelectedTime?.TotalMinutes)
            {
                LocalSettings.IntervalMin = (int)RemindInterval.SelectedTime?.TotalMinutes;
                SaveSuccessfullyFlyout.ShowAt(sender);
                Notification.RescheduleNotification();
            }
        }

        private void NotificationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NotificationTextBox.Text != null && localSettings.Values[NotificationTextKey].ToString() != NotificationTextBox.Text)
            {
                localSettings.Values[NotificationTextKey] = NotificationTextBox.Text;
                SaveSuccessfullyFlyout.ShowAt((FrameworkElement)sender);
                Notification.RescheduleNotification();
            }
        }

        private void AdvanceSetting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdvanceSettingPage));
        }
    }
}
