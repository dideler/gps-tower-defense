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
    /// This tower is similar to the AOETower, but destroys itself when it fires.
    /// </summary>
    public class BombTower : Tower
    {

        #region Static Fields

        public static Texture2D defaultTexture;

        // TODO: Tweak this!!
        public static readonly float BLAST_RADIUS = 20.0f;
        public static readonly int PRICE = 1;

        #endregion

        #region Static Methods
        public static BombTower FactoryMake(Game game)
        {
            return new BombTower(game)
            {
                Range = 50,
                ReloadTime = TimeSpan.FromSeconds(0.5),
                AttackPower = 10,
                ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.BombTower, "default")
            };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="g">The game containing this object.</param>
        private BombTower(Game g)
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
        /// <param name="target">The creep that caused the explosion.</param>
        public override void Fire(Creep target)
        {
            base.Fire(target);

            AOEProjectile p = new AOEProjectile(Game, Position, Vector2.Zero, AttackPower);

            p.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Ex, DEFAULT_LOOP);

            GameState.Singleton.CurrentLevel.Projectiles.AddLast(p);
            p.Radius = this.Range;

            Alive = false;
        }

        #endregion
    }
}
