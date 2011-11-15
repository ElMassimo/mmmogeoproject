using System;
using System.Windows;
using USARoadTrip.Silverlight.UserControls;

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

        public static LoginWindow GetLoginWindow(EventHandler closedHandler)
        {
            LoginWindow userInfoWindow = new LoginWindow();
            userInfoWindow.Closed += closedHandler;
            return userInfoWindow;
        }

        public static TripWindow GetTripWindow(EventHandler closedHandler)
        {
            TripWindow tripWindow = new TripWindow();
            tripWindow.Closed += closedHandler;
            return tripWindow;
        }

        public static Visibility BooleanToVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static void ShowConnectionErrorMessage()
        {
            MessageBox.Show("The operation could not be completed, please try again later", "Connection error", MessageBoxButton.OK);
        }

        public static string ToFormattedString(this TimeSpan time)
        {
            return String.Format("{0} hour{1} {2} minute{3}",
                time.Hours, time.Hours == 1 ? "" : "s",
                time.Minutes, time.Minutes == 1 ? "" : "s");
        }
    }
}
