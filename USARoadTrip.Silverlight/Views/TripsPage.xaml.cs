using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
