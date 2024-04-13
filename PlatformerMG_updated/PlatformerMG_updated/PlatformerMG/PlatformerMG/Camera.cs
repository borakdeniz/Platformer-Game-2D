
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PlatformerMG
{
    internal class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Player player, GraphicsDeviceManager graphics)
        {
            var position = Matrix.CreateTranslation(
    -player.Position.X - (player.localBounds.Width/4), 
    -player.Position.Y - (player.localBounds.Height/4) + 70, 
    0);
            var offset = Matrix.CreateTranslation(
                graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2,
                0);
            Transform = position * offset;

        }
    }
  
}
