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

namespace USARoadTrip.Silverlight.Views
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonGoToMap_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Navigate(new Uri("/Views/MapPage.xaml", UriKind.Relative));
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            MainContent.GoBack();
        }

        private void MainContent_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ButtonGoBack.IsEnabled =  MainContent.CanGoBack;
            ButtonGoToMap.IsEnabled = !e.Uri.ToString().Contains("MapPage");
        }
    }
}
