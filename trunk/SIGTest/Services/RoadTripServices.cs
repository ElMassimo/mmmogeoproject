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
using ESRI.ArcGIS.Client.Tasks;

namespace SIGTest.Services
{
    public class RoadTripServices
    {
        public const string LOCATOR_GEOCODE_SERVER_URL = "http://tasks.arcgisonline.com/ArcGIS/rest/services/Locators/ESRI_Places_World/GeocodeServer";
        public const string GEOMETRY_SERVER_URL = "http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer";
        public const string DEMOGRAPHICS_MAP_SERVER_URL = "http://server.arcgisonline.com/ArcGIS/rest/services/Demographics/USA_1990-2000_Population_Change/MapServer";
        
            public static GeometryService GetGeometryService(EventHandler<GraphicsEventArgs> successHandler, EventHandler<TaskFailedEventArgs> failureHandler)
        {
            GeometryService geometryService = new GeometryService(GEOMETRY_SERVER_URL);
            geometryService.BufferCompleted += successHandler;
            geometryService.Failed += failureHandler;
            return geometryService;
        }

        public static QueryTask GetQueryTask(EventHandler<QueryEventArgs> successHandler, EventHandler<TaskFailedEventArgs> failureHandler)
        {
            QueryTask queryTask = new QueryTask(DEMOGRAPHICS_MAP_SERVER_URL+"/3");
            queryTask.ExecuteCompleted += successHandler;
            queryTask.Failed += failureHandler;
            return queryTask;
        }

         public static Locator GetLocator(EventHandler<AddressToLocationsEventArgs> successHandler, EventHandler<TaskFailedEventArgs> failureHandler)
         {
             Locator locatorTask = new Locator(LOCATOR_GEOCODE_SERVER_URL);
             locatorTask.AddressToLocationsCompleted += successHandler;
             locatorTask.Failed += failureHandler;
             return locatorTask;
         }
    }
}
