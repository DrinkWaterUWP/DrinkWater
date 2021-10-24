using Newtonsoft.Json;
using SharedClass;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using static SharedClass.Constant;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DrinkWater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationPage : Page
    {
        public List<Notification> Notifications;
        ApplicationDataContainer localSettings;

        public bool HasNoItems
        {
            get
            {
                return Notifications.Count == 0;
            }
        }

        public NotificationPage()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            DataContext = this;
            Notifications = new List<Notification>();
            if (localSettings.Values[NotificationKey] != null)
            {
                Notifications = JsonConvert.DeserializeObject<List<Notification>>(localSettings.Values[NotificationKey].ToString());
                Notifications = Notification.RemoveExpiredNotification(Notifications);
                localSettings.Values[NotificationKey] = JsonConvert.SerializeObject(Notifications);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        private async void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            new ContentDialog
            {
                Content = 
                "Drink water notification will show below.\n" +
                "Background task will run every 15 minutes to schedule notification.\n" +
                "Stop the timer if you wish to not receiving notification anymore.",
                CloseButtonText = "Close"
            }.ShowAsync();
        }
    }
}
