using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using SharedClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using static SharedClass.Constant;

namespace DrinkWater
{
    public sealed partial class SettingPage : Page
    {
        ApplicationDataContainer localSettings;
        List<Notification> Notifications;

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
            set
            {
                uint interval = 0;
                if (localSettings.Values[ReminderIntervalMinKey] != null)
                {
                    uint.TryParse(localSettings.Values[ReminderIntervalMinKey].ToString(), out interval);
                }
                if (interval != value)
                {
                    localSettings.Values[ReminderIntervalMinKey] = value;
                }
            }
        }

        public List<ComboBoxPairs> ActionsItem { get; set; } = new List<ComboBoxPairs>
            {
                new ComboBoxPairs("Show reminder without sound", Actions.Notification.ToString()),
                new ComboBoxPairs("Show reminder with sound", Actions.NotificationAndSound.ToString()),
                //new ComboBoxPairs("Notification with custom sound", Actions.NotificationAndCustomSound.ToString())
            };

        public SettingPage()
        {
            InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            DataContext = this;
            ActionComboBox.DisplayMemberPath = "Key";
            ActionComboBox.SelectedValuePath = "Value";
            Notifications = new List<Notification>();
            if (localSettings.Values[NotificationKey] != null)
            {
                Notifications = JsonConvert.DeserializeObject<List<Notification>>(localSettings.Values[NotificationKey].ToString());
            }
            Notifications = Notification.RemoveExpiredNotification(Notifications);
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
                        .AddText("Test reminder without sound")
                        .Show(toast =>
                        {
                            toast.ExpirationTime = DateTime.Now.AddSeconds(5);
                        });
                    break;
                case Actions.NotificationAndSound:
                    DefaultNotificationTemplate
                        .AddText("Test reminder with sound")
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
            if (IntervalMin != RemindInterval.SelectedTime?.TotalMinutes)
            {
                IntervalMin = (int)RemindInterval.SelectedTime?.TotalMinutes;

                localSettings.Values[IsTimerStarted] = true;
                if (Notifications.Count > 0)
                {
                    Notifications = Notification.RemoveScheduledNotification();
                    Notifications = Notification.ScheduleNotification(Notifications, IntervalMin, Action);
                }
                localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(Notifications);
                SaveSuccessfullyFlyout.ShowAt(sender);
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
    }
}
