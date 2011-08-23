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
    /// This tower creates an AOEProjectile when it fires.
    /// </summary>
    public class AOETower : Tower
    {

        #region Static Fields

        /// <summary>
        /// Use this texture if none is specified
        /// </summary>
        public static Texture2D defaultTexture;
        //TODO: Add default sprite field?


        public static readonly int PRICE = 20;

        #endregion

        /// <summary>
        /// Default stats for the current tower.
        /// </summary>
        /// <param name="game">The game that contains this object.</param>
        /// <returns>AOETower with the default stats.</returns>
        public static AOETower FactoryMake(Game game)
        {
            return new AOETower(game)
            {
                Range = 50,
                Radius = 50,
                ReloadTime = TimeSpan.FromSeconds(2.0),
                AttackPower = 10,
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.AOETower, "default")
            };
        }

        #region Fields

        private float rad, rad2;
        /// <summary>
        /// Get or set the radius (automatically sets the
        /// square radius).
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
        /// Timw when the projectile was spawned.
        /// </summary>
        private TimeSpan startTime;

        /// <summary>
        /// Amount of time the projectile is alive.
        /// </summary>
        public TimeSpan LifeSpan { get; set; }

        /// <summary>
        /// Has this tower hit a target?
        /// </summary>
        public bool HitTargets { get; set; }

        #endregion

        /// <summary>
        /// The basic constructor
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        private AOETower(Game g)
            : base(g)
        {
            ObjectTexture = defaultTexture;
            HitTargets = false;
        }

        /// <summary>
        /// This implementation of fire creates an AOEProjectile and
        /// destroys the tower.
        /// </summary>
        /// <param name="target">The creep that activated the explosion.</param>
        public override void Fire(Creep target)
        {
            base.Fire(target);

            AOEProjectile p = new AOEProjectile(Game, Position, Vector2.Zero, AttackPower);

            p.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Ex, DEFAULT_LOOP);
            p.Radius = this.Radius;

            GameState.Singleton.CurrentLevel.Projectiles.AddLast(p);
            
        }
    }
}
