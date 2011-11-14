using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using ESRI.ArcGIS.Client.Geometry;

namespace USARoadTrip.SilverlightUtility
{
    public static class RoadUtils
    {
        private const int CAR_MINIMUM_SPEED = 60;
        private const int CAR_MAXIMUM_SPEED = 140;
        private const string CAR_IMAGES_PATH = "../Assets/Images/Cars/car_{0}.png";
        private static Random _numberGenerator = new Random();

        public static BitmapImage GetCarImage(object carName)
        {
            return new BitmapImage(new Uri(String.Format(CAR_IMAGES_PATH, carName), UriKind.Relative));
        }

        public static double CalculateDistance(MapPoint firstPoint, MapPoint lastPoint)
        {
            double x = lastPoint.X - firstPoint.X;
            double y = lastPoint.Y - firstPoint.Y;
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public static double GetRandomSectionSpeed()
        {
            return CAR_MINIMUM_SPEED + (CAR_MAXIMUM_SPEED - CAR_MINIMUM_SPEED) * _numberGenerator.NextDouble();
        }

        public static Envelope GetCenteredEnvelope(MapPoint point, double displaySize)
        {
            double offset = displaySize / 2;
            return new Envelope(point.X - offset, point.Y - offset, point.X + offset, point.Y + offset);
        }
    }
}
