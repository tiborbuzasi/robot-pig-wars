/**
 * Game Logic - Player
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

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
