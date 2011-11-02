
using SIGTest.ViewModels;
using System.Collections.Generic;

namespace SIGTest.Mock
{
    public static class MockGenerator
    {
        private static List<AddressViewModel> _addresses = new List<AddressViewModel>()
            {                
                new AddressViewModel(){ Address = "680 Ocean Drive", City = "South Beach", State = "FL", Zip = "33139" },                
                new AddressViewModel(){ Address = "1050 Van Ness Avenue", City = "San Francisco", State = "CA", Zip = "94109" },
                new AddressViewModel(){ Address = "520 South Michigan Avenue", City = "South Loop", State = "IL", Zip = "60605" }
            };

        private static int _currentAddressIndex = -1;

        public static AddressViewModel GetMockAddress()
        {
            _currentAddressIndex = (_currentAddressIndex + 1) % _addresses.Count;
            return _addresses[_currentAddressIndex];
        }
    }
}