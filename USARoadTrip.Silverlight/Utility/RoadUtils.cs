using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Projection;
using System.Collections.Generic;

namespace USARoadTrip.Silverlight.Utility
{
    public static class RoadUtils
    {
        public const int CAR_MINIMUM_SPEED = 50;
        public const int CAR_MAXIMUM_SPEED = 140;

        private const double SHORTEST_SIMULATION_TIME_IN_HOURS = 0.25;
        private const double LONGEST_SIMULATION_TIME_IN_HOURS = 10;

        private const string CAR_IMAGES_PATH = "../Assets/Images/Cars/car_{0}.png";
        private static Random _numberGenerator = new Random();

        private const double EarthRadius = 6378.137;
        private static WebMercator _geographicHelper = new WebMercator();

        public static BitmapImage GetCarImage(object carName)
        {
            return new BitmapImage(new Uri(String.Format(CAR_IMAGES_PATH, carName), UriKind.Relative));
        }

        private static double GetSphericalDistance(this MapPoint start, MapPoint end)
        {
            double lon1 = start.X / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((lat1 - lat2) / 2)), 2) +
             Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2))) * EarthRadius;
        }

        public static double GetDistanceInKilometers(this MapPoint firstPoint, MapPoint lastPoint)
        {
            firstPoint = _geographicHelper.ToGeographic(firstPoint) as MapPoint;
            lastPoint = _geographicHelper.ToGeographic(lastPoint) as MapPoint;
            return firstPoint.GetSphericalDistance(lastPoint);
        }

        public static double GetTotalDistance(this IEnumerable<MapPoint> locations)
        {
            double totalDistance = 0;
            MapPoint lastPoint = null;
            foreach (MapPoint currentPoint in locations)
            {
                if(lastPoint != null)
                    totalDistance += GetDistanceInKilometers(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
            return totalDistance;
        }

        public static double GetRandomSectionSpeed()
        {
            return CAR_MINIMUM_SPEED + (CAR_MAXIMUM_SPEED - CAR_MINIMUM_SPEED) * _numberGenerator.NextDouble();
        }

        public static double GetTimeBetweenGpsEmissions(double ratio)
        {
            return SHORTEST_SIMULATION_TIME_IN_HOURS + (LONGEST_SIMULATION_TIME_IN_HOURS - SHORTEST_SIMULATION_TIME_IN_HOURS) * ratio;
        }

        public static Envelope GetCenteredEnvelope(MapPoint point, double displaySize)
        {
            double offset = displaySize / 2;
            return new Envelope(point.X - offset, point.Y - offset, point.X + offset, point.Y + offset);
        }

        public static MapPoint Last(this PointCollection points)
        {
            return points[points.Count - 1];
        }
    }
}
