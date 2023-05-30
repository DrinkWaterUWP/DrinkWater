using static SharedClass.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json;
using SharedClass;

namespace BackgroundTask
{
    public sealed class ScheduleNotification : IBackgroundTask
    {
        Notification Notification;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Notification = new Notification();
            Notification.RemoveExpiredNotification();
            Notification.ScheduleNotification();
        }
    }
}
