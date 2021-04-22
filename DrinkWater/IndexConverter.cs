using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace DrinkWater
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var item = value as SharedClass.Notification;
            int index = 0;
            var frame = Window.Current.Content as Frame; //this will find the current instance of the frame so you can get the instance of main page
            if (frame != null && frame.Content is NotificationPage)
            {
                var mainpage = (NotificationPage)frame.Content;
                index = mainpage.Notifications.IndexOf(item);
            }
            return string.Format("{0}. ", index + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
