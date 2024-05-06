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
    public class HighScoreState : State
    {
        private List<Component> _components;
        Texture2D introBackground;
        SpriteFont _buttonFont;
        ScoreManager _scoreManager;

        public HighScoreState(PlatformerGame game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            var buttonTexture = content.Load<Texture2D>("Controls/Button");
            _buttonFont = content.Load<SpriteFont>("Fonts/gameFont");
            introBackground = content.Load<Texture2D>("Overlays/background_menu");

            _scoreManager = _game.scoreManager;

            var backButton = new Button(buttonTexture, _buttonFont)
            {
                Position = new Vector2(500, 300),
                Text = "Back",
            };

            backButton.Click += backButton_Click;

            _components = new List<Component>()
    {
    backButton,
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
            spriteBatch.DrawString(_buttonFont, "Highscores:\n" + string.Join("\n", _scoreManager.Highscores.Select(c => c.PlayerName + ": " + c.Value).ToArray()), new Vector2(150, 50), Color.White);

            spriteBatch.End();
        }
        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }
    }
}
