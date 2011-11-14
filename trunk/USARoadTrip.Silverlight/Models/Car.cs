
using ESRI.ArcGIS.Client.Geometry;
using System.Windows.Controls;
using USARoadTrip.SilverlightUtility;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace USARoadTrip.Silverlight.Models
{
    public class Car
    {
        private int _imageIndex = -1;
        private const int CAR_IMAGES_COUNT = 5;
        private MapPoint _currentLocation = null;
        private MapPoint _nextLocation = null;

        private string StateName { get; set; }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _nextLocation = value;
                _currentLocation = value;
            }
        }

        public MapPoint NextLocation
        {
            get { return _nextLocation; }
            set 
            {
                _currentLocation = _nextLocation;
                _nextLocation = value; 
            }
        }
        
        public Car(string stateName, MapPoint startingLocation)
        {
            CurrentLocation = startingLocation;
            UpdateState(stateName);
        }

        public void UpdateState(string stateName)
        {
            if (StateName != stateName)
            {
                StateName = stateName;
                _imageIndex = (_imageIndex + 1) % CAR_IMAGES_COUNT;
            }
        }

        public Symbol GetSymbol()
        {
            PictureMarkerSymbol carSymbol = new PictureMarkerSymbol();
            carSymbol.Source = RoadUtils.GetCarImage(_imageIndex);
            carSymbol.OffsetX = 20;
            carSymbol.OffsetY = 15;
            carSymbol.Width = 40;
            carSymbol.Height = 30;
            return carSymbol;
        }
    }
}
