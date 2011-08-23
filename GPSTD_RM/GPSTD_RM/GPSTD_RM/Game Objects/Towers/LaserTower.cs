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
using System.Device.Location;

namespace GPSTD_RM
{
    /// <summary>
    /// This tower fires lazers, which hit all enemies in a straight line.
    /// </summary>
    public class LaserTower : Tower
    {

        #region Static Fields
        
        //TODO: Replace with sprite?
        public static Texture2D defaultTexture;

        /// <summary>
        /// Default cost for a laser tower.
        /// </summary>
        public static readonly int PRICE = 50;

        #endregion

        #region Static Methods

        /// <summary>
        /// Creates a laser tower with all the default properties.
        /// </summary>
        /// <param name="game">The game that contains this object.</param>
        /// <returns>A default laser tower.</returns>
        public static LaserTower FactoryMake(Game game)
        {
            return new LaserTower(game)
            {
                Range = 100,
                ReloadTime = TimeSpan.FromSeconds(0.5),
                AttackPower = 10,
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.LaserTower, "default")
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="g">The game that contains this object.</param>
        private LaserTower(Game g)
            : base(g)
        {
            ObjectTexture = defaultTexture;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// This implementation of fire creates an AOEProjectile and
        /// destroys the tower.
        /// </summary>
        /// <param name="target">The creep that set off the laser.</param>
        public override void Fire(Creep target)
        {
            base.Fire(target);

            LaserProjectile p = new LaserProjectile(Game, Position, Vector2.Zero, target.Position - Position, AttackPower);

            p.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Laser, DEFAULT_LOOP);

            GameState.Singleton.CurrentLevel.Projectiles.AddLast(p);
        }

        #endregion

    }
}
