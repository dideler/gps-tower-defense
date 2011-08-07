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
    public abstract class Screen
    {

        /// <summary>
        /// A reference to the game containing this screen.
        /// </summary>
        public Game1 theGame { get; set; }

        public Screen(Game1 game)
        {
            this.theGame = game;
        }

        public virtual void Draw(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Load(GraphicsDevice gDev)
        {
        }

        public virtual void Unload()
        {
        }
    }
}
