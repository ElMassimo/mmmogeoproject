
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using USARoadTrip.Silverlight.Utility;

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
