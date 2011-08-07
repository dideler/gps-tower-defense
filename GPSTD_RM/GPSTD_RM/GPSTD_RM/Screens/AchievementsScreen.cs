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
    public class AchievementsScreen : Screen
    {
        public AchievementsScreen(Game1 game)
            : base(game)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.screens.Main.animateLogo();

            base.Draw(gameTime);
        } 

    }
}
