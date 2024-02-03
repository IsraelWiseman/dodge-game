using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Dodge_Game_2
{
    public class GoodPlayer:Player
    {
        

        //properties
        public List<Image> Lives { get; set; }                                //list of lives images.
        public int lifeCounter{ get; set; }

        //constructor
        public GoodPlayer(int height, int width, double positionTop, double positionLeft,
            string imagePlayer) : base(height, width,  positionTop, positionLeft, imagePlayer)
        {
            lifeCounter = 3;
            Lives = new List<Image>();
            for (int i = 0; i < 3; i++)
            {
                Image life = new Image();
                life.Width = 25;
                life.Height = 25;
                life.Source = new BitmapImage(new Uri("ms-appx:///pictures/Green heart.png"));
                Canvas.SetTop(life, 0);                                     //position the image on the top left corner.
                Canvas.SetLeft(life,i*25);                                  //position the next 2 lives images right next to it.
                Lives.Add(life);                                            //add the images to the canvas.
            }
        }

        //public GoodPlayer(Image playerImage, Canvas canvas, int amountOfLives)
        //{
        //    this.playerImage = playerImage;
        //    this.canvas = canvas;
        //    this.amountOfLives = amountOfLives;
        //}

        //internal int LivesLeft()
        //{
        //   return(lifeCounter);
        //}
    }
}
