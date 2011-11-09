using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using USARoadTrip.Api.Services;
using USARoadTrip.Api.Models;

namespace USARoadTrip.Web
{
    public class RoadTripServices : IRoadTripServices
    {
        private UserServices _userServices = new UserServices();
        private TripServices _tripServices = new TripServices();

        #region TripServices
        public bool CreateTrip(Trip tripModel)
        {
            return _tripServices.Add(tripModel);
        }

        public void DeleteTrip(int tripId)
        {
            _tripServices.Delete(tripId);
        }

        public bool UpdateTrip(Trip tripModel)
        {
            return _tripServices.Update(tripModel);
        }
        
        public IList<Location> GetTripDestinations(int tripId)
        {
            return _tripServices.GetTripDestinations(tripId);
        }
        #endregion

        #region UserServices
        public bool Register(User userModel)
        {
            return _userServices.Add(userModel);
        }

        public bool Login(string nick, string password)
        {
            User user = _userServices.Get(nick);
            if (user == null)
                return false;
            else
                return user.Password == password;
        }

        public IList<Trip> GetUserTrips(string nick)
        {
            return _userServices.GetUserTrips(nick);
        }
        #endregion
    }
}
