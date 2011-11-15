
using System.Collections.Generic;
using USARoadTrip.Silverlight.ViewModels;

namespace USARoadTrip.Silverlight.Mock
{
    public static class MockGenerator
    {
        private static List<LocationViewModel> _addresses = new List<LocationViewModel>()
            {                           
                new LocationViewModel(){ Address = "1050 Van Ness Avenue", City = "San Francisco", State = "CA", Zip = "94109" },                 
                new LocationViewModel(){ Address = "2055 South Redwood Road", City = "Salt Lake City", State = "UT", Zip = "84104" },
                new LocationViewModel(){ Address = "520 South Michigan Avenue", City = "South Loop", State = "IL", Zip = "60605" },  
                new LocationViewModel(){ Address = "9600 International Dr", City = "Orlando", State = "FL", Zip = "32819" },     
                new LocationViewModel(){ Address = "240 West 55th Street", City = "New York", State = "NY", Zip = "10019" },  
                new LocationViewModel(){ Address = "316 Cambridge Street", City = "Boston", State = "MA", Zip = "02114" },  
                new LocationViewModel(){ Address = "220 Stanford Dr SE", City = "Albuquerque", State = "NM", Zip = "87106" },  
                new LocationViewModel(){ Address = "2205 Forest Drive Southeast", City = "Cedar Rapids", State = "IA", Zip = "52403" },  
                new LocationViewModel(){ Address = "324 Northeast 12th Avenue", City = "Portland", State = "OR", Zip = "97232" },  
                new LocationViewModel(){ Address = "750 West Broadway", City = "Jackson", State = "WY", Zip = "83001" },  
            };

        private static int _currentAddressIndex = -1;

        public static LocationViewModel GetMockAddress()
        {
            _currentAddressIndex = (_currentAddressIndex + 1) % _addresses.Count;
            return _addresses[_currentAddressIndex];
        }
    }
}