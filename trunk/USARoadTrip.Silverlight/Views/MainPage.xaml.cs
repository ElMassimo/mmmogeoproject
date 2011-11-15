using System;
using System.Windows;
using System.Windows.Controls;

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
    }
}
