using System;
using Microsoft.Xna.Framework;

namespace Tower
{
    internal class Block : Cell
    {
        public Block(ColorSpace col, int x, int y)
        {
            Color = col;
            Type = CellType.Block;
            Position = new Point(x, y);
        }
    }
}