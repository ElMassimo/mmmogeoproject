using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USARoadTrip.Silverlight.ViewModels;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using System;
using USARoadTrip.Silverlight.Utility;
using USARoadTrip.Silverlight.WCFServices;
using USARoadTrip.Silverlight.Services;

namespace USARoadTrip.Silverlight.UserControls
{
    public partial class LocationList : UserControl
    {
        private Trip _trip;        
        private ObservableCollection<LocationViewModel> _locations = new ObservableCollection<LocationViewModel>();
        private GraphicCollection _stops = new GraphicCollection();

        private BusyIndicator _busyIndicator { get; set; }

        public Trip MyTrip
        {
            get { return _trip; }
            set 
            { 
                _trip = value;
                if (_trip != null)
                    LoadTrip();
            }
        }

        public ObservableCollection<LocationViewModel> Locations
        {
            get { return _locations; }
            set { _locations = value; }
        }

        public GraphicCollection Stops
        {
            get { return _stops; }
            set { _stops = value; }
        }

        public event ListItemSelectedEventHandler ListItemSelected;

        public LocationList()
        {
            InitializeComponent();
            LocationListBox.ItemsSource = _locations;
            LocationListBox.MouseRightButtonDown += new MouseButtonEventHandler(AvoidSilverlightMenu);
        }

        public void SetLocations(List<LocationViewModel> locations)
        {
            _locations = new ObservableCollection<LocationViewModel>(locations);
            LocationListBox.ItemsSource = _locations;
        }

        public void AddLocation(LocationViewModel locationViewModel)
        {
            _locations.Add(locationViewModel);

            Graphic stop = new Graphic() { Geometry = locationViewModel.Point };
            stop.Attributes["StopNumber"] = _stops.Count + 1;
            _stops.Add(stop);
        }

        public void SaveLocations(BusyIndicator busyIndicator)
        {
            _busyIndicator = busyIndicator;
            if (MyTrip == null)
            {
                var tripWindow = GuiUtils.GetTripWindow(TripWindow_Closed);
                _trip = new Trip();
                tripWindow.MyTrip = _trip;
                tripWindow.Show();
            }
            else
                UploadTripLocations();
        }

        public void ClearLocations()
        {
            _locations.Clear();
            _stops.Clear();
        }

        private void AvoidSilverlightMenu(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TripWindow_Closed(object sender, EventArgs e)
        {
            TripWindow window = sender as TripWindow;
            if (window.DialogResult ?? false)
            {
                TripNameLabel.Text = _trip.Name;
                UploadTripLocations();
            }
        }

        private void UploadTripLocations()
        {
            _busyIndicator.BusyContent = "Saving locations...";
            _busyIndicator.IsBusy = true;
            MyTrip.Destinations = new List<Location>();

            int i = 0;
            foreach (LocationViewModel location in Locations)
                MyTrip.Destinations.Add(location.ToDataContract(i++));

            RoadTripServices.UpdateTripService(UpdateTrip_Completed).UpdateTripAsync(MyTrip);
        }

        private void UpdateTrip_Completed(object sender, UpdateTripCompletedEventArgs e)
        {
            _busyIndicator.IsBusy = false;
            if (e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
            else if (!e.Result)
                MessageBox.Show(String.Format("Sorry, you haven't created a trip with the name '{0}'", MyTrip.Name), "Save my trip locations", MessageBoxButton.OK);
            else 
                MessageBox.Show("Your trip locations were succesfully saved", "Save my trip locations", MessageBoxButton.OK);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var location = ((MenuItem)sender).DataContext as LocationViewModel;
            if (location != null)
            {
                int index = _locations.IndexOf(location);
                _locations.RemoveAt(index);
                _stops.RemoveAt(index);
                RenumberStops();
            }
        }

        private void MoveUpStop(int index)
        {
            var stop = _stops[index];
            var upperStop = _stops[index - 1];

            stop.Attributes["StopNumber"] = index;
            upperStop.Attributes["StopNumber"] = index + 1;

            _stops.RemoveAt(index);
            _stops.Insert(index - 1, stop);
        }

        private void MoveDownStop(int index)
        {
            var stop = _stops[index];
            var downStop = _stops[index + 1];

            downStop.Attributes["StopNumber"] = index + 1;
            stop.Attributes["StopNumber"] = index + 2;

            _stops.RemoveAt(index);
            _stops.Insert(index + 1, stop);
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
                    MoveUpStop(index);
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
                    MoveDownStop(index);
                }
            }
        }

        private void RenumberStops()
        {
            int i = 1;
            foreach (var stop in Stops)
                stop.Attributes["StopNumber"] = i++;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            var location = ((MenuItem)sender).DataContext as LocationViewModel;
            if (location != null)
                ListItemSelected(LocationListBox, new ListItemSelectedEventArgs(_locations.IndexOf(location), location));
        }

        private void LoadTrip()
        {
            ClearLocations();
            TripNameLabel.Text = _trip.Name;
            if(_trip.Destinations != null)
            foreach (var location in _trip.Destinations)
                AddLocation(LocationViewModel.FromDataContract(location));
        }
    }

    public delegate void ListItemSelectedEventHandler(object sender, ListItemSelectedEventArgs e);

    public class ListItemSelectedEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
        public LocationViewModel SelectedItem { get; set; }

        public ListItemSelectedEventArgs(int index, LocationViewModel item)
        {
            SelectedIndex = index;
            SelectedItem = item;
        }
    }
}
