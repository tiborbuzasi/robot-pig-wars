﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPigWars.Logic
{
    class Game
    {
        public enum GameState { Input, Process, GameOver };
        public static readonly byte[] fieldSizes = { 4, 6, 8 };

        public const int numberOfLives = 3;
        public const byte numberOfPlayers = 2;
        public const byte numberOfSteps = 5;
        public readonly Actions[] movementActions = {
            Actions.Forward,
            Actions.Backward,
            Actions.MoveLeft,
            Actions.MoveRight
        };

        public GameState State { get; private set; }
        public byte FieldSize { get; private set; }
        public byte CurrentStep { get; private set; }
        public byte CurrentPlayer { get; private set; }
        public Player[] Players { get; private set; }

        public Game()
        {
            FieldSize = fieldSizes[1];
            State = GameState.GameOver;

            Players = new Player[2];
            Players[0] = new Player(
                numberOfLives,
                FieldSize / 2 - 1,
                FieldSize / 2 - 1,
                Directions.Down,
                Enumerable.Repeat(Actions.None, numberOfSteps).ToArray()
            );
            Players[1] = new Player(
                numberOfLives,
                FieldSize / 2 + 1,
                FieldSize / 2 + 1,
                Directions.Up,
                Enumerable.Repeat(Actions.None, numberOfSteps).ToArray()
            );

            CurrentStep = 0;
            CurrentPlayer = 0;
        }

        public void ProcessStep()
        {
            if (CurrentStep >= numberOfSteps)
            {
                return;
            }

            if (!ForecastCollision())
            {
                for (byte p = 0; p < numberOfPlayers; p++)
                {
                    Players[p].Position = CalculatePosition(Players[p]);
                }
            }

            for (byte p = 0; p < numberOfPlayers; p++)
            {
                switch (Players[p].Steps[CurrentStep])
                {
                    case Actions.Fist:
                        ProcessAction(Players[p], CalculateFist(Players[p]));
                        break;
                    case Actions.Gun:
                        ProcessAction(Players[p], CalculateGun(Players[p]));
                        break;
                }
            }

            CurrentStep++;
        }

        public bool ForecastCollision()
        {
            if (movementActions.Contains(Players[0].Steps[CurrentStep]) &&
                movementActions.Contains(Players[1].Steps[CurrentStep]) &&
                CalculatePosition(Players[0]) == CalculatePosition(Players[1]))
            {
                return true;
            }

            return false;
        }

        private int[] CalculatePosition(Player player)
        {
            int[] movement = new int[2] { 0, 0 };
            switch (player.Steps[CurrentStep])
            {
                case Actions.Forward:
                    movement = new int[2] { 1, 0 };
                    break;
                case Actions.Backward:
                    movement = new int[2] { -1, 0 };
                    break;
                case Actions.MoveLeft:
                    movement = new int[2] { 0, -1 };
                    break;
                case Actions.MoveRight:
                    movement = new int[2] { 0, 1 };
                    break;
            }

            switch (player.Face)
            {
                case Directions.Left:
                    movement.Select(i => -i).ToArray();
                    break;
                case Directions.Down:
                    movement.Reverse();
                    break;
                case Directions.Up:
                    movement.Select(i => -i).ToArray().Reverse();
                    break;
            }

            if (player.Steps[CurrentStep] == Actions.MoveLeft || player.Steps[CurrentStep] == Actions.MoveRight)
            {
                movement.Select(i => -i).ToArray();
            }

            return FixPosition(new int[2] { player.Position[0] + movement[0], player.Position[1] + movement[1] });
        }

        private int[] FixPosition(int[] position)
        {
            position[0] = (position[0] < 0) ? 0 : position[0];
            position[0] = (position[0] >= FieldSize) ? FieldSize : position[0];
            position[1] = (position[1] < 0) ? 0 : position[1];
            position[1] = (position[1] >= FieldSize) ? FieldSize : position[1];

            return position;
        }

        private void CheckState()
        {
            if (Players.Any(p => p.Life == 0))
            {
                State = GameState.GameOver;
            }
        }

        public void EndTurn()
        {
            switch (State)
            {
                case GameState.Input:
                    CurrentPlayer++;
                    break;
                case GameState.Process:
                    CurrentPlayer = 0;
                    CheckState();
                    break;
            }
        }

        public int[][] CalculateFist(Player player)
        {
            int[][] area = new int[2][];

            area[0] = FixPosition(new int[2] { player.Position[0] - 1, player.Position[1] - 1 });
            area[1] = FixPosition(new int[2] { player.Position[0] + 1, player.Position[1] + 1 });

            return area;
        }

        public int[][] CalculateGun(Player player)
        {
            int[][] area = new int[2][];

            switch (player.Face)
            {
                case Directions.Right:
                    area[0] = FixPosition(new int[2] { player.Position[0] + 1, player.Position[1] });
                    area[1] = FixPosition(new int[2] { 0, player.Position[1] });
                    break;
                case Directions.Left:
                    area[0] = FixPosition(new int[2] { player.Position[0] - 1, player.Position[1] });
                    area[1] = FixPosition(new int[2] { FieldSize - 1, player.Position[1] });
                    break;
                case Directions.Down:
                    area[0] = FixPosition(new int[2] { player.Position[0], player.Position[1] + 1 });
                    area[1] = FixPosition(new int[2] { player.Position[0], FieldSize - 1 });
                    break;
                case Directions.Up:
                    area[0] = FixPosition(new int[2] { player.Position[0], player.Position[1] - 1 });
                    area[1] = FixPosition(new int[2] { player.Position[0], 0 });
                    break;
            }

            return area;
        }

        private void ProcessAction(Player player, int[][] area)
        {
            Player[] effectedPlayers = Players.Where(p => p != player)
                .Where(p => p.Position[0] >= area[0][0] && p.Position[1] >= area[0][1])
                .Where(p => p.Position[0] <= area[1][0] && p.Position[1] <= area[1][1]).ToArray();
            foreach (Player effectedPlayer in effectedPlayers)
            {
                if (effectedPlayer.Life > 0)
                {
                    effectedPlayer.Life--;
                }
            }
        }
    }
}
