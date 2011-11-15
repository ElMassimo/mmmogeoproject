using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace USARoadTrip.Api.Models
{
    [DataContract]
    public class Trip
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string UserNick { get; set; }

        [DataMember]
        public List<Location> Destinations { get; set; }
    }
}
