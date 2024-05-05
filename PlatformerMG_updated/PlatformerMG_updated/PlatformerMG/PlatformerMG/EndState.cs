using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PlatformerMG;

namespace PlatformerMG
{
    public class EndState : State
    {
        private List<Component> _components;
        Texture2D introBackground;
        SpriteFont _buttonFont;
        ScoreManager _scoreManager;

        public EndState(PlatformerGame game, GraphicsDevice graphicsDevice, ContentManager content)
          : base(game, graphicsDevice, content)
        {
            var buttonTexture = content.Load<Texture2D>("Controls/Button");
            _buttonFont = content.Load<SpriteFont>("Fonts/gameFont");
            introBackground = content.Load<Texture2D>("Overlays/background_menu");

            _scoreManager = _game.scoreManager;

            var restartButton = new Button(buttonTexture, _buttonFont)
            {
                Position = new Vector2(500, 300),
                Text = "Restart",
            };

            restartButton.Click += restartButton_Click;

            _components = new List<Component>()
      {
        restartButton,
      };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(introBackground, Vector2.Zero, Color.White);
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(_buttonFont, "Score:\n" + _game._score.ToString(), new Vector2(150,50), Color.White);

            spriteBatch.End();
        }
        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
            _game.LoadNextLevel();
        }
    }
}
