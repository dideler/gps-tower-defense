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
    public class AboutScreen : Screen
    {

        public Texture2D WhiteBoard;
        public Texture2D small_logo;

        public AboutScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            WhiteBoard = Helper.LoadTextureStream("Content\\WhiteBoard.png", gDev);
            small_logo = Helper.LoadTextureStream("Content\\small_logo.png", gDev);
        }

        public override void Unload()
        {
            WhiteBoard = null;
            small_logo = null;
        }

        public override void Draw(GameTime gameTime)
        {

            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.screens.Main.animateLogo();
            theGame.spriteBatch.Draw(WhiteBoard, new Rectangle(0, 0, WhiteBoard.Width, WhiteBoard.Height), Color.White);
            theGame.spriteBatch.Draw(small_logo, new Rectangle(WhiteBoard.Width / 3, WhiteBoard.Height / 4, small_logo.Width, small_logo.Height), Color.White);
            theGame.spriteBatch.DrawString(theGame.font, "GPS Tower Defense was developed by four Brock University students ", new Vector2(45, 40), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "at the 48-hour Great Canadian Appathon competition which took place", new Vector2(45, 70), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Graham Sharp", new Vector2(80, 160), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Kaylen Wheeler", new Vector2(80, 220), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Justin Masse", new Vector2(80, 280), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, "Dennis Ideler", new Vector2(80, 340), Color.Black);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle Back_hitbox = new Rectangle(584, 396, 148, 32);
            if (TouchPanel.IsGestureAvailable && TouchPanel.ReadGesture().GestureType == GestureType.Tap)
            {
                if (theGame.finger.Intersects(Back_hitbox))
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
