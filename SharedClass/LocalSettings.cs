using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Storage;
using static SharedClass.Constant;

namespace SharedClass
{
    public class LocalSettings
    {
        ApplicationDataContainer localSettings;
        public LocalSettings()
        {
            localSettings = ApplicationData.Current.LocalSettings;
        }


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
            set
            {
                localSettings.Values[ReminderIntervalMinKey] = value.ToString();
            }
        }

        public Actions Action
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
            set
            {
                localSettings.Values[ActionKey] = value.ToString();
            }
        }

        public string NotificationText
        {
            get
            {
                if (localSettings.Values[NotificationTextKey] != null)
                {
                    return localSettings.Values[NotificationTextKey].ToString();
                }
                return "Keep calm and drink water.";
            }
            set
            {
                localSettings.Values[NotificationTextKey] = value;
            }
        }

        public NotificationModeEnum NotificationMode
        {
            get
            {
                if (localSettings.Values[NotificationModeKey] != null)
                {
                    Enum.TryParse(localSettings.Values[NotificationModeKey].ToString(), out NotificationModeEnum result);
                    return result;
                }
                return NotificationModeEnum.Default;
            }
            set
            {
                localSettings.Values[NotificationModeKey] = value.ToString();
            }
        }

        public TimeSpan StartTime
        {
            get
            {
                if (localSettings.Values[NotificationScheduleStartTime] != null)
                {
                    TimeSpan.TryParse(localSettings.Values[NotificationScheduleStartTime].ToString(), out TimeSpan result);
                    return result;
                }
                return new TimeSpan(6, 0, 0);
            }
            set
            {
                localSettings.Values[NotificationScheduleStartTime] = value.ToString();
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                if (localSettings.Values[NotificationScheduleEndTime] != null)
                {
                    TimeSpan.TryParse(localSettings.Values[NotificationScheduleEndTime].ToString(), out TimeSpan result);
                    return result;
                }
                return new TimeSpan(18, 0, 0);
            }
            set
            {
                localSettings.Values[NotificationScheduleEndTime] = value.ToString();
            }
        }

        public List<NotificationModel> Notifications
        {
            get
            {
                if (localSettings.Values[NotificationKey] != null)
                {
                    return JsonConvert.DeserializeObject<List<NotificationModel>>(localSettings.Values[NotificationKey].ToString());
                }
                return new List<NotificationModel>();
            }
            set
            {
                localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(value);
            }
        }

        public bool IsFirstTime
        {
            get
            {
                if (localSettings.Values[IsFirstTimeKey] == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                localSettings.Values[IsFirstTimeKey] = value;
            }
        }

        public bool IsTimerStarted
        {
            get
            {
                if (localSettings.Values[IsFirstTimeKey] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)localSettings.Values[IsTimerStartedKey];
                }
            }
            set
            {
                localSettings.Values[IsTimerStartedKey] = value;
            }
        }
    }
}
