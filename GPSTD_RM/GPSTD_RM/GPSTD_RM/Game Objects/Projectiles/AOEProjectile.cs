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
    /// <summary>
    /// Area of effect Projectile.  It doesn't move.  It only expands.
    /// </summary>
    public class AOEProjectile : Projectile
    {
        #region Constants

        public static readonly TimeSpan DEFAULT_LIFESPAN = TimeSpan.FromSeconds(0.25);

        #endregion

        #region Fields

        private float rad, rad2;
        
        /// <summary>
        /// 
        /// </summary>
        public float Radius
        {
            get
            {
                return rad;
            }
            set
            {
                rad = value;
                rad2 = value * value;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private TimeSpan startTime;
        
        /// <summary>
        /// The amount of time the projectile stays alive.
        /// </summary>
        public TimeSpan LifeSpan { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public bool HitTargets { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Basic initialization.
        /// </summary>
        /// <param name="g">The game that contains this object.</param>
        public AOEProjectile(Game g)
            : base(g)
        {
            HitTargets = false;
            //initializeTextures();
            LifeSpan = DEFAULT_LIFESPAN;
            ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Ex, DEFAULT_LOOP);
        }

        /// <summary>
        /// Full constructor, inherited from the projectile constructor.
        /// </summary>
        /// <param name="g">The game that contains this object.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="velocity">Initial velocity.</param>
        /// <param name="attackPower">How much damage will it do?</param>
        public AOEProjectile(Game g, Vector2 position, Vector2 velocity, int attackPower)
            : base(g, position, velocity, attackPower)
        {
            HitTargets = false;
            //initializeTextures();
            LifeSpan = DEFAULT_LIFESPAN;
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

                // TODO: Find an appropriate storage location for the
                // creeps collection
                foreach (Creep c in GameState.Singleton.CurrentLevel.Creeps)
                {
                    if (Vector2.DistanceSquared(c.Position, Position) < rad2)
                    {
                        c.ReceiveDamage(AttackPower);
                    }
                }

                HitTargets = true;
            }

            float totalSeconds = (float)(time.TotalGameTime - startTime).TotalSeconds;

            if (totalSeconds >= LifeSpan.TotalSeconds)
            {
                Alive = false;
            }
        }

        #endregion

    }
}
