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
using System.Windows.Navigation;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using SIGTest.ViewModels;
using SIGTest.Services;
using ESRI.ArcGIS.Client.Symbols;
using SIGTest.Mock;
using SIGTest.Models;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;
using Polyline = ESRI.ArcGIS.Client.Geometry.Polyline;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using SIGTest.Utility;

namespace SIGTest.Views
{
    public partial class MapPage : Page
    {
        #region Services
        private Locator _usaStreetsLocatorTask;
        private GeometryService _geometryService;
        private QueryTask _countiesQueryTask;
        private QueryTask _statesQueryTask;
        private RouteTask _routingTask;
        #endregion
        
        private const double MAP_TO_KM_RATIO = 5000;
        private const int TIMER_MILLISECONDS = 3000;
        private DispatcherTimer _travelTimer;
        private MapPoint _lastPoint;
        private Car _car;
        private Road _road;
        private State _currentState = new State();
        List<Graphic> _stops = new List<Graphic>();

        #region Constructor / Properties
        public MapPage()
        {
            InitializeComponent();

            _usaStreetsLocatorTask = RoadTripServices.GetUsaStreetsLocator(UsaStreetsLocatorTask_AddressToLocationsCompleted, LocatorTask_Failed);
            _geometryService = RoadTripServices.GetGeometryService(GeometryService_BufferCompleted, GeometryService_Failed);
            _countiesQueryTask = RoadTripServices.GetCountiesQueryTask(CountiesQueryTask_ExecuteCompleted, QueryTask_Failed);
            _statesQueryTask = RoadTripServices.GetStatesQueryTask(StatesQueryTask_ExecuteCompleted, QueryTask_Failed);
            _routingTask = RoadTripServices.GetRoutingTask(RoutingTask_SolveCompleted, RoutingTask_Failed);
            
            _travelTimer = new DispatcherTimer();
            _travelTimer.Tick += new EventHandler(DrivingLoop);
        }

        private GraphicsLayer BufferLayer
        {
            get { return GetLayer("Buffer"); } 
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
        private GraphicsLayer CountiesLayer
        {
            get { return GetLayer("Counties"); }
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
        
        private void ShowCar()
        {
            CarLayer.ClearGraphics();

            Graphic carMarker = new Graphic();
            carMarker.Symbol = _car.GetSymbol();
            carMarker.Geometry = _car.CurrentLocation;
            carMarker.Geometry.SpatialReference = MyMap.SpatialReference;

            CarLayer.Graphics.Add(carMarker);
        }

        private void ShowCounties()
        {
            CountiesLayer.ClearGraphics();
            if(_currentState.Counties != null)
                foreach (Graphic selectedGraphic in _currentState.Counties)
                {
                    selectedGraphic.Symbol = GetSymbol("ParcelSymbol");
                    CountiesLayer.Graphics.Add(selectedGraphic);
                }
        }

        private void PrepareTrip(Polyline route)
        {
            _road = new Road(route);
            _car = new Car(_currentState.Name, _road.StartLocation);
            ShowCar();
        }

        private void DrivingLoop(object sender, EventArgs e)
        {
            if(_statesQueryTask.IsBusy || _countiesQueryTask.IsBusy)
                return;
            

            for (double d = 0; d < MAP_TO_KM_RATIO * (SpeedSlider.Value + 1) * 50; )
            {
                MapPoint tempPoint = _road.NextLocation();
                if(tempPoint == null)
                    break;
                d += RoadUtils.CalculateDistance(_lastPoint, tempPoint);
                _lastPoint = tempPoint;
            }

            _car.NextLocation = _lastPoint;

            ShowCar();
            ShowCounties();

            if (_lastPoint == null)
                _travelTimer.Stop();
            else
            {
                ExecuteStateQuery(_car.NextLocation);
                ExecuteCountiesQuery(_car.NextLocation);
            }
        }

        private void BeginTravel_Click(object sender, RoutedEventArgs e)
        {
            _road.GoToStart();
            _lastPoint = _road.StartLocation;
            _car.CurrentLocation = _road.StartLocation;
            _travelTimer.Interval = TimeSpan.FromMilliseconds(TIMER_MILLISECONDS);
            _travelTimer.Start();
        }

        private void StopTravel_Click(object sender, RoutedEventArgs e)
        {
            _travelTimer.Stop();
        }

        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            Graphic marker = new Graphic();
            marker.Symbol = GetSymbol("DefaultMarkerSymbol");
            marker.Geometry = e.MapPoint;
            marker.Geometry.SpatialReference = MyMap.SpatialReference;
            marker.SetZIndex(2);
            BufferLayer.Graphics.Add(marker);

            QueryTask _statesQueryTask2 = RoadTripServices.GetStatesQueryTask(StatesQueryTask_ExecuteCompleted2, QueryTask_Failed);
             Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.OutFields.Add("NAME_12");
            query.OutSpatialReference = MyMap.SpatialReference;
            query.Geometry = e.MapPoint;
            _statesQueryTask2.ExecuteAsync(query);
        }
        private void StatesQueryTask_ExecuteCompleted2(object sender, QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet != null && featureSet.Features.Count > 0)
            {
                string stateName = featureSet.Features[0].Attributes["NAME_12"] as String;
                 MessageBox.Show(stateName);
            }
            else
            {
                MessageBox.Show("The selected location does not belong to USA");
            }
        }
        private void ExecuteCountiesQuery(MapPoint mapPoint)
        {            
            _geometryService.CancelAsync();
            _countiesQueryTask.CancelAsync();
            BufferLayer.ClearGraphics();

            Graphic marker = new Graphic();
            marker.Symbol = GetSymbol("DefaultMarkerSymbol");
            marker.Geometry = mapPoint;
            marker.Geometry.SpatialReference = MyMap.SpatialReference;

            BufferParameters bufferParams = new BufferParameters()
            {
                Unit = LinearUnit.Kilometer,
                Distances = { 20 },
                BufferSpatialReference = new SpatialReference(4326),
                OutSpatialReference = MyMap.SpatialReference,
                Features = { marker }
            };

            _geometryService.BufferAsync(bufferParams);
        }

        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {
            Graphic bufferGraphic = new Graphic();
            bufferGraphic.Geometry = args.Results[0].Geometry;
            bufferGraphic.Symbol = GetSymbol("BufferSymbol");
            bufferGraphic.SetZIndex(1);

            BufferLayer.Graphics.Add(bufferGraphic);

            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.ReturnGeometry = true;
            query.OutSpatialReference = MyMap.SpatialReference;
            query.Geometry = bufferGraphic.Geometry;
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


        #region Geocoding
        private void AddStop(MapPoint mapPoint)
        {
            Graphic stop = new Graphic() { Geometry = mapPoint, Symbol = GetSymbol("StopSymbol") };
            stop.Attributes.Add("StopNumber", _stops.Count + 1);
            StopsLayer.Graphics.Add(stop);
            _stops.Add(stop);

            if (_stops.Count == 1)
            {
                ExecuteStateQuery(mapPoint);
                ExecuteCountiesQuery(mapPoint);
            }
        }

        private void AddAddressButton_Click(object sender, RoutedEventArgs e)
        {
            AddressToLocationsParameters addressParams = new AddressToLocationsParameters();
            addressParams.OutSpatialReference = MyMap.SpatialReference;

            AddressViewModel address = AddressPanel.DataContext as AddressViewModel;

            if (address.ToKeyValue(addressParams.Address).Count > 0)
            {
                AddAddressButton.IsEnabled = false;
                _usaStreetsLocatorTask.AddressToLocationsAsync(addressParams);
            }
            else
                MessageBox.Show("Please enter a valid address");            
        }

        private void AddMockAddresses_Click(object sender, RoutedEventArgs e)
        {
            AddressPanel.DataContext = MockGenerator.GetMockAddress();
        }

        private void ClearAddresses_Click(object sender, RoutedEventArgs e)
        {
            StopsLayer.ClearGraphics();
            _stops.Clear();
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
                AddStop(bestCandidate.Location);
            else
                MessageBox.Show("There are no locations that match the specified address.");

            AddAddressButton.IsEnabled = true;
        }

        private void LocatorTask_Failed(object sender, TaskFailedEventArgs e)
        {
            AddAddressButton.IsEnabled = true;
            MessageBox.Show("Locator service failed: " + e.Error);
        }
        #endregion

        #region Routing
        private void FindRouteButton_Click(object sender, RoutedEventArgs e)
        {
            FindRouteButton.IsEnabled = false;
            _routingTask.SolveAsync(new RouteParameters() { Stops = StopsLayer, UseTimeWindows = false, OutSpatialReference = MyMap.SpatialReference });
        }

        private void RoutingTask_SolveCompleted(object sender, RouteEventArgs e)
        {
            RoadLayer.Graphics.Clear();

            RouteResult routeResult = e.RouteResults[0];
            Graphic lastRoute = routeResult.Route;

            decimal totalTime = (decimal)lastRoute.Attributes["Total_Time"];
            string tip = string.Format("{0} minutes", totalTime.ToString("#0.000"));
            lastRoute.Attributes.Add("TIP", tip);

            RoadLayer.Graphics.Add(lastRoute);

            PrepareTrip(lastRoute.Geometry as Polyline);

            MessageBox.Show("Routing completed: travel safely!");
            FindRouteButton.IsEnabled = true;
        }

        private void RoutingTask_Failed(object sender, TaskFailedEventArgs e)
        {
            FindRouteButton.IsEnabled = true;
            MessageBox.Show("Routing task failed: " + e.Error);
        }
        #endregion
    }
}
