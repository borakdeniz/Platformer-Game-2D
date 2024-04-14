using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace PlatformerMG
{
    class CollisionManager
    {
        private List<Collidable> m_Collidables = new List<Collidable>();
        private HashSet<Collision> m_Collisions = new HashSet<Collision>(new CollisionComparer());
        public void AddCollidable(Collidable c)
        {
            m_Collidables.Add(c);
        }
        public void AddCollidable(Rectangle rect, TileCollision tileCollision)
        {
            Collidable temp = new Collidable();
            temp.boundingBox = rect;
            temp.TileCollision = tileCollision;
            m_Collidables.Add(temp);
        }
        public void Update(Player player)
        {
            UpdateCollisions(player.BoundingRectangle);
            ResolveCollisions(player);
        }
        private void UpdateCollisions(Rectangle rect)
        {
            if (m_Collisions.Count > 0)
            {
                m_Collisions.Clear();
            }
            // Iterate through collidable objects and test for collisions between each one
            for (int i = 0; i < m_Collidables.Count; i++)
            {
                    Collidable collidable1 = m_Collidables[i];
                    Collidable playerCollidable = new Collidable();
                playerCollidable.boundingBox = rect;

                // Make sure we're not checking an object with itself
                if (!collidable1.Equals(playerCollidable))
                    {
                        // If the two objects are colliding then add them to the set
                        if (collidable1.CollisionTest(playerCollidable))
                        {
                            m_Collisions.Add(new Collision(collidable1, playerCollidable));
                        }
                    }
            }
        }
        private void ResolveCollisions(Player player)
        {
            foreach (var collision in m_Collisions)
            {
                collision.A.OnCollision(collision.A, player);
            }
        }
    }
}

