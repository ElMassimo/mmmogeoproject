
using ESRI.ArcGIS.Client.Geometry;
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
namespace SIGTest.Models
{
    public class State
    {
        public string Name { get; set; }
        public IList<Graphic> Counties { get; set; }
    }
}
