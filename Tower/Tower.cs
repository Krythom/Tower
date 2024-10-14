using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.HighPerformance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Tower
{
    public class Tower : Game
    {
        //0 to skip drawing, 1 for base speed, higher for faster
        private const int _speedup = 100;
        private const int WindowX = 500;
        private const int WindowY = 500;
        private const int WorldY = 500;
        private const int WorldX = 500;
        private const float MutationStrength = 3f;
        private const double Rarity = 0.8;
        private const bool BatchMode = false;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _tex;
        
        private int seed;
        private Random rand;
        
        private ColorSpace _startCol;
        
        private bool _completed;
        private bool _saved;
        private int _iterations;

        private int buildX = 0;
        private int buildHeight = 0;
        private Seed[] seeds;
        private Cell[,] world;
        private Color[] _backingColors;
        private Memory2D<Color> _colors;

        public Tower()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            seed = Environment.TickCount;
            rand = new Random(seed);
            _startCol = new RGB(rand.Next(256), rand.Next(256), rand.Next(256));

            InitWorld();
            _graphics.PreferredBackBufferHeight = WindowY;
            _graphics.PreferredBackBufferWidth = WindowX;
            _graphics.SynchronizeWithVerticalRetrace = false;
            _graphics.ApplyChanges();

            InactiveSleepTime = new TimeSpan(0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _completed = true;

            foreach (Seed seed in seeds)
            {
                if (seed.Alive())
                {
                    _completed = false;
                }
            }

            if (_completed)
            {
                if (!_saved)
                {
                    SaveImage();
                    _saved = true;
                }

                if (!BatchMode)
                {
                    return;
                }

                _completed = false;
                _saved = false;
                Initialize();
                _iterations = 0;
            }
            else
            {
                Iterate();
                _iterations++;
                if (_speedup == 0 || _iterations % _speedup != 0)
                {
                    SuppressDraw();
                }
            }

            double ms = gameTime.ElapsedGameTime.TotalMilliseconds;
            Debug.WriteLine(
                "fps: " + (1000 / ms) + " (" + ms + "ms)" + " iterations: " + _iterations
            );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            _tex.SetData(_backingColors);
            _spriteBatch.Draw(_tex, new Rectangle(0,0,WindowX,WindowY), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitWorld()
        {
            world = new Cell[WorldX, WorldY];
            seeds = new Seed[WorldX];
            _backingColors = new Color[WorldX * WorldY];
            _colors = new Memory2D<Color>(_backingColors, WorldX, WorldY);
            _tex = new Texture2D(GraphicsDevice, WorldX, WorldY);

            foreach (ref Color c in _backingColors.AsSpan())
                c = Color.Black;

            for (int x = 0; x < WorldX; x++)
            {
                for (int y = 0; y < WorldY; y++)
                {
                    world[x, y] = new Void();
                    world[x, y].Position = new Point(x, y);
                }
                seeds[x] = new Seed(_startCol);
            }
        }

        private void Iterate()
        {
            //Color mutation
            //Change to be based on neghbors?
            _startCol.Mutate(MutationStrength, rand);
            var colors = _colors.Span;

            //Move builder
            buildX = (buildX + 1) % WorldX;

            //Increment builder size
            //Change how much this increments based on tower size maybe
            buildHeight++;

            //Check to build
            if (rand.NextDouble() > Rarity)
            {
                //Build
                //Add widths?
                Seed s = seeds[buildX];

                if (s.Alive())
                {
                    if (buildHeight > s.GetHeight())
                    {
                        for (int y = Math.Max(WorldY - buildHeight, 0); y < WorldY - s.GetHeight(); y++)
                        {
                            world[buildX, y] = new Block(_startCol, buildX, y);
                            colors[buildX, y] = _startCol.ToColor();
                        }
                        s.SetHeight(buildHeight);
                        buildHeight = 0;

                    }
                    else
                    {
                        s.SetLiving(false);
                        buildHeight = 0;
                    }
                }
            }
        }

        private void SaveImage()
        {
            unsafe
            {
                fixed (void* ptr = _backingColors)
                {
                    var img = Image.WrapMemory<Rgba32>(
                        ptr,
                        _backingColors.AsSpan().AsBytes().Length,
                        WorldX,
                        WorldY
                    );

                    string date = DateTime.Now.ToString("s").Replace("T", " ").Replace(":", "-");

                    img.Save(
                        $"_i{_iterations}.png",
                        new PngEncoder()
                    );
                }
            }
        }
    }
}