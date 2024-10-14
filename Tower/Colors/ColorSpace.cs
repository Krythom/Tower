using Microsoft.Xna.Framework;
using System;

namespace Tower
{
    public abstract class ColorSpace
    {
        public abstract void Mutate(float strength, Random rand);

        public abstract Color ToColor();

        public abstract double GetDistance(ColorSpace other);
    }
}
