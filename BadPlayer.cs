using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Dodge_Game_2
{
    public class BadPlayer : Player
    {
        public BadPlayer(int height, int width,  double positionTop, double positionLeft, string playerImage)
            : base(height, width, positionTop, positionLeft, playerImage)
        {

        }
    }
}
