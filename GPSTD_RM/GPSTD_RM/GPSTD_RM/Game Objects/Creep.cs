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
    /// This is the superclass of all enemies in the game.
    /// </summary>
    public class Creep : GameObject
    {

        #region Constants

        /// <summary>
        /// The amount of time it takes for a slow counter to wear off
        /// </summary>
        public static readonly TimeSpan SLOW_DECAY = TimeSpan.FromSeconds(1.0);


        

        #endregion

        #region Spawning Info

        /// <summary>
        /// Important.  Determines which wave they belong to.
        /// Perhaps this can be used in the get methods of the stats.
        /// An equation can be used for each enemy type to "level them up."
        /// </summary>
        public int Wavenum { get; set; }

        /// <summary>
        /// Each creep has an associated spawn point, determining the
        /// path that it follows.
        /// </summary>
        public SpawnPoint SpawnPoint { get; set; }

        /// <summary>
        /// The current link in the waypoint list.  Represents the next
        /// Vector2 that will be reached if the object continues along in the
        /// same direction it is going.
        /// </summary>
        public LinkedListNode<Link> CurrentLink { get; set; }

        /// <summary>
        /// Bounding rectangle defined by the current link.
        /// </summary>
        public BoundingBox currentLinkBox;

        #endregion

        #region Stats

        /// <summary>
        /// Determines how much damage it can take before dying.
        /// </summary>
        public int hp { get; set; }

        /// <summary>
        /// Determines how much damage is subtracted from each hit it takes.
        /// </summary>
        public int defence { get; set; }

        /// <summary>
        /// Determines how many points are received by the player for killing it.
        /// </summary>
        public int pointValue { get; set; }

        /// <summary>
        /// Determines how much money the player receives for killing it.
        /// </summary>
        public double moneyValue { get; set; }

        /// <summary>
        /// The slow tower puts slow counters on all the enemies it hits.
        /// The number of slow counters determines how much each enemy is slowed down.
        /// </summary>
        public int slowCounters { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        private TimeSpan lastDecayTime;

        /// <summary>
        /// Radius of this creep squared.
        /// </summary>
        public float Radius2
        {
            get
            {
                float ret = (this.Bounds.Max.X - this.Bounds.Min.X) / 2f;
                return ret * ret;
            }
        }

        #endregion

        /// <summary>
        /// Fairly basic initialization.
        /// </summary>
        /// <param name="game">The game containing this object.</param>
        public Creep(Game game)
            : base(game)
        {
            Alive = true;
            lastDecayTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Called by projectiles to attempt to damage the enemy.  The enemy provides its
        /// own implementation of how to get damaged.  This default should serve most
        /// purposes.
        /// </summary>
        /// <param name="damageValue">Damage value.</param>
        public virtual void ReceiveDamage(int damageValue)
        {
            // Default implementation of creeps getting damaged
            int val = damageValue - defence;
            if (val < 0) val = 0;

            hp -= val;

            if (hp < 0) hp = 0;

            if (hp == 0)
            {
                Alive = false;
                GameState.Singleton.Money += moneyValue;
                GameState.Singleton.Score += pointValue;
            }
        }

        /// <summary>
        /// Basic update logic for all creeps.
        /// </summary>
        /// <param name="time">Current game time.</param>
        public override void Update(GameTime time)
        {
            base.Update(time);

            if (!Alive)
                return;

            //TODO: Implement the slowness counters!!!

            if (CurrentLink != null)
            {
                Vector2 v = CurrentLink.Value.last - (Position - new Vector2(400, 240));

                while (v.LengthSquared() < Speed * Speed)
                {
                    CurrentLink = CurrentLink.Next;
                    if (CurrentLink == null) return;
                    v = CurrentLink.Value.last - CurrentLink.Value.first;
                }
                v.Normalize();
                Position += v * Speed;
                Rotation = Helper.GetAngle(Vector2.UnitX, v);
            }
            if (CurrentLink == null)
            {
                // DEAL DAMAGE
                GameState.Singleton.Lives--;

                Alive = false;
            }

        }

        #region PrivateMethods

        /// <summary>
        /// Has the creep passed a point along the path?
        /// </summary>
        private bool PassedPoint()
        {
            return currentLinkBox.Contains(new Vector3(Position, 0)) == ContainmentType.Contains;
        }

        /// <summary>
        /// Updates the bounding rectangle defined by the current link.
        /// </summary>
        private void UpdateBox()
        {
            float minX = Math.Min(CurrentLink.Value.first.X, CurrentLink.Value.last.X);
            float minY = Math.Min(CurrentLink.Value.first.Y, CurrentLink.Value.last.Y);
            float maxX = Math.Max(CurrentLink.Value.first.X, CurrentLink.Value.last.X);
            float maxY = Math.Max(CurrentLink.Value.first.Y, CurrentLink.Value.last.Y);

            currentLinkBox = new BoundingBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 0));
        }

        #endregion
    }
}
