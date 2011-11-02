using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using ESRI.ArcGIS.Client.Geometry;

namespace SIGTest.Utility
{
    public static class RoadUtils
    {
        private const string CAR_IMAGES_PATH = "../Assets/Images/Cars/car_{0}.png";

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
    }
}
