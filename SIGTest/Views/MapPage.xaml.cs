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

namespace SIGTest.Views
{
    public partial class MapPage : Page
    {
        private Locator _usaStreetsLocatorTask;
        private GeometryService _geometryService;
        private QueryTask _countiesQueryTask;
        private RouteTask _routingTask;

        List<Graphic> _stops = new List<Graphic>();

        #region Constructor / Properties
        public MapPage()
        {
            InitializeComponent();

            _usaStreetsLocatorTask = RoadTripServices.GetUsaStreetsLocator(UsaStreetsLocatorTask_AddressToLocationsCompleted, LocatorTask_Failed);
            _geometryService = RoadTripServices.GetGeometryService(GeometryService_BufferCompleted, GeometryService_Failed);
            _countiesQueryTask = RoadTripServices.GetCountiesQueryTask(CountiesQueryTask_ExecuteCompleted, QueryTask_Failed);
            _routingTask = RoadTripServices.GetRoutingTask(RoutingTask_SolveCompleted, RoutingTask_Failed);
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

        private void AddStop(MapPoint mapPoint)
        {
            Graphic stop = new Graphic() { Geometry = mapPoint, Symbol = GetSymbol("StopSymbol") };
            stop.Attributes.Add("StopNumber", StopsLayer.Graphics.Count + 1);
            StopsLayer.Graphics.Add(stop);
            _stops.Add(stop);
        }
        #endregion
        
        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            _geometryService.CancelAsync();
            _countiesQueryTask.CancelAsync();

            BufferLayer.ClearGraphics();
            CountiesLayer.ClearGraphics();

            Graphic marker = new Graphic();
            marker.Symbol = GetSymbol("DefaultMarkerSymbol");
            marker.Geometry = e.MapPoint;
            // Input spatial reference for buffer operation defined by first feature of input geometry array
            marker.Geometry.SpatialReference = MyMap.SpatialReference;

            marker.SetZIndex(2);
            BufferLayer.Graphics.Add(marker);

            // If buffer spatial reference is GCS and unit is linear, geometry service will do geodesic buffering
            BufferParameters bufferParams = new BufferParameters()
            {
                Unit = LinearUnit.StatuteMile,
                BufferSpatialReference = new SpatialReference(4326),
                OutSpatialReference = MyMap.SpatialReference
            };
            bufferParams.Features.Add(marker);
            bufferParams.Distances.Add(100);

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
            if (args.FeatureSet.Features.Count < 1)
            {
                MessageBox.Show("No counties found");
                return;
            }

            foreach (Graphic selectedGraphic in args.FeatureSet.Features)
            {
                selectedGraphic.Symbol = GetSymbol("ParcelSymbol");
                CountiesLayer.Graphics.Add(selectedGraphic);
            }
        }

        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }


        #region Geocoding
        private void AddAddressButton_Click(object sender, RoutedEventArgs e)
        {
            AddressToLocationsParameters addressParams = new AddressToLocationsParameters();
            addressParams.OutSpatialReference = MyMap.SpatialReference;

            AddressViewModel address = AddressPanel.DataContext as AddressViewModel;

            if (address.ToKeyValue(addressParams.Address).Count > 0)
            {
                _usaStreetsLocatorTask.AddressToLocationsAsync(addressParams);
                MessageBox.Show("Query sent");
            }
            else
                MessageBox.Show("Please enter an address");

            address.Clear();
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
        }

        private void LocatorTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Locator service failed: " + e.Error);
        }
        #endregion

        #region Routing
        private void FindRouteButton_Click(object sender, RoutedEventArgs e)
        {
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

            MessageBox.Show("Routing completed: travel safely!");
        }

        private void RoutingTask_Failed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("Routing task failed: " + e.Error);
        }
        #endregion

        private void AddMockAddresses_Click(object sender, RoutedEventArgs e)
        {
            AddressPanel.DataContext = MockGenerator.GetMockAddress();
        }
    }
}
