using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PlatformerMG;

namespace PlatformerMG
{
    public class MenuState : State
    {
        private List<Component> _components;
        Texture2D introBackground;

        public MenuState(PlatformerGame game, GraphicsDevice graphicsDevice, ContentManager content)
          : base(game, graphicsDevice, content)
        {
            var buttonTexture = content.Load<Texture2D>("Controls/Button");
            var buttonFont = content.Load<SpriteFont>("Fonts/gameFont");
            introBackground = content.Load<Texture2D>("Overlays/background_menu");

            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(500,50),
                Text = "New Game",
            };

            newGameButton.Click += NewGameButton_Click;

            var highScoresButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(500, 175),
                Text = "High Scores",

            };

            highScoresButton.Click += highScoresButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(500, 300),
                Text = "Quit Game",
            };

            quitGameButton.Click += QuitGameButton_Click;

            _components = new List<Component>()
      {
        newGameButton,
        highScoresButton,
        quitGameButton,
      };
    }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(); 
            spriteBatch.Draw(introBackground, Vector2.Zero, Color.White);
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void highScoresButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new HighScoreState(_game, _graphicsDevice, _content));
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }
    }
}