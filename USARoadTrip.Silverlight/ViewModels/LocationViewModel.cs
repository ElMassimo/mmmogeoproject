using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Client.Geometry;
using USARoadTrip.Silverlight.WCFServices;
using USARoadTrip.Silverlight.Utility;

namespace USARoadTrip.Silverlight.ViewModels
{
    public class LocationViewModel
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public MapPoint Point { get; set; }

        public string ShortLocationString
        {
            get
            {
                return String.Format("{0}, {1}, {2}", State, City, Address);
            }
        }

        public bool IsValid
        {
            get
            {
                return !String.IsNullOrWhiteSpace(Address)
                    && !String.IsNullOrWhiteSpace(City)
                    && !String.IsNullOrWhiteSpace(State);
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

        public Location ToDataContract(int tripOrder)
        {
            return new Location()
            {
                Address = this.Address,
                City = this.City,
                State = this.State,
                Zip = this.Zip,
                X = Point.X,
                Y = Point.Y,
                TripOrder = tripOrder
            };
        }

        public static LocationViewModel FromDataContract(Location location)
        {
            return new LocationViewModel()
            {
                Address = location.Address,
                City = location.City,
                State = location.State,
                Zip = location.Zip,
                Point = new MapPoint(location.X, location.Y, new SpatialReference(RoadUtils.SPATIAL_REFERENCE_WKID))
            };
        }
    }
}
