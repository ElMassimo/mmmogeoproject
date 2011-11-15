using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USARoadTrip.Silverlight.ViewModels;

namespace USARoadTrip.Silverlight.UserControls
{
    public partial class LocationList : UserControl
    {
        private ObservableCollection<LocationViewModel> _locations = new ObservableCollection<LocationViewModel>();

        public ObservableCollection<LocationViewModel> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }

        public LocationList()
        {
            InitializeComponent();
            LocationZip.DataContext = this;
            LocationListBox.ItemsSource = _locations;
            LocationListBox.MouseRightButtonDown += new MouseButtonEventHandler(AvoidSilverlightMenu);
        }

        public void SetLocations(List<LocationViewModel> locations)
        {
            _locations = new ObservableCollection<LocationViewModel>(locations);
            LocationListBox.ItemsSource = _locations;
        }

        private void AvoidSilverlightMenu(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var location = ((MenuItem)sender).DataContext as LocationViewModel;
            if (location != null)
                _locations.Remove(location);
        }
        
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            var location = ((MenuItem)sender).DataContext as LocationViewModel;
            if (location != null)
            {
                int index = _locations.IndexOf(location);
                if (index > 0)
                {
                    bool wasSelected = LocationListBox.SelectedItem == location;
                    _locations.RemoveAt(index);
                    _locations.Insert(index - 1, location);
                    if (wasSelected)
                        LocationListBox.SelectedItem = location;
                }
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            var location = ((MenuItem)sender).DataContext as LocationViewModel;
            if (location != null)
            {
                int index = _locations.IndexOf(location);
                if (index < _locations.Count - 1)
                {
                    bool wasSelected = LocationListBox.SelectedItem == location;
                    _locations.RemoveAt(index);
                    _locations.Insert(index + 1, location);
                    if (wasSelected)
                        LocationListBox.SelectedItem = location;
                }
            }
        }
    }
}
