#region File Description
//-----------------------------------------------------------------------------
// PlatformerGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Reflection;
using PlatformerMG;


namespace PlatformerMG
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        public bool GameStarted = false;
        public bool GameFinished = false;
        Camera camera;

        List<Rectangle> collisionRects;
        CollisionManager collisionManager;
        Rectangle startRect;
        Rectangle endRect;

        // Resources for drawing.
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        private State currentState;
        private State nextState;

        // Image used to display the static background
        Texture2D mainBackground;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // govern how fast our laser can fire.
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;


        // Global content.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        private bool wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;
        private TouchCollection touchState;
        private AccelerometerState accelerometerState;

        public int _score;
        public ScoreManager scoreManager;
        
        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        private const int numberOfLevels = 3;


        public void ChangeState(State state)
        {
            nextState = state;
        }
        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if WINDOWS_PHONE
            graphics.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

            Accelerometer.Initialize();

            //Background
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            collisionManager = new CollisionManager();



        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            IsMouseVisible = true;

            scoreManager = ScoreManager.Load();

            currentState = new MenuState(this, graphics.GraphicsDevice, Content);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
            // Load fonts
            hudFont = Content.Load<SpriteFont>("Fonts/gameFont");

            // Load overlay textures
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "Backgrounds/Layers/back_decor", GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Backgrounds/Layers/back_decor", GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Backgrounds/Layers/mainbackground");



            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
            }
            catch { }

            LoadNextLevel();

            foreach (var o in level.TileMapInfo)
            {
                collisionManager.AddCollidable(o.Item1, o.Item2);
            }

            camera = new Camera();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if(nextState != null) 
            {
                currentState = nextState; 
                 nextState = null;
            }
            currentState.Update(gameTime);
            currentState.PostUpdate(gameTime);

            // Handle polling for our input and handling high-level input
            //Restart(gameTime);

            if(GameStarted && !GameFinished)
            {
                level.Update(gameTime, keyboardState, gamePadState, touchState,
             accelerometerState, Window.CurrentOrientation);

                camera.Follow(level.Player, graphics);

                // update our level, passing down the GameTime along with all of our input states
                base.Update(gameTime);

                // Update the parallaxing background
                bgLayer1.Update(gameTime);
                bgLayer2.Update(gameTime);

                collisionManager.Update(level.Player);
            }




        }

        private void Restart()
        {
            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (!level.Player.IsAlive)
            {
                LoadNextLevel();
                level.StartNewLife();
            }
            else if (level.TimeRemaining == TimeSpan.Zero)
            {
                LoadNextLevel();
                level.StartNewLife();
            }
        }
        private void LoadNextLevel()
        {
            // move to the next level
            //levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            //string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);

            string levelPath = string.Format("Content/Levels/{0}.txt",1);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
            level.CollisionTiles();

        }

        //public void ReloadCurrentLevel()
        //{
        //    --levelIndex;
        //    LoadNextLevel();
        //}

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


                spriteBatch.Begin();

                spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
                // Draw the moving background
                bgLayer1.Draw(spriteBatch);
                bgLayer2.Draw(spriteBatch);
                spriteBatch.End();

                //Draw the Main Background Texture
                spriteBatch.Begin(transformMatrix: camera.Transform);
                level.Draw(gameTime, spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin();
                DrawHud();
                spriteBatch.End();

            





            currentState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }

        private void EndGame()
        {
            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedExit)
                {
                    scoreManager.Add(new Score()
                    {
                        PlayerName = "Bora",
                        Value = level.Score,

                    });
                    _score = level.Score;
                    ScoreManager.Save(scoreManager);
                    nextState = new EndState(this, graphics.GraphicsDevice, Content);
                    GameFinished = true;
                    Restart();
                }
                else
                {
                    scoreManager.Add(new Score()
                    {
                        PlayerName = "Bora",
                        Value = level.Score,

                    });
                    _score = level.Score;
                    ScoreManager.Save(scoreManager);
                    nextState = new EndState(this, graphics.GraphicsDevice, Content);
                    GameFinished = true;
                    Restart();
                }
            }
            else if (!level.Player.IsAlive)
            {
                scoreManager.Add(new Score()
                {
                    PlayerName = "Bora",
                    Value = level.Score,

                });
                _score = level.Score;
                ScoreManager.Save(scoreManager);
                nextState = new EndState(this, graphics.GraphicsDevice, Content);
                GameFinished = true;
                Restart();
            }

        }
        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);

            // Draw Number of Lives
            DrawShadowedString(hudFont, "LIVES: " + level.Player.Lives.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 2.4f), Color.Yellow);

            EndGame();

        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
