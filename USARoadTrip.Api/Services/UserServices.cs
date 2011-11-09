using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USARoadTrip.Api.Models;
using USARoadTrip.Api.EntityModels;
using USARoadTrip.Api.Extensions;
using System.Data.Objects;
using USARoadTrip.Api.Repository;

namespace USARoadTrip.Api.Services
{
    public class UserServices
    {
        private UserRepository UserRepo { get; set; }

        public UserServices() { UserRepo = new UserRepository(); }

        public User Get(string nick)
        {
            UserEntity user = UserRepo.GetSingle(u => u.Nick == nick);
            if (user == null)
                return null;
            else
                return user.ToModel<UserEntity, User>();
        }

        public bool Add(User userModel)
        {
            if (UserRepo.Any(u => u.Nick == userModel.Nick))
                return false;
            
            UserEntity user = userModel.ToEntity<User, UserEntity>();
            UserRepo.Add(user);
            UserRepo.Save();
            return true;
        }

        public IList<Trip> GetUserTrips(string nick)
        {
            UserEntity user = UserRepo.GetSingle(u => u.Nick == nick);
            if (user == null)
                return null;
            else
                return user.Trips.ToModels<TripEntity, Trip>();       
        }
    }
}
