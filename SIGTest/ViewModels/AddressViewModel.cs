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
using System.Collections.Generic;

namespace SIGTest.ViewModels
{
    public class AddressViewModel
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

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

        public void Clear()
        {
            Zip = State = City = Address = null;
        }
    }
}
