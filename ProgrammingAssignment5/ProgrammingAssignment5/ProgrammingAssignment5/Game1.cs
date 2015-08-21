using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TeddyMineExplosion;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        // mine support
        Texture2D mineSprite;
        List<Mine> mines = new List<Mine>();

        // teddy support
        Texture2D teddySprite;
        List<TeddyBear> teddies = new List<TeddyBear>();

        // explosion support
        Texture2D explosionSprite;
        List<Explosion> explosions = new List<Explosion>();

        // click processing
        bool leftClickStarted = false;
        bool leftButtonReleased = true;

        // used to handle generating random values
        Random rand = new Random();
        int spawnDelay = 1000;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // setting resolution
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            // make mouse visible
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // loading sprites
            mineSprite = Content.Load<Texture2D>("mine");
            teddySprite = Content.Load<Texture2D>("teddybear");
            explosionSprite = Content.Load<Texture2D>("explosion");

            // set initial spawn time in ms
            spawnDelay = rand.Next(1000, 3001);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // get current mouse state
            MouseState mouse = Mouse.GetState();

            // check for left click started
            if (mouse.LeftButton == ButtonState.Pressed &&
                leftButtonReleased)
            {
                leftClickStarted = true;
                leftButtonReleased = false;
            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                leftButtonReleased = true;

                // if left click finished, add new mine to list
                if (leftClickStarted)
                {
                    leftClickStarted = false;

                    // add a new mine to the end of the list of mines
                    mines.Add(new Mine(mineSprite, mouse.X, mouse.Y));

                }
            }

            // spawn teddy bear, if enough time elapsed
            spawnDelay -= gameTime.ElapsedGameTime.Milliseconds;
            if (spawnDelay <= 0)
            {
                spawnDelay = rand.Next(1000, 3001);
                Vector2 velocity = new Vector2((float)rand.NextDouble() - 0.5f, (float)rand.NextDouble() - 0.5f);
                teddies.Add(new TeddyBear(teddySprite, velocity, WINDOW_WIDTH, WINDOW_HEIGHT));
            }

            // update teddie bears
            foreach (TeddyBear teddy in teddies)
            {
                teddy.Update(gameTime);
            }


            // update explosions
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // detect collisions between teddies and mines
            foreach (TeddyBear teddy in teddies)
            {
                foreach (Mine mine in mines)
                {
                    if (mine.Active && teddy.Active)
                    {
                        // collision detected
                        if (teddy.CollisionRectangle.Intersects(mine.CollisionRectangle))
                        {
                            // set mine and teddy to inactive
                            teddy.Active = false;
                            mine.Active = false;
                            explosions.Add(new Explosion(explosionSprite, 
                                            mine.CollisionRectangle.Center.X,
                                            mine.CollisionRectangle.Center.Y));
                        }
                    }
                }

            }

            // remove inactive teddies
            for (int i = teddies.Count()-1; i >= 0; --i)
            {
                if (!teddies[i].Active)
                {
                    teddies.RemoveAt(i);
                }
            }

            // remove inactive mines
            for (int i = mines.Count()-1; i >= 0; --i)
            {
                if (!mines[i].Active)
                {
                    mines.RemoveAt(i);
                }
            }

            // remove inactive explosions
            for (int i = explosions.Count()-1; i >= 0; --i)
            {
                if (!explosions[i].Playing)
                {
                    explosions.RemoveAt(i);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // draw game objects
            spriteBatch.Begin();

            // Draw mines
            foreach (Mine mine in mines)
            {
                mine.Draw(spriteBatch);
            }
            // Draw teddies
            foreach (TeddyBear teddy in teddies)
            {
                teddy.Draw(spriteBatch);
            }
            // Draw explosions
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
