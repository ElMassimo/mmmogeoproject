using System;
using System.Windows;
using System.Windows.Controls;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.WCFServices;

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
            if(String.IsNullOrWhiteSpace(UsernameTextBox.Text) || String.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Please enter your username and password", "Login", MessageBoxButton.OK);
                return;
            }

            RoadTripServices.GetLoginService(LoginService_Completed).LoginAsync(UsernameTextBox.Text, PasswordTextBox.Text);
        }

        private void LoginService_Completed(object sender, LoginCompletedEventArgs e)
        {
            if(e.Result)
                NavigationService.Navigate(new Uri("/Views/MapPage.xaml", UriKind.Relative));
            else
                MessageBox.Show("Incorrect username or password", "Login", MessageBoxButton.OK);
        }
    }
}
