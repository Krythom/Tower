using System;
using Microsoft.Xna.Framework;

namespace Tower
{
    internal class Seed
    {
        int height;
        bool living;
        ColorSpace Color;

        public Seed(ColorSpace col)
        {
            Color = col;
            height = 0;
            living = true;
        }

        public int GetHeight()
        {
            return height;
        }

        public bool Alive()
        {
            return living;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public void SetLiving(bool living)
        {
            this.living = living;
        }

        public ColorSpace GetColor()
        {
            return Color;
        }
    }
}