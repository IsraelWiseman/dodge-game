using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodge_Game_2
{
    public class Gift : Player
    {
        public Gift(int height, int width, double positionTop, double positionLeft, string imagePlayer) 
            : base(height, width, positionTop, positionLeft, imagePlayer)
        {
        }
    }
}
