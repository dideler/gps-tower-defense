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
    public class BlackScreen : Screen
    {
        public const int TIME_SPAN_MILLIS = 500;

        public Texture2D black;

        public BlackScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            black = Helper.LoadTextureStream("Content\\black.png", gDev);
        }

        public override void Unload()
        {
            black = null;
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(black, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);

            base.Draw(gameTime);
        }

        public override void Update(GameTime time)
        {

            if (time.TotalGameTime.Milliseconds > TIME_SPAN_MILLIS)
            {
                theGame.screen = theGame.screens.Splash;
                theGame.screen.Load(theGame.GraphicsDevice);
            }

            base.Update(time);
        }
    }
}
