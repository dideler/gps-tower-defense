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
    /// Superclass of all physical objects in the game.
    /// </summary>
    public class GameObject : DrawableGameComponent
    {

        public const string DEFAULT_LOOP = "default";

        #region Graphics Fields
        /// <summary>
        /// Reference to a batch in which to draw sprites.
        /// </summary>
        public SpriteBatch Batch { get; set; }

        /// <summary>
        /// Current texture representing the object.  This property
        /// may be specified.  If it is not, the animated sprite will
        /// be used.
        /// 
        /// Note : This field should always be used while drawing.
        /// </summary>
        private Texture2D objTex;
        public Texture2D ObjectTexture
        {
            get
            {
                if (objTex == null)
                {
                    return ObjectSprite.CurrentTexture();
                }
                return objTex;
            }

            set
            {
                objTex = value;
            }
        }

        /// <summary>
        /// Sprite representing the object's animations.
        /// </summary>
        public AnimatedSpriteInstance ObjectSprite { get; set; }

        #endregion

        #region Physical Fields
        /// <summary>
        /// Object's position in world space.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Object's rotation in radians.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Object's velocity vector in world units per second.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Axis-aligned bounding box.
        /// </summary>
        public virtual BoundingBox Bounds
        {
            get
            {
                //TODO: Change this from pixel space? (Depends on how it's used!)
                return new BoundingBox(new Vector3(this.Position - new Vector2(ObjectTexture.Width / 2f, ObjectTexture.Height / 2f), 0),
                    new Vector3(this.Position + new Vector2(ObjectTexture.Width / 2f, ObjectTexture.Height / 2f), 0));
            }
            set { }
        }

        #endregion

        #region Object State Fields

        public bool Alive { get; set; }

        #endregion

        #region Initialization

        public GameObject(Game game)
            : base(game)
        {
            this.Batch = ((Game1)game).spriteBatch;
            Alive = true;
        }

        #endregion

        #region Updating

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Handle sprite animations
            this.ObjectSprite.Update(gameTime);
        }

        #endregion

        #region Collision

        /// <summary>
        /// Determines whether this object collides with another object.  Note that, in some
        /// cases, this should be overridden.
        /// </summary>
        /// <param name="o">The object that it may be colliding with.</param>
        /// <returns>True if collision happened.</returns>
        public virtual bool CollidesWith(GameObject o)
        {
            return Bounds.Intersects(o.Bounds);
        }

        #endregion

        #region Drawing

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 min = GameState.Singleton.LevelToPixel(new Vector2(Bounds.Min.X, Bounds.Min.Y));
            Vector2 max = GameState.Singleton.LevelToPixel(new Vector2(Bounds.Max.X, Bounds.Max.Y));

            Batch.Draw(ObjectTexture, Position, null, Color.White, Rotation, (new Vector2(ObjectTexture.Width / 2f, ObjectTexture.Height / 2f)),
                1.0f, SpriteEffects.None, 0f);
        }

        #endregion
        
    }
}
