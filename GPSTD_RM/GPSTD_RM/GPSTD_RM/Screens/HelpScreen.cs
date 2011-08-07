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
    public class HelpScreen : Screen
    {

        public Texture2D Help;

        public HelpScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            Help = Helper.LoadTextureStream("Content\\Help.png", gDev);
        }

        public override void Unload()
        {
            Help = null;
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.screens.Main.animateLogo();
            theGame.spriteBatch.Draw(Help, new Rectangle(0, 0, Help.Width, Help.Height), Color.White);
            theGame.spriteBatch.DrawString(theGame.font, "It's tower defense on Bing Maps!.", new Vector2(50, 50), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Play on a preset map location or use your own location.", new Vector2(50, 100), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Drag a tower icon onto the map to place it.", new Vector2(50, 150), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "To not place a tower, drag it into the recycle bin.", new Vector2(50, 200), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Have fun!", new Vector2(50, 300), Color.Black);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle Home_hitbox = new Rectangle(334, 396, 148, 32);
            if (TouchPanel.IsGestureAvailable && TouchPanel.ReadGesture().GestureType == GestureType.Tap)
            {
                if (theGame.finger.Intersects(Home_hitbox))
                {
                    theGame.sfx.buttonSound.Play();
                    theGame.screen.Unload();
                    theGame.screen = theGame.screens.Main;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
            }

            base.Update(gameTime);
        }
    }
}
