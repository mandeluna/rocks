using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Rocks_for_Food
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Biome biome;
        Texture2D biomeTexture, geologyTexture;

        readonly int width = 1024;
        readonly int height = 1024;

        readonly double timeScale = 86400.0; // one second = one day of game time
        DateTime worldTime = new DateTime(1, 1, 1, 0, 0, 0);

        private MouseState previousMouseState;
        private float zoomLevel = 1.0f;
        private Vector2 panLevel = new Vector2(0, 0);
        private bool showBiome = true;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Console.WriteLine("Creating biome...");
            biome = new Biome(width, height);
            Console.WriteLine("...biome created");

            biomeTexture = CreateBiomeTexture();
            geologyTexture = CreateGeologyTexture();

            this.IsMouseVisible = true;
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Status");
        }

        private Color BiomeColor(Biome.Geology type)
        {
            switch (type)
            {
                case Biome.Geology.SAND:
                    return Color.Tan;
                case Biome.Geology.GRAVEL:
                    return Color.DarkGray;
                case Biome.Geology.SALT:
                    return Color.WhiteSmoke;
                case Biome.Geology.TIN:
                    return Color.Gray;
                case Biome.Geology.COPPER:
                    return Color.DarkSeaGreen;
                case Biome.Geology.IRON:
                    return Color.DarkRed;
                case Biome.Geology.SILVER:
                    return Color.Silver;
                case Biome.Geology.GOLD:
                    return Color.Gold;
                default:
                    return Color.Black;
            }
        }

        private Color BiomeColor(Biome.Climate type)
        {
            switch (type)
            {
                case Biome.Climate.OCEAN:
                    return Color.DarkBlue;
                case Biome.Climate.BEACH:
                    return Color.Tan;
                case Biome.Climate.SCORCHED:
                    return Color.DarkRed;
                case Biome.Climate.BARE:
                    return Color.Gray;
                case Biome.Climate.TUNDRA:
                    return Color.LightGray;
                case Biome.Climate.SNOW:
                    return Color.WhiteSmoke;
                case Biome.Climate.TEMPERATE_DESERT:
                    return Color.LightGoldenrodYellow;
                case Biome.Climate.SHRUBLAND:
                    return Color.LawnGreen;
                case Biome.Climate.TAIGA:
                    return Color.DarkKhaki;
                case Biome.Climate.GRASSLAND:
                    return Color.LightGreen;
                case Biome.Climate.TEMPERATE_DECIDUOUS_FOREST:
                    return Color.ForestGreen;
                case Biome.Climate.TEMPERATE_RAIN_FOREST:
                    return Color.LightSeaGreen;
                case Biome.Climate.SUBTROPICAL_DESERT:
                    return Color.OliveDrab;
                case Biome.Climate.TROPICAL_SEASONAL_FOREST:
                    return Color.SeaGreen;
                case Biome.Climate.TROPICAL_RAIN_FOREST:
                    return Color.DarkGreen;
                
                default:
                    return Color.Black;
            }
        }

        private Texture2D CreateBiomeTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);

            Color[] data = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = BiomeColor(biome.ClimateType(x, y));
                    int offset = x + y * width;
                    data[offset] = pixel;
                }
            }

            texture.SetData(data);

            return texture;
        }

        private Texture2D CreateGeologyTexture()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height);

            Color[] data = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = BiomeColor(biome.GeologyType(x, y));
                    int offset = x + y * width;
                    data[offset] = pixel;
                }
            }

            texture.SetData(data);

            return texture;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update world time

            double elapsedMilliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
            worldTime = worldTime.AddMilliseconds(elapsedMilliseconds * timeScale);

            base.Update(gameTime);

            // Handle input events

            MouseState mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue < previousMouseState.ScrollWheelValue)
            {
                zoomLevel *= 0.9f;
            }
            else if (mouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue)
            {
                zoomLevel *= 1.1f;
            }

            // handle drag
            if (GraphicsDevice.Viewport.Bounds.Contains(mouseState.Position) &&
                mouseState.LeftButton.Equals(ButtonState.Pressed) && previousMouseState.LeftButton.Equals(ButtonState.Pressed))
            {
                panLevel.X += (mouseState.Position.X - previousMouseState.Position.X) / zoomLevel;
                panLevel.Y += (mouseState.Position.Y - previousMouseState.Position.Y) / zoomLevel;
            }

            if (mouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed)
            {
                showBiome = !showBiome;
            }

            previousMouseState = mouseState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw Game Map

            var scale = Matrix.CreateScale(zoomLevel, zoomLevel, 1.0f);
            var translate = Matrix.CreateTranslation(panLevel.X, panLevel.Y, 0);
            var matrix = translate * scale;

            spriteBatch.Begin(transformMatrix: matrix);

            if (showBiome)
            {
                spriteBatch.Draw(biomeTexture, new Vector2(0, 0), Color.White);
            }
            else
            {
                spriteBatch.Draw(geologyTexture, new Vector2(0, 0), Color.White);
            }

            spriteBatch.End();

            // Draw Static Content

            string status = string.Format("Day {0} of Year {1}", worldTime.DayOfYear, worldTime.Year);
            var mousePos = new Vector2(previousMouseState.Position.X, previousMouseState.Position.Y);
            var worldPos = Vector2.Transform(mousePos, Matrix.Invert(matrix));

            spriteBatch.Begin();

            spriteBatch.DrawString(font, status, new Vector2(10, 10), Color.Black);
            if (worldPos.X >= 0 && worldPos.Y >= 0 && worldPos.X < width && worldPos.Y < height)
            {
                var climate = biome.ClimateType((int)worldPos.X, (int)worldPos.Y);
                var geology = biome.GeologyType((int)worldPos.X, (int)worldPos.Y);
                string biomeString = string.Format("Climate: {0}, Geology: {1}", climate, geology);
                spriteBatch.DrawString(font, biomeString, new Vector2(10, 30), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
