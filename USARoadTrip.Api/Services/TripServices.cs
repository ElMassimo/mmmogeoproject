using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USARoadTrip.Api.Repository;
using USARoadTrip.Api.Models;
using USARoadTrip.Api.EntityModels;
using USARoadTrip.Api.Extensions;

namespace USARoadTrip.Api.Services
{
    public class TripServices
    {
        private UserRepository UserRepo { get; set; }
        private TripRepository TripRepo { get; set; }
        private LocationRepository LocationRepo { get; set; }

        public TripServices()
        {
            UserRepo = new UserRepository(); 
            TripRepo = new TripRepository();
            LocationRepo = new LocationRepository();
        }


        private void AddNewLocations(TripEntity trip, IEnumerable<Location> newLocations)
        {
            var locations = trip.Destinations.ToDictionary(d => d.Id);

            foreach (var location in newLocations)
            {
                if (!locations.ContainsKey(location.Id))
                    trip.Destinations.Add(location.ToEntity<Location, LocationEntity>());
            }
        }

        private void UpdateExistingLocations(TripEntity trip, IEnumerable<Location> newLocations)
        {
            var locations = newLocations.ToDictionary(l => l.Id);

            foreach (var location in trip.Destinations)
            {
                if (locations.ContainsKey(location.Id))
                    locations[location.Id].ToEntity(location);
                else
                {
                    trip.Destinations.Remove(location);
                    LocationRepo.Delete(location);
                }
            }
        }

        public bool Add(Trip tripModel)
        {
            UserEntity user = UserRepo.GetSingle(u => u.Nick == tripModel.UserNick);
            if (user == null)
                return false;

            if (TripRepo.Any(t => t.UserNick == user.Nick && t.Name == tripModel.Name))
                return false;
            
            TripEntity trip = tripModel.ToEntity<Trip, TripEntity>();
            trip.User = user;
            user.Trips.Add(trip);
            TripRepo.Add(trip);
            TripRepo.Save();
            return true;
        }

        public void Delete(int tripId)
        {
            TripEntity trip = TripRepo.GetSingle(t => t.Id == tripId);
            if (trip == null)
                return;

            UserEntity user = UserRepo.GetSingle(u => u.Nick == trip.UserNick);
            if (user != null)
                user.Trips.Remove(trip);

            foreach (var location in trip.Destinations)
            {
                trip.Destinations.Remove(location);
                LocationRepo.Delete(location);
            }

            TripRepo.Delete(trip);
            TripRepo.Save();
        }

        public bool Update(Trip tripModel)
        {
            TripEntity trip = TripRepo.GetSingle(t => t.Id == tripModel.Id);
            if (trip == null)
                return false;

            tripModel.ToEntity(trip);

            UpdateExistingLocations(trip, tripModel.Destinations);
            AddNewLocations(trip, tripModel.Destinations);

            TripRepo.Save();
            return true;
        }

        public IList<Location> GetTripDestinations(int tripId)
        {
            TripEntity trip = TripRepo.GetSingle(t => t.Id == tripId);
            if (trip == null)
                return null;
            else
                return trip.Destinations.ToModels<LocationEntity, Location>();            
        }
    }
}
