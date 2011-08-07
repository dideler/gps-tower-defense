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
    public class LaserProjectile : Projectile
    {

        #region Constants

        public static readonly TimeSpan DEFAULT_LIFESPAN = TimeSpan.FromSeconds(0.25);

        #endregion


        #region Fields

        /// <summary>
        /// Timw when the projectile was spawned.
        /// </summary>
        private TimeSpan startTime;

        /// <summary>
        /// Amount of time the projectile is alive.
        /// </summary>
        public TimeSpan LifeSpan { get; set; }


        /// <summary>
        /// Have targets been hit?
        /// </summary>
        public bool HitTargets { get; set; }

        #endregion

        #region Constructors

        //TODO: Should these constructors even be used.
        /*
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        public LaserProjectile(Game g)
            : base(g)
        {
            HitTargets = false;
            LifeSpan = DEFAULT_LIFESPAN;
        }
        */

        //TODO: I don't think this should be public.=
        private LaserProjectile(Game g, Vector2 position, Vector2 velocity, int attackPower)
            : base(g, position, velocity, attackPower)
        {
            HitTargets = false;
            LifeSpan = DEFAULT_LIFESPAN;
        }

        /// <summary>
        /// Construct a laser projectile with a direction.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="velocity">This really has no effect on this type of projectile.</param>
        /// <param name="direction">Which way is the laser going?</param>
        /// <param name="attackPower">How much damage will it do?</param>
        public LaserProjectile(Game g, Vector2 position, Vector2 velocity, Vector2 direction, int attackPower)
            : this(g, position, velocity, attackPower)
        {
            HitTargets = false;
            direction.Normalize();
            EndOfLine = direction * LENGTH + position;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Basic update logic.
        /// </summary>
        /// <param name="time">The current game time.</param>
        public override void Update(GameTime time)
        {
            base.Update(time);

            if (Alive == false) return;

            // If it hasn't hit targets yet, hit them,
            // but only once.
            if (!HitTargets)
            {
                startTime = time.TotalGameTime;

                foreach (Creep c in GameState.Singleton.CurrentLevel.Creeps)
                {
                    if (Helper.pointLineDistanceSquared(c.Position, this.Position, this.EndOfLine) < c.Radius2)
                    {
                        c.ReceiveDamage(AttackPower);
                    }
                }

                Rotation = Helper.GetAngle(Vector2.UnitX, EndOfLine - Position);

                HitTargets = true;
            }

            // Kill it off if it's lifespan is up.
            if (time.TotalGameTime - startTime >= LifeSpan)
            {
                Alive = false;
            }
        }

        #endregion

        #region Drawing and Graphics

        /// <summary>
        /// The drawing of the laser projectile should be different because
        /// it's not a sprite
        /// </summary>
        /// <param name="time">The current game time.</param>
        public override void Draw(GameTime time)
        {
            //base.Draw(time);
            // TODO: Implement a good draw method
            // TODO: Sprite drawing...?  Also, is this override even necessary?
            ((Game1)Game).spriteBatch.Draw(ObjectTexture, Position, null, Color.White, Rotation, new Vector2(0, 10), 1.0f, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
