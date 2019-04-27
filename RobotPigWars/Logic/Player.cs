using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPigWars.Logic
{
    class Player
    {
        public string Name { get; set; }
        public int Life { get; set; }
        public int[] Position { get; set; }
        public Directions Face { get; set; }
        public Actions[] Steps { get; set; }

        public Player(int life, int x, int y, Directions face, Actions[] steps)
        {
            Life = life;
            Position = new int[2] { x, y };
            Face = face;
            Steps = steps;
        }
    }
}
