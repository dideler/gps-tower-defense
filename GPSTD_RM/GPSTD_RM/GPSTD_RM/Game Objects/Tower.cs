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
    /// All objects which are placeable by the player and fight against the
    /// creeps.
    /// </summary>
    public abstract class Tower : GameObject
    {
        #region Fields

        /// <summary>
        /// The current level of this tower.
        /// </summary>
        public virtual int UpgradeLevel { get; set; }
        
        /// <summary>
        /// The amount of damage done per attack.
        /// </summary>
        public virtual int AttackPower { get; set; }

        /// <summary>
        /// A set of costs for each upgrade.  The first
        /// element represents level 1-2, the second level 2-3, etc.
        /// </summary>
        public List<int> moneyToUpgrade { get; set; }
        
        /// <summary>
        /// The highest level to which it can upgrade.
        /// </summary>
        public int maxLevels
        {
            get
            {
                return moneyToUpgrade.Count;
            }
        }

        /// <summary>
        /// Attack range.  When a creep enters this range, the tower
        /// is able to attack it.
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        /// Used locally to keep track of reloading.
        /// </summary>
        protected TimeSpan timeSinceLastReload;

        /// <summary>
        /// The amount of time that must pass after firing
        /// before the tower can fire again.
        /// </summary>
        public TimeSpan ReloadTime { get; set; }

        #endregion

        /// <summary>
        /// Initialize the tower with a reference to a game.
        /// </summary>
        /// <param name="game">The game containing this object.</param>
        public Tower(Game game)
            : base(game)
        {
            timeSinceLastReload = TimeSpan.Zero;
        }

        #region Updating

        public override void Update(GameTime time)
        {
            base.Update(time);

            if (Alive == false) return;

            timeSinceLastReload += time.ElapsedGameTime;

            Creep target = LocateTarget();

            // Locate a target and fire!!!
            if (timeSinceLastReload >= ReloadTime)
            {
                if (target != null)
                {
                    Fire(target);
                    timeSinceLastReload = TimeSpan.Zero;
                }

            }
        }

        /// <summary>
        /// Executes a "firing" function at the target creep.
        /// </summary>
        public virtual void Fire(Creep target)
        {
        }

        /// <summary>
        /// A private method that most towers implement to find the creep closest to them.
        /// </summary>
        protected virtual Creep LocateTarget()
        {

            Creep minCreep = null;
            float minDist = float.PositiveInfinity;

            foreach (Creep c in GameState.Singleton.CurrentLevel.Creeps)
            {
                float d = Vector2.DistanceSquared(Position, c.Position);
                if (d < Range * Range)
                {

                    if (d < minDist)
                    {
                        minDist = d;
                        minCreep = c;
                    }
                }
            }

            if (minCreep == null) return minCreep;

            // Change the orientation
            Rotation = Helper.GetAngle(Vector2.UnitX, minCreep.Position - Position);

            return minCreep;
        }

        #endregion
    }
}
