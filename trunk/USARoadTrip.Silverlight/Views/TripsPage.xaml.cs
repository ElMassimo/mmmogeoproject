using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.WCFServices;

namespace USARoadTrip.Silverlight.Views
{
    public partial class TripsPage : Page
    {
        public TripsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(!RoadTripGlobals.IsUserLogged)
                NavigationService.Navigate(new Uri("/Views/WelcomePage.xaml", UriKind.Relative));

            string userId = RoadTripGlobals.UserId;
            var client = RoadTripServices.GetUserTripsService(UserTripsService_Completed);
        }

        private void UserTripsService_Completed(object sender, GetUserTripsCompletedEventArgs e)
        {

        }
    }
}
