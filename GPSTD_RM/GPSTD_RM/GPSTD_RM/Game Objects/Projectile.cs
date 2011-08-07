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
    /// Represents all objects that are shot out of towers.  I think
    /// the name is fairly self-explanatory.
    /// </summary>
    public abstract class Projectile : GameObject
    {
        /// <summary>
        /// How far should the projectile go before it stops?
        /// </summary>
        public static readonly float LENGTH = 10000;

        /// <summary>
        /// Default speed of each moving projectile.
        /// </summary>
        public static readonly float DEFAULT_SPEED = 20;

        public int AttackPower { get; set; }
        public Vector2 Velocity { get; set; }
        public List<Creep> HitList { get; set; }
        protected Vector2 EndOfLine;

        public Projectile(Game game)
            : base(game)
        {
        }

        public Projectile(Game game, Vector2 position, Vector2 velocity, int attackPower)
            : base(game)
        {
            Position = position;
            Velocity = velocity;
            AttackPower = attackPower;

        }

        /// <summary>
        /// Default implemetnation of update for Projectiles.
        /// Updates the position, based on the velocity.
        /// </summary>
        public override void Update(GameTime time)
        {
            base.Update(time);

            if (Alive == false) return;

            HitList = new List<Creep>();

            Vector2 p2 = Position + Velocity;

            Position += Velocity;
        }
    }
}
