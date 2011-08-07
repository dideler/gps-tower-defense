using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace GPSTD_RM
{
    public class BasicProjectile : Projectile
    {

        #region Fields
        
        /// <summary>
        /// TODO
        /// </summary>
        protected bool gotTargets;

        protected Vector2 initialPosition;
        protected Vector2 lastPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        public BasicProjectile(Game g)
            : base(g)
        {
            gotTargets = false;
        }

        /// <summary>
        /// Full constructor, inherited from the projectile constructor.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="velocity">Initial velocity.</param>
        /// <param name="attackPower">How much damage will it do?</param>
        public BasicProjectile(Game g, Vector2 position, Vector2 velocity, int attackPower)
            : base(g, position, velocity, attackPower)
        {
            gotTargets = false;
            velocity.Normalize();
            EndOfLine = Position + velocity * Projectile.LENGTH;
            lastPosition = position;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Checks for collisions with any creep, deals damage and destroys itself.
        /// </summary>
        /// <param name="time">Current game time.</param>
        public override void Update(GameTime time)
        {
            base.Update(time);

            if (Alive == false) return;

            // For each creep, draw a line between the last position and the current position,
            // and check the creep's proximity to that line.  If it's close enough, it counts as
            // a collision.
            //TODO: Creep collection should be stored somewhere else
            foreach (Creep c in GameState.Singleton.CurrentLevel.Creeps)
            {
                if (Helper.pointLineDistanceSquared(c.Position, Position, lastPosition) < c.Radius2)
                {
                    CollisionEffect(c);
                    break;
                }
            }

            lastPosition = Position;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// What happens when this projectile collides with a creep.
        /// </summary>
        /// <param name="c">The creep on which to apply the collision effect.</param>
        protected virtual void CollisionEffect(Creep c)
        {
            c.ReceiveDamage(AttackPower);
            Alive = false;
        }

        #endregion
    }
}
