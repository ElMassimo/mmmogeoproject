using System;
using USARoadTrip.Silverlight.WCFServices;

namespace USARoadTrip.Silverlight.Services
{
    public class RoadTripServices
    {
        public static RoadTripServicesClient GetLoginService(EventHandler<LoginCompletedEventArgs> completedHandler)
        {
            RoadTripServicesClient client = new RoadTripServicesClient();
            client.LoginCompleted += completedHandler;
            return client;
        }

        public static RoadTripServicesClient GetRegistrationService(EventHandler<RegisterCompletedEventArgs> completedHandler)
        {
            RoadTripServicesClient client = new RoadTripServicesClient();
            client.RegisterCompleted += completedHandler;
            return client;
        }

        public static RoadTripServicesClient GetUserTripsService(EventHandler<GetUserTripsCompletedEventArgs> completedHandler)
        {
            RoadTripServicesClient client = new RoadTripServicesClient();
            client.GetUserTripsCompleted += completedHandler;
            return client;
        }
    }
}
