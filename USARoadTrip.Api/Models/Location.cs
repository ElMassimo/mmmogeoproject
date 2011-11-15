using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace USARoadTrip.Api.Models
{
    [DataContract]
    public class Location
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public int TripOrder { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
    }
}
