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

namespace ProgrammingAssignment2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // STUDENTS: declare variables for three sprites
        Texture2D guy0;
        Texture2D guy1;
        Texture2D guy2;

        // STUDENTS: declare variables for x and y speeds
        int xSpeed;
        int ySpeed;

        // used to handle generating random values
        Random rand = new Random();
        const int CHANGE_DELAY_TIME = 1000;
        int elapsedTime = 0;

        // used to keep track of current sprite and location
        Texture2D currentSprite;
        Rectangle drawRectangle = new Rectangle();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
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

            // STUDENTS: load the sprite images here
            guy0 = Content.Load<Texture2D>("guy0");
            guy1 = Content.Load<Texture2D>("guy1");
            guy2 = Content.Load<Texture2D>("guy2");

            // STUDENTS: set the currentSprite variable to one of your sprite variables
            currentSprite = guy0;

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

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime > CHANGE_DELAY_TIME)
            {
                elapsedTime = 0;

                // STUDENTS: uncomment the code below and make it generate a random number 
                // between 0 and 2 inclusive using the rand field I provided

                // get random integer number, that is smaller than 3(meaning 0,1 or 2)
                int spriteNumber = rand.Next(3);

                // sets current sprite
                // STUDENTS: uncomment the lines below and change sprite0, sprite1, and sprite2
                //      to the three different names of your sprite variables
                if (spriteNumber == 0)
                {
                    currentSprite = guy0;
                }
                else if (spriteNumber == 1)
                {
                    currentSprite = guy1;
                }
                else if (spriteNumber == 2)
                {
                    currentSprite = guy2;
                }

                // STUDENTS: set the drawRectangle.Width and drawRectangle.Height to match the width and height of currentSprite

                // rectanlge size is same as size of current sprite
                drawRectangle.Width  = currentSprite.Width;
                drawRectangle.Height = currentSprite.Height;


                // STUDENTS: center the draw rectangle in the window. Note that the X and Y properties of the rectangle
                // are for the upper left corner of the rectangle, not the center of the rectangle
                drawRectangle.X = (int) (0.5 * (WINDOW_WIDTH  - currentSprite.Width ));
                drawRectangle.Y = (int) (0.5 * (WINDOW_HEIGHT - currentSprite.Height));


                // STUDENTS: write code below to generate random numbers  between -4 and 4 inclusive for the x and y speed 
				// using the rand field I provided
                // CAUTION: Don't redeclare the x speed and y speed variables here!

                // now we can set the speed
                xSpeed = rand.Next(-4,5);
                ySpeed = rand.Next(-4,5);


            }

            // STUDENTS: move the drawRectangle by the x speed and the y speed

            // rectanlge position is old position + respective speeds
            drawRectangle.X += xSpeed;
            drawRectangle.Y += ySpeed;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // STUDENTS: draw current sprite here
            spriteBatch.Begin();

            spriteBatch.Draw(currentSprite, drawRectangle, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
