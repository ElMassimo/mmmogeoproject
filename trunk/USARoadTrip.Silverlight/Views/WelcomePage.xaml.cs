using System;
using System.Windows;
using System.Windows.Controls;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.WCFServices;
using USARoadTrip.Silverlight.UserControls;
using USARoadTrip.Silverlight.ViewModels;
using System.ComponentModel;
using USARoadTrip.Silverlight.Utility;

namespace USARoadTrip.Silverlight.Views
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(UsernameTextBox.Text) || String.IsNullOrWhiteSpace(PasswordTextBox.Text))
                MessageBox.Show("Please enter your username and password", "Login", MessageBoxButton.OK);
            else
            {
                BusyIndicator.IsBusy = true;
                RoadTripServices.GetLoginService(LoginService_Completed).LoginAsync(UsernameTextBox.Text, PasswordTextBox.Text);
            }
        }

        private void LoginService_Completed(object sender, LoginCompletedEventArgs e)
        {
            BusyIndicator.IsBusy = false;

            if(e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
            else if (!e.Result)
                MessageBox.Show("Incorrect username or password", "Login", MessageBoxButton.OK);
            else
            {
                RoadTripGlobals.IsUserLogged = true;
                RoadTripGlobals.UserId = UsernameTextBox.Text;

                WelcomeLabel.Text = String.Format("Welcome {0}!", RoadTripGlobals.UserId);
                LoginStackPanel.Visibility = Visibility.Collapsed;
                UserInfoStackPanel.Visibility = Visibility.Visible;

                TripsButton.IsEnabled = true;
            }
        }

        private void LogoutLink_Click(object sender, RoutedEventArgs e)
        {
            TripsButton.IsEnabled = false;

            UserInfoStackPanel.Visibility = Visibility.Collapsed;
            LoginStackPanel.Visibility = Visibility.Visible;
            WelcomeLabel.Text = null;

            RoadTripGlobals.IsUserLogged = false;
            RoadTripGlobals.UserId = null;
        }

        private void RegistrationLink_Click(object sender, RoutedEventArgs e)
        {
            GuiUtils.GetRegistrationWindow(UserInfoWindow_Closed).Show();
        }

        private void UserInfoWindow_Closed(object sender, EventArgs e)
        {
            RegistrationWindow window = sender as RegistrationWindow;

            if(window.DialogResult ?? false)
                MessageBox.Show("Registration succesful!", "Registration", MessageBoxButton.OK);
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/MapPage.xaml", UriKind.Relative));
        }

        private void TripsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/TripsPage.xaml", UriKind.Relative));
        }
    }
}
