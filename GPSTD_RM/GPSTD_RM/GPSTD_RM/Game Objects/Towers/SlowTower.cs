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
    public class SlowTower : Tower
    {

        #region Static Fields
        
        // TODO: Replace with a sprite or something
        public static Texture2D defaultTexture;

        /// <summary>
        /// Default cost for a slow tower.
        /// </summary>
        public static readonly int PRICE = 15;

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a slow tower with the default stats.
        /// </summary>
        /// <param name="game">The game containing this object.</param>
        /// <returns>A default slow tower.</returns>
        public static SlowTower FactoryMake(Game game)
        {
            return new SlowTower(game)
            {
                Range = 100,
                ReloadTime = TimeSpan.FromSeconds(0.5),
                AttackPower = 10,
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.SlowTower, "default")
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        private SlowTower(Game g)
            : base(g)
        {
            ObjectTexture = defaultTexture;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Basic update logic.  Not much different that the base.
        /// </summary>
        /// <param name="time">The current game time.</param>
        public override void Update(GameTime time)
        {
            base.Update(time);

            if (Alive == false) return;
        }

        #endregion

        #region Other methods

        /// <summary>
        /// The implementation of this fire method creates a basic projectile that
        /// is heading towards the target with a high velocity.
        /// </summary>
        /// <param name="target"></param>
        public override void Fire(Creep target)
        {
            if (target == null) return;

            base.Fire(target);

            Vector2 vel = target.Position - this.Position;
            vel.Normalize();
            
            vel *= SlowProjectile.DEFAULT_SPEED;
            

            SlowProjectile p = new SlowProjectile(Game, this.Position, vel, this.AttackPower);

            p.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Bullet3, DEFAULT_LOOP);

            // Add the projectile to a the current level's projectile list.
            GameState.Singleton.CurrentLevel.Projectiles.AddLast(p);

        }

        #endregion
    }
}
