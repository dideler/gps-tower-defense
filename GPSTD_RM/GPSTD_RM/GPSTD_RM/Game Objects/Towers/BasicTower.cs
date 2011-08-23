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
    /// This tower fires the basic projectile.
    /// </summary>
    public class BasicTower : Tower
    {

        #region Static Fields

        public static Texture2D defaultTexture;
        // TODO: Replace this with a default sprite..?
        // TODO: Does this even do anything... it appears to have no value.

        /// <summary>
        /// The default cost for placing this tower.
        /// </summary>
        public static readonly int PRICE = 5;

        #endregion

        #region Static Methods

        /// <summary>
        /// Create a basic tower with the default settings.
        /// </summary>
        /// <param name="game">The game containing this object.</param>
        /// <returns>A default basic tower.</returns>
        public static BasicTower FactoryMake(Game game)
        {
            return new BasicTower(game)
            {
                Range = 100,
                ReloadTime = TimeSpan.FromSeconds(0.5),
                AttackPower = 10,
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.BasicTower, "default")
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        private BasicTower(Game g)
            : base(g)
        {
            ObjectTexture = defaultTexture;
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
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// The implementation of this fire method creates a basic projectile that
        /// is heading towards the target with a high velocity.
        /// </summary>
        /// <param name="target">The creep that is being shot.</param>
        public override void Fire(Creep target)
        {
            if (target == null) return;

            base.Fire(target);

            Vector2 vel = target.Position - this.Position;
            vel.Normalize();
            vel *= Projectile.DEFAULT_SPEED;

            BasicProjectile p = new BasicProjectile(Game, this.Position, vel, this.AttackPower)
            {
                //TODO: Replace this with an appropriate sprite field or something.
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Bullet1, DEFAULT_LOOP)
            };

            // Add the projectile to the current leve's collection
            GameState.Singleton.CurrentLevel.Projectiles.AddLast(p);
        }

        #endregion

    }
}
