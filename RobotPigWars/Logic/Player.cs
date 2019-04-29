using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPigWars.Logic
{
    class Player
    {
        // Name of the player
        public string Name { get; set; }
        // Remaining lives of the player
        public int Life { get; set; }
        // Position of the player (X and Y coordinates)
        public int[] Position { get; set; }
        // Facing direction of the player
        public Directions Face { get; set; }
        // Steps of the player
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
