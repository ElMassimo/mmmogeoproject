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
using System.Collections.ObjectModel;
using USARoadTrip.Silverlight.Utility;
using USARoadTrip.Silverlight.Services;
using System.ComponentModel;

namespace USARoadTrip.Silverlight.UserControls
{
    public partial class TripList : UserControl
    {
        private ObservableCollection<Trip> _trips = new ObservableCollection<Trip>();
        private Trip _addedTrip;

        public event TripSelectedEventHandler TripSelected;

        public TripList()
        {
            InitializeComponent();
            TripListBox.ItemsSource = _trips;
            TripListBox.MouseRightButtonDown += new MouseButtonEventHandler(AvoidSilverlightMenu);
        }

        public void LoadTrips()
        {
            BusyIndicator.IsBusy = true;
            RoadTripServices.GetUserTripsService(LoadTrips_Completed).GetUserTripsAsync(RoadTripGlobals.UserId);
        }

        private void LoadTrips_Completed(object sender, GetUserTripsCompletedEventArgs e)
        {
            BusyIndicator.IsBusy = false;

            if (e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
            else if (e.Result == null)
                MessageBox.Show("The user does not exist");
            else
            {
                _trips = new ObservableCollection<Trip>(e.Result);
                TripListBox.ItemsSource = _trips;
            }
        }
                
        private void AvoidSilverlightMenu(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }        

        private void AddTripButton_Click(object sender, RoutedEventArgs e)
        {
            var tripWindow = GuiUtils.GetTripWindow(AddTripWindow_Closed);
            _addedTrip = tripWindow.MyTrip;
            tripWindow.Show();
        }

        private void AddTripWindow_Closed(object sender, EventArgs e)
        {
            TripWindow window = sender as TripWindow;
            if (window.DialogResult ?? false)
                _trips.Add(_addedTrip);
        }

        private void EditTrip(Trip trip)
        {
            TripWindow tripWindow = new TripWindow(trip);
            tripWindow.Closed += EditTripWindow_Closed;
            tripWindow.Show();
        }

        private void EditTripWindow_Closed(object sender, EventArgs e)
        {
            //TripWindow window = sender as TripWindow;
            //if (window.DialogResult ?? false)
            //    _trips.Add(_addedTrip);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var trip = ((MenuItem)sender).DataContext as Trip;
            if (trip != null)
                EditTrip(trip);
        }

        private void EditTripButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripListBox.SelectedItem != null)
                EditTrip(TripListBox.SelectedItem as Trip);
        }

        private void DeleteTrip(Trip trip)
        {
            BusyIndicator.IsBusy = true;
            int index = _trips.IndexOf(trip);
            _trips.RemoveAt(index);
            RoadTripServices.DeleteTripService(DeleteTrip_Complete).DeleteTripAsync(trip.UserNick, trip.Name);
        }

        private void DeleteTrip_Complete(object sender, AsyncCompletedEventArgs e)
        {
            BusyIndicator.IsBusy = false;
            if (e.Error != null)
                GuiUtils.ShowConnectionErrorMessage();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var trip = ((MenuItem)sender).DataContext as Trip;
            if (trip != null)
                DeleteTrip(trip);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripListBox.SelectedItem != null)
                DeleteTrip(TripListBox.SelectedItem as Trip);
        }

        private void ViewMap_Click(object sender, RoutedEventArgs e)
        {
            var trip = ((MenuItem)sender).DataContext as Trip;
            if (trip != null)
                TripSelected(TripListBox, new TripSelectedEventArgs(trip));
        }

        private void ViewMapButton_Click(object sender, RoutedEventArgs e)
        {
            if(TripListBox.SelectedItem != null)
                TripSelected(TripListBox, new TripSelectedEventArgs(TripListBox.SelectedItem as Trip));
        }

        private void TripListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool itemSelected = e.AddedItems.Count > 0;
            ViewMapButton.IsEnabled = AddTripButton.IsEnabled = EditTripButton.IsEnabled = DeleteTripButton.IsEnabled = itemSelected;
        }
    }

    public delegate void TripSelectedEventHandler(object sender, TripSelectedEventArgs e);

    public class TripSelectedEventArgs : EventArgs
    {
        public Trip SelectedItem { get; set; }

        public TripSelectedEventArgs(Trip item)
        {
            SelectedItem = item;
        }
    }
}
