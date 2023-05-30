using SharedClass;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DrinkWater
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotificationPage : Page
    {
        public List<NotificationModel> Notifications;


        Notification Notification;
        LocalSettings LocalSettings;

        public bool HasNoItems
        {
            get
            {
                return Notifications.Count == 0;
            }
        }

        public NotificationPage()
        {
            InitializeComponent();
            Notification = new Notification();
            LocalSettings = new LocalSettings();
            Notification.RemoveExpiredNotification();
            Notifications = LocalSettings.Notifications;
            DataContext = this;
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
                "This list shows all of your scheduled drink water notifications.\n" +
                "A background task will run every 15 minutes to schedule new notifications.\n" +
                "To stop receiving notifications, stop the timer.",
                CloseButtonText = "Close"
            }.ShowAsync();
        }
    }
}
