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
using USARoadTrip.Silverlight.WCFServices;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.Utility;

namespace USARoadTrip.Silverlight.UserControls
{
    public partial class LoginWindow : ChildWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(UsernameTextBox.Text) || String.IsNullOrWhiteSpace(PasswordTextBox.Password))
                MessageBox.Show("Please enter your username and password", "Login", MessageBoxButton.OK);
            else
            {
                BusyIndicator.IsBusy = true;
                RoadTripServices.GetLoginService(LoginService_Completed).LoginAsync(UsernameTextBox.Text, PasswordTextBox.Password);
            }
        }

        private void LoginService_Completed(object sender, LoginCompletedEventArgs e)
        {
            BusyIndicator.IsBusy = false;

            if (e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
            else if (!e.Result)
                MessageBox.Show("Incorrect username or password", "Login", MessageBoxButton.OK);
            else
            {
                RoadTripGlobals.IsUserLogged = true;
                RoadTripGlobals.UserId = UsernameTextBox.Text;
                this.DialogResult = true;
            }
        }
    }
}
