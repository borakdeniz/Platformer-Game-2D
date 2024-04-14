using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PlatformerMG
{
    public class Collidable
    {
        TileCollision tileCollision;

        #region Fields
        protected BoundingSphere boundingSphere = new BoundingSphere();
        public Rectangle boundingBox = new Rectangle();
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }
        #endregion
        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }
        public TileCollision TileCollision
        {
            get { return tileCollision; }
            set { tileCollision = value; }
        }

        #region Member Functions

        public virtual bool CollisionTest(Collidable obj)
        {
            if (obj != null)
            {
                return boundingBox.Intersects(obj.BoundingBox);
            }
            return false;
        }

        public virtual bool CollisionTest(Rectangle obj)
        {
            return false;
        }

        public void OnCollision(Collidable obj, Player player)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = player.BoundingRectangle;
            TileCollision collision = obj.tileCollision;
            
            // Reset flag to search for ground collision.
            player.isOnGround = false;

            // Determine collision depth (with direction) and magnitude.
            Rectangle tileBounds = obj.boundingBox;
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.
                if (absDepthY < absDepthX || collision == TileCollision.Platform)
                {
                    // If we crossed the top of a tile, we are on the ground.
                    if (player.previousBottom <= tileBounds.Top)
                        player.isOnGround = true;

                    // Ignore platforms, unless we are on the ground.
                    if (collision == TileCollision.Impassable || player.isOnGround)
                    {
                        // Resolve the collision along the Y axis.
                        player.Position = new Vector2(player.Position.X, player.Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.
                        bounds = player.BoundingRectangle;
                    }
                }
                else if (collision == TileCollision.Impassable && tileBounds.Y < bounds.Y) // Ignore platforms.
                {
                    // Resolve the collision along the X axis.
                    player.Position = new Vector2(player.Position.X + depth.X, player.Position.Y);

                    // Perform further collisions with the new bounds.
                    bounds = player.BoundingRectangle;
                }
            }
        }
        #endregion


    }
}
