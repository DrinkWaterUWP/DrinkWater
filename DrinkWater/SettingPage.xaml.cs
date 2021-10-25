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
            LocalSettings = new LocalSettings();
            Notification = new Notification();

            DataContext = this;
            ActionComboBox.DisplayMemberPath = "Key";
            ActionComboBox.SelectedValuePath = "Value";
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double.TryParse(LocalSettings.IntervalMin.ToString(), out double interval);
            RemindInterval.SelectedTime = TimeSpan.FromMinutes(interval);

            Enum.TryParse(LocalSettings.Action.ToString(), out Actions savedAction);
            ActionComboBox.SelectedIndex = (int)savedAction;

            NotificationTextBox.Text = LocalSettings.NotificationText;
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
            if (LocalSettings.Action.ToString() != ActionComboBox.SelectedValue.ToString())
            {
                LocalSettings.Action = (Actions)Enum.Parse(typeof(Actions), ActionComboBox.SelectedValue.ToString());
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
            if (LocalSettings.IntervalMin != RemindInterval.SelectedTime?.TotalMinutes)
            {
                LocalSettings.IntervalMin = (int)RemindInterval.SelectedTime?.TotalMinutes;
                SaveSuccessfullyFlyout.ShowAt(sender);
                Notification.RescheduleNotification();
            }
        }

        private void NotificationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NotificationTextBox.Text != null && LocalSettings.NotificationText != NotificationTextBox.Text)
            {
                LocalSettings.NotificationText = NotificationTextBox.Text;
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
