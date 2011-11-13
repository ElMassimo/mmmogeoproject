using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace USARoadTrip.Api.Models
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Nick { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
