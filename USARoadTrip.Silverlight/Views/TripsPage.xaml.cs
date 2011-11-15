using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.WCFServices;
using USARoadTrip.Silverlight.UserControls;
using USARoadTrip.Silverlight.Utility;
using System.Windows;

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

            MyTripsList.LoadTrips();
        }

        private void MyTripsList_TripSelected(object sender, TripSelectedEventArgs e)
        {
            LoadingTripBusyIndicator.IsBusy = true;
            RoadTripGlobals.LoadedTrip = e.SelectedItem;
            RoadTripServices.GetTripDestinations(GetTripDestinations_Completed).GetTripDestinationsAsync(e.SelectedItem.UserNick, e.SelectedItem.Name);
        }

        private void GetTripDestinations_Completed(object sender, GetTripDestinationsCompletedEventArgs e)
        {
            LoadingTripBusyIndicator.IsBusy = false;
            if (e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
            else if (e.Result == null)
                MessageBox.Show("The selected trip no longer exists");
            else
            {
                RoadTripGlobals.LoadedTrip.Destinations = e.Result;
                NavigationService.Navigate(new Uri("/Views/MapPage.xaml?TripLoaded", UriKind.Relative));
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
