using System;
using System.Collections.Generic;

namespace USARoadTrip.Silverlight.ViewModels
{
    public class LocationViewModel
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string ShortLocationString
        {
            get
            {
                return String.Format("{0}, {1}, {2}", State, City, Address);
            }
        }

        public Dictionary<string, string> ToKeyValue(Dictionary<string, string> address)
        {
            if(address == null)
                address = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Address))
                address.Add("Street", Address);
            if (!string.IsNullOrEmpty(City))
                address.Add("City", City);
            if (!string.IsNullOrEmpty(State))
                address.Add("State", State);
            if (!string.IsNullOrEmpty(Zip))
                address.Add("ZIP", Zip);

            return address;
        }
    }
}
