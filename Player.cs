using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Dodge_Game_2
{
    public abstract class Player // base class
    {
           //properties
        public int _height { get; set; }
        public int _width { get; set; }
        public int step { get; set; }
        public double _positionTop { get; set; }
        public double _positionLeft { get; set; }
        public string image { get; set; }
        public Image _image { get; set; }
        
        //constructor
        public Player(int height, int width, double positionTop, double positionLeft, string imagePlayer)
        {
            _width = width;
            _height = height;
            _positionTop = positionTop;
            _positionLeft = positionLeft;
            image = imagePlayer;
            _image = SetImage();
            Canvas.SetLeft(_image, _positionLeft);
            Canvas.SetTop(_image, _positionTop);
            step = 3;
        }
        public Image SetImage()                                       
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(image));
            img.Width = _width;
            img.Height = _height;

            return img;
        }

        public double ImageLeft()                         
        {
            return Canvas.GetLeft(_image);
        }
        public double ImageTop()                             
        {
            return Canvas.GetTop(_image);
        }



    }

   
}
