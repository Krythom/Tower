using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tower
{
    public abstract class Cell
    {
        public CellType Type;
        public Point Position;
        public ColorSpace Color;
        
        protected int _xMax;
        protected int _yMax;

        public object DeepCopy()
        {
            Cell copy = (Cell) MemberwiseClone();
            copy.Position = Position;
            copy.Color = Color;
            copy._yMax = _yMax;
            copy._xMax = _xMax;
            return copy;
        }

        public List<Cell> GetMoore(Cell[,] world)
        {
            var neighbors = new List<Cell>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        neighbors.Add(world[Mod(Position.X + x, _xMax), Mod(Position.Y + y, _yMax)]);
                    }
                }
            }

            return neighbors;
        }

        public List<Cell> GetNeumann(Cell[,] world)
        {
            var neighbors = new List<Cell>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x != y && x != -y)
                    {
                        neighbors.Add(world[Mod(Position.X + x, _xMax), Mod(Position.Y + y, _yMax)]);
                    }
                }
            }

            return neighbors;
        }

        private static int Mod(int x, int m)
        {
            return (Math.Abs(x * m) + x) % m;
        }

        public enum CellType
        {
            Void,
            Seed,
            Block,
        }
    }
}