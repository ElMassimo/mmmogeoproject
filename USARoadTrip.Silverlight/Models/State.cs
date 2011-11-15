
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
namespace USARoadTrip.Silverlight.Models
{
    public class State
    {
        public string Name { get; set; }
        public IList<Graphic> Counties { get; set; }
    }
}
