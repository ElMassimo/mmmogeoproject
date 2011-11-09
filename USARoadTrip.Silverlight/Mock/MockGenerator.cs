
using USARoadTrip.Silverlight.ViewModels;
using System.Collections.Generic;

namespace USARoadTrip.Silverlight.Mock
{
    public static class MockGenerator
    {
        private static List<AddressViewModel> _addresses = new List<AddressViewModel>()
            {                           
                new AddressViewModel(){ Address = "1050 Van Ness Avenue", City = "San Francisco", State = "CA", Zip = "94109" },                 
                new AddressViewModel(){ Address = "2055 South Redwood Road", City = "Salt Lake City", State = "UT", Zip = "84104" },
                new AddressViewModel(){ Address = "520 South Michigan Avenue", City = "South Loop", State = "IL", Zip = "60605" },  
                new AddressViewModel(){ Address = "9600 International Dr", City = "Orlando", State = "FL", Zip = "32819" },     
                new AddressViewModel(){ Address = "240 West 55th Street", City = "New York", State = "NY", Zip = "10019" },  
                new AddressViewModel(){ Address = "316 Cambridge Street", City = "Boston", State = "MA", Zip = "02114" },  
                new AddressViewModel(){ Address = "220 Stanford Dr SE", City = "Albuquerque", State = "NM", Zip = "87106" },  
                new AddressViewModel(){ Address = "2205 Forest Drive Southeast", City = "Cedar Rapids", State = "IA", Zip = "52403" },  
                new AddressViewModel(){ Address = "324 Northeast 12th Avenue", City = "Portland", State = "OR", Zip = "97232" },  
                new AddressViewModel(){ Address = "750 West Broadway", City = "Jackson", State = "WY", Zip = "83001" },  
            };

        private static int _currentAddressIndex = -1;

        public static AddressViewModel GetMockAddress()
        {
            _currentAddressIndex = (_currentAddressIndex + 1) % _addresses.Count;
            return _addresses[_currentAddressIndex];
        }
    }
}