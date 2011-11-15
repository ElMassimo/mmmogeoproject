using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using USARoadTrip.Silverlight.Mock;
using USARoadTrip.Silverlight.Models;
using USARoadTrip.Silverlight.Services;
using USARoadTrip.Silverlight.Utility;
using USARoadTrip.Silverlight.ViewModels;
using Polyline = ESRI.ArcGIS.Client.Geometry.Polyline;
using USARoadTrip.Silverlight.UserControls;

namespace USARoadTrip.Silverlight.Views
{
    public partial class MapPage : Page
    {
        private const int BUFFER_DISTANCE = 20;
        private const int TIMER_MILLISECONDS = 3000;
        private DispatcherTimer _travelTimer;
        private MapPoint _lastPoint;
        private Car _car;
        private Road _road;
        private PointCollection _lastRoadSection;
        private double _lastSectionSpeed;
        private LocationViewModel _lastLocation;
        private State _currentState = new State();

        #region Services
        private Locator _usaStreetsLocatorTask;
        private GeometryService _geometryService;
        private QueryTask _countiesQueryTask;
        private QueryTask _statesQueryTask;
        private RouteTask _routingTask;
        #endregion
        
        #region Constructor / Properties
        public MapPage()
        {
            InitializeComponent();

            _usaStreetsLocatorTask = ESRIServices.GetUsaStreetsLocator(UsaStreetsLocatorTask_AddressToLocationsCompleted, LocatorTask_Failed);
            _geometryService = ESRIServices.GetGeometryService(GeometryService_BufferCompleted, GeometryService_Failed);
            _countiesQueryTask = ESRIServices.GetCountiesQueryTask(CountiesQueryTask_ExecuteCompleted, QueryTask_Failed);
            _statesQueryTask = ESRIServices.GetStatesQueryTask(StatesQueryTask_ExecuteCompleted, QueryTask_Failed);
            _routingTask = ESRIServices.GetRoutingTask(RoutingTask_SolveCompleted, RoutingTask_Failed);
            
            _travelTimer = new DispatcherTimer();
            _travelTimer.Tick += new EventHandler(DrivingLoop);

            StopsLayer.Graphics = MyTripList.Stops;
            UpdateLoggedState();
        }

        private GraphicsLayer CarLayer
        {
            get { return GetLayer("Car"); }
        }
        private GraphicsLayer StopsLayer
        {
            get { return GetLayer("Stops"); }
        }
        private GraphicsLayer RoadLayer
        {
            get { return GetLayer("Road"); }
        }
        private GraphicsLayer TravelLayer
        {
            get { return GetLayer("Travel"); }
        }
        private GraphicsLayer GPSLayer
        {
            get { return GetLayer("GPS"); }
        }
        private FeatureLayer CountiesLayer
        {
            get { return GetLayer("Counties") as FeatureLayer; }
        }
        #endregion

        #region Auxiliary Methods
        private GraphicsLayer GetLayer(string layerName)
        {
            return MyMap.Layers[layerName + "Layer"] as GraphicsLayer;
        }

        private Symbol GetSymbol(string symbolName)
        {
            return LayoutRoot.Resources[symbolName] as Symbol;
        }
        #endregion

        #region Graphics
        private void ShowCar()
        {
            CarLayer.ClearGraphics();

            Graphic carMarker = new Graphic();
            carMarker.Symbol = _car.GetSymbol();
            carMarker.Geometry = _car.CurrentLocation;
            carMarker.Geometry.SpatialReference = MyMap.SpatialReference;

            CarLayer.Graphics.Add(carMarker);
        }

        private void ShowRoadSection()
        {
            if (_lastRoadSection == null)
                return;

            PointCollection roadSection = _lastRoadSection;
            double sectionSpeed = _lastSectionSpeed;

            Polyline lastRouteSection = new Polyline();
            lastRouteSection.Paths.Add(roadSection);

            Graphic road = new Graphic();
            road.Attributes["SPEED"] = sectionSpeed;
            road.Attributes["SPEED_READABLE"] = String.Format("{0} km/h", Math.Round(sectionSpeed, 1));
            road.Attributes["DISTANCE"] = roadSection.GetTotalDistance().ToString("#0.0 km");
            road.Geometry = lastRouteSection;
            road.Geometry.SpatialReference = MyMap.SpatialReference;
            TravelLayer.Graphics.Add(road);

            Graphic gpsMarker = new Graphic();
            gpsMarker.Symbol = GetSymbol("GPSMarkerSymbol");
            gpsMarker.Geometry = roadSection.Last();
            gpsMarker.Geometry.SpatialReference = MyMap.SpatialReference;
            gpsMarker.SetZIndex(2);
            GPSLayer.Graphics.Add(gpsMarker);
        }

        private void ShowCounties()
        {
            CountiesLayer.ClearGraphics();
            if (_currentState.Counties != null)
                foreach (Graphic selectedGraphic in _currentState.Counties)
                    CountiesLayer.Graphics.Add(selectedGraphic);
        }
        #endregion

        #region Simulation
        private void PrepareTrip(Polyline route)
        {
            _road = new Road(route);
            _car = new Car(_currentState.Name, _road.StartLocation);
            ShowCar();
            BeginTravelButton.IsEnabled = true;
        }

        private void DrivingLoop(object sender, EventArgs e)
        {
            if(_statesQueryTask.IsBusy || _countiesQueryTask.IsBusy)
                return;

            double sectionSpeed = RoadUtils.GetRandomSectionSpeed();
            double sectionTime = RoadUtils.GetTimeBetweenGpsEmissions(SpeedSlider.Value / 10);  
           
            PointCollection routeSection = new PointCollection();
            routeSection.Add(_lastPoint);
            for (double distance = 0; distance < sectionSpeed * sectionTime; )
            {
                MapPoint tempPoint = _road.NextLocation();
                if (tempPoint == null)
                    break;
                distance += RoadUtils.GetDistanceInKilometers(_lastPoint, tempPoint);
                _lastPoint = tempPoint;
                routeSection.Add(_lastPoint);
            }

            _car.NextLocation = _lastPoint;

            ShowCar();
            ShowRoadSection();
            ShowCounties();

            _lastRoadSection = routeSection;
            _lastSectionSpeed = sectionSpeed;

            if (routeSection[0] == _lastPoint)
            {
                _travelTimer.Stop();
                BeginTravelButton.Content = "Begin travel";
            }
            else
            {
                ExecuteStateQuery(_car.NextLocation);
                ExecuteCountiesQuery(_car.NextLocation);
            }
        }

        private void BeginTravel_Click(object sender, RoutedEventArgs e)
        {
            if (_travelTimer.IsEnabled)
            {
                _travelTimer.Stop();
                BeginTravelButton.Content = "Begin travel";
            }
            else
            {
                StopSimulation();
                TravelLayer.ClearGraphics();

                ExecuteStateQuery(_road.StartLocation);
                ExecuteCountiesQuery(_road.StartLocation);
                _travelTimer.Interval = TimeSpan.FromMilliseconds(TIMER_MILLISECONDS);
                _travelTimer.Start();
                BeginTravelButton.Content = "Stop travel";
            }
        }
        #endregion

        private void LocationList_ListItemSelected(object sender, ListItemSelectedEventArgs e)
        {
            MapPoint location = e.SelectedItem.Point;
            double displaySize = MyMap.MinimumResolution * 300000;
            Envelope displayExtent = RoadUtils.GetCenteredEnvelope(location, displaySize);
            MyMap.ZoomTo(displayExtent);
        }

        private void MyMap_MouseClick(object sender, Map.MouseEventArgs e)
        {
        }

        #region States/Counties Queries
        private void ExecuteCountiesQuery(MapPoint mapPoint)
        {            
            _geometryService.CancelAsync();
            _countiesQueryTask.CancelAsync();

            Graphic marker = new Graphic();
            marker.Geometry = mapPoint;
            marker.Geometry.SpatialReference = MyMap.SpatialReference;

            BufferParameters bufferParams = new BufferParameters()
            {
                Unit = LinearUnit.Kilometer,
                Distances = { BUFFER_DISTANCE },
                BufferSpatialReference = new SpatialReference(4326),
                OutSpatialReference = MyMap.SpatialReference,
                Features = { marker }
            };

            _geometryService.BufferAsync(bufferParams);
        }

        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {
            //Graphic bufferGraphic = new Graphic();
            //bufferGraphic.Geometry = args.Results[0].Geometry;
            //bufferGraphic.Symbol = GetSymbol("BufferSymbol");
            //bufferGraphic.SetZIndex(1);
            //GPSLayer.Graphics.Add(bufferGraphic);

            Query query = new Query();
            query.ReturnGeometry = true;
            query.OutSpatialReference = MyMap.SpatialReference;
            query.Geometry = args.Results[0].Geometry;
            query.OutFields.Add("NAME");
            _countiesQueryTask.ExecuteAsync(query);
        }
        
        private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Geometry Service error: " + e.Error);
        }

        private void CountiesQueryTask_ExecuteCompleted(object sender, QueryEventArgs args)
        {
            if (args.FeatureSet.Features.Count > 0)
                _currentState.Counties = args.FeatureSet.Features;
            else
                MessageBox.Show("No counties found");
        }

        private void ExecuteStateQuery(MapPoint mapPoint)
        {
            _statesQueryTask.CancelAsync();
            Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutFields.Add("NAME_12");
            query.OutSpatialReference = MyMap.SpatialReference;
            query.Geometry = mapPoint;
            _statesQueryTask.ExecuteAsync(query);
        }
        
        private void StatesQueryTask_ExecuteCompleted(object sender, QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet != null && featureSet.Features.Count > 0)
            {
                string stateName = featureSet.Features[0].Attributes["NAME_12"] as String;
                _currentState.Name = stateName;
                if(_car != null)
                    _car.UpdateState(stateName);
            }
            else
                MessageBox.Show("The selected location does not belong to USA");
        }

        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }
        #endregion

        #region Geocoding
        private void AddLocation(MapPoint mapPoint)
        {
            if (_lastLocation == null)
                return;

            _lastLocation.Point = mapPoint;
            MyTripList.AddLocation(_lastLocation);
        }

        private void AddMockAddresses_Click(object sender, RoutedEventArgs e)
        {
            AddressPanel.DataContext = MockGenerator.GetMockAddress();
        }

        private void AddAddressButton_Click(object sender, RoutedEventArgs e)
        {
            AddressToLocationsParameters addressParams = new AddressToLocationsParameters();
            addressParams.OutSpatialReference = MyMap.SpatialReference;

            LocationViewModel address = AddressPanel.DataContext as LocationViewModel;

            if (address.IsValid)
            {
                address.ToKeyValue(addressParams.Address);
                AddingLocationBusyIndicator.IsBusy = true;
                _lastLocation = address;
                _usaStreetsLocatorTask.AddressToLocationsAsync(addressParams);
            }
            else
                MessageBox.Show("Please fill the address, city, and state");   
        }

        private void UsaStreetsLocatorTask_AddressToLocationsCompleted(object sender, ESRI.ArcGIS.Client.Tasks.AddressToLocationsEventArgs args)
        {
            List<AddressCandidate> returnedCandidates = args.Results;
            AddressCandidate bestCandidate = null;

            foreach (AddressCandidate candidate in returnedCandidates)
            {
                if (candidate.Score >= 80)
                {
                    if (candidate.Location.SpatialReference == null)
                        candidate.Location.SpatialReference = MyMap.SpatialReference;

                    if (!candidate.Location.SpatialReference.Equals(MyMap.SpatialReference))
                        MessageBox.Show("TODO: Translate the Spatial Reference");
                    else if (bestCandidate == null || candidate.Score > bestCandidate.Score)
                        bestCandidate = candidate;
                }
            }

            if (bestCandidate != null)
                AddLocation(bestCandidate.Location);
            else
                MessageBox.Show("There are no locations that match the specified address.");

            AddingLocationBusyIndicator.IsBusy = false;
        }

        private void LocatorTask_Failed(object sender, TaskFailedEventArgs e)
        {
            AddingLocationBusyIndicator.IsBusy = false;
            MessageBox.Show("Locator service failed: " + e.Error);
        }
        #endregion

        #region Routing
        private void FindRouteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StopsLayer.Graphics.Count > 1)
            {
                MenuBusyIndicator.IsBusy = true;
                _routingTask.SolveAsync(new RouteParameters() { Stops = StopsLayer, UseTimeWindows = false, OutSpatialReference = MyMap.SpatialReference });
            }
            else
                MessageBox.Show("You must select at least two locations to find a route");
        }

        private void RoutingTask_SolveCompleted(object sender, RouteEventArgs e)
        {
            RoadLayer.Graphics.Clear();

            RouteResult routeResult = e.RouteResults[0];
            Graphic lastRoute = routeResult.Route;
            
            PrepareTrip(lastRoute.Geometry as Polyline);

            double totalDistance = _road.MapPoints.GetTotalDistance();
            string distance = string.Format("{0} km", totalDistance.ToString("#0.0"));
            lastRoute.Attributes.Add("DISTANCE", distance);

            decimal minutes = (decimal)lastRoute.Attributes["Total_Time"];
            var totalTime = TimeSpan.FromMinutes(Convert.ToDouble(minutes));
            lastRoute.Attributes.Add("TIME", totalTime.ToFormattedString());

            RoadLayer.Graphics.Add(lastRoute);
            MessageBox.Show("Routing completed: travel safely!");
            MenuBusyIndicator.IsBusy = false;
        }

        private void RoutingTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MenuBusyIndicator.IsBusy = false;
            MessageBox.Show("Routing task failed: " + e.Error);
        }
        #endregion

        private void OpenAddressDialogButton_Click(object sender, RoutedEventArgs e)
        {
            AddressPanel.DataContext = new LocationViewModel();
            NewLocationDialog.Visibility = Visibility.Visible;
        }

        private void CloseAddressDialogButton_Click(object sender, RoutedEventArgs e)
        {
            NewLocationDialog.Visibility = Visibility.Collapsed;
        }

        private void StopSimulation()
        {
            _travelTimer.Stop();
            _routingTask.CancelAsync();
            _countiesQueryTask.CancelAsync();
            _geometryService.CancelAsync();
            _statesQueryTask.CancelAsync();

            if (_road != null)
            {
                _road.GoToStart();
                _lastPoint = _road.StartLocation;
                _car.CurrentLocation = _road.StartLocation;
                _lastRoadSection = null;
            }
        }

        private void ClearRoadGraphics()
        {
            RoadLayer.ClearGraphics();
            TravelLayer.ClearGraphics();
            CarLayer.ClearGraphics();
            CountiesLayer.ClearGraphics();
            GPSLayer.ClearGraphics();
        }

        private void Reset()
        {
            StopSimulation();
            ClearRoadGraphics();
            BeginTravelButton.IsEnabled = false;
        }
    

        private void StopTravel_Click(object sender, RoutedEventArgs e)
        {
            StopSimulation();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void ShowTripButton_Click(object sender, RoutedEventArgs e)
        {
            TripStackPanel.Visibility = Visibility.Visible;
            ShowTripButton.Visibility = Visibility.Collapsed;
        }

        private void HideTripLink_Click(object sender, RoutedEventArgs e)
        {
            TripStackPanel.Visibility = Visibility.Collapsed;
            ShowTripButton.Visibility = Visibility.Visible;
        }

        private void SaveTripButton_Click(object sender, RoutedEventArgs e)
        {
            MyTripList.SaveLocations(TripBusyIndicator);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            GuiUtils.GetLoginWindow(LoginWindow_Closed).Show();
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            LoginWindow window = sender as LoginWindow;

            if (window.DialogResult ?? false)
            {
                MessageBox.Show("Login succesful!", "Login", MessageBoxButton.OK);
                UpdateLoggedState();
            }
        }

        private void UpdateLoggedState()
        {
            if (RoadTripGlobals.IsUserLogged)
            {
                LoginButton.Visibility = Visibility.Collapsed;
                SaveTripButton.Visibility = Visibility.Visible;
            }
            else
            {
                LoginButton.Visibility = Visibility.Visible;
                SaveTripButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
