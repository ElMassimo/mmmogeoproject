
using System.ServiceModel;
using System.Collections.Generic;
using USARoadTrip.Api.Models;

namespace USARoadTrip.Web
{
    [ServiceContract]
    public interface IRoadTripServices
    {
        // Trip Services
        [OperationContract]
        bool CreateTrip(Trip tripModel);

        [OperationContract]
        void DeleteTrip(int tripId);

        [OperationContract]
        bool UpdateTrip(Trip tripModel);

        [OperationContract]
        IList<Location> GetTripDestinations(int tripId);

        // UserServices
        [OperationContract]
        bool Register(User userModel);

        [OperationContract]
        bool Login(string nick, string password);

        [OperationContract]
        IList<Trip> GetUserTrips(string nick);
    }
}
