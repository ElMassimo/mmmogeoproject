using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using USARoadTrip.Silverlight.UserControls;
using USARoadTrip.Silverlight.ViewModels;

namespace USARoadTrip.Silverlight.Utility
{
    public static class GuiUtils
    {
        public static RegistrationWindow GetRegistrationWindow(EventHandler closedHandler)
        {
            RegistrationWindow userInfoWindow = new RegistrationWindow();
            userInfoWindow.Closed += closedHandler;
            return userInfoWindow;
        }

        public static void ShowConnectionErrorMessage()
        {
            MessageBox.Show("The operation could not be completed, please try again later", "Connection error", MessageBoxButton.OK);
        }
    }
}
