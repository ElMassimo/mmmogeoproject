using System.Collections.Generic;
using ESRI.ArcGIS.Client.Geometry;

namespace USARoadTrip.Silverlight.Models
{
    public class Road
    {
        private Polyline Polyline { get; set; }
        private IEnumerator<MapPoint> _points;

        public Road(Polyline road)
        {
            Polyline = road;
            _points = MapPoints.GetEnumerator();

        }

        public MapPoint StartLocation
        {
            get { return Polyline.Paths[0][0]; }
        }

        public MapPoint LastLocation
        {
            get 
            {
                var lastPath = Polyline.Paths[Polyline.Paths.Count - 1];
                return lastPath[lastPath.Count - 1];
            }
        }

        public void GoToStart()
        {
            _points.Dispose();
            _points = MapPoints.GetEnumerator();
        }

        public MapPoint NextLocation()
        {
            if(_points.MoveNext())
                return _points.Current;
            else
                return null;
        }

        public IEnumerable<MapPoint> MapPoints
        {
            get
            {                
                foreach (PointCollection path in Polyline.Paths)
                {
                    foreach (MapPoint mapPoint in path)
                    {
                        yield return mapPoint;
                    }
                }            
            }
        }
    }
}
