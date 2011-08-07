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
    public class MainScreen : Screen
    {
        public Texture2D Start_btn;
        public Texture2D Achieve_btn;
        public Texture2D High_btn;
        public Texture2D About_btn;
        public Texture2D Help_btn;

        public Texture2D main;
        public Texture2D main_logo;
        public Texture2D main_logo_left;
        public Texture2D main_logo_right;

        public MainScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            Start_btn = Helper.LoadTextureStream("Content\\button1.png", gDev);
            Achieve_btn = Helper.LoadTextureStream("Content\\button3.png", gDev);
            High_btn = Helper.LoadTextureStream("Content\\button2.png", gDev);
            Help_btn = Helper.LoadTextureStream("Content\\button4.png", gDev);
            About_btn = Helper.LoadTextureStream("Content\\button5.png", gDev);

            main = Helper.LoadTextureStream("Content\\Main.png", gDev);

            main_logo = Helper.LoadTextureStream("Content\\logo_main.png", gDev);
            main_logo_left = Helper.LoadTextureStream("Content\\logo_main_left.png", gDev);
            main_logo_right = Helper.LoadTextureStream("Content\\logo_main_right.png", gDev);
        }

        public override void Unload()
        {
            Start_btn = null;
            Achieve_btn = null;
            High_btn = null;
            Help_btn = null;
            About_btn = null;
            main_logo = null;
            main_logo_left = null;
            main_logo_right = null;
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            animateLogo();
            theGame.spriteBatch.Draw(Start_btn, new Rectangle(590, 15, Start_btn.Width, Start_btn.Height), Color.White);
            //spriteBatch.Draw(Achieve_btn, new Rectangle(590, 105, Achieve_btn.Width, Achieve_btn.Height), Color.White);
            theGame.spriteBatch.Draw(High_btn, new Rectangle(590, 105, High_btn.Width, High_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Help_btn, new Rectangle(590, 195, Help_btn.Width, Help_btn.Height), Color.White);
            theGame.spriteBatch.Draw(About_btn, new Rectangle(590, 285, About_btn.Width, About_btn.Height), Color.White);
            if (theGame.display_victory == true)
            {
                theGame.spriteBatch.DrawString(theGame.font, "You Win!", new Vector2(400, 240), Color.Black);

            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {

            if (theGame.sfx.menuSoundInstance != null &&
                        (theGame.sfx.menuSoundInstance.State == SoundState.Stopped || theGame.sfx.menuSoundInstance.State == SoundState.Paused))
            {
                theGame.sfx.menuSoundInstance.Play();
            }
            Rectangle Start_hitbox = new Rectangle(590, 15, Start_btn.Width, Start_btn.Height);
            //Rectangle Achieve_hitbox = new Rectangle(590, 105, Achieve_btn.Width, Achieve_btn.Height);
            Rectangle High_hitbox = new Rectangle(590, 105, High_btn.Width, High_btn.Height);
            Rectangle Help_hitbox = new Rectangle(590, 195, Help_btn.Width, Help_btn.Height);
            Rectangle About_hitbox = new Rectangle(590, 285, About_btn.Width, About_btn.Height);
            if (TouchPanel.IsGestureAvailable && TouchPanel.ReadGesture().GestureType == GestureType.Tap)
            {
                // The button for the start screen was pressed
                if (theGame.finger.Intersects(Start_hitbox))
                {
                    theGame.sfx.buttonSound.Play();
                    theGame.screen.Update(gameTime);
                    theGame.screen = theGame.screens.Start;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
                // The button for the high scores screen was pressed
                else if (theGame.finger.Intersects(High_hitbox))
                {
                    theGame.sfx.buttonSound.Play();
                    theGame.screen.Unload();
                    theGame.screen = theGame.screens.Scores;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
                // The button for the help screen was pressed
                else if (theGame.finger.Intersects(Help_hitbox))
                {
                    theGame.sfx.buttonSound.Play();
                    theGame.screen.Unload();
                    theGame.screen = theGame.screens.Help;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
                // The button for the about screen was pressed
                else if (theGame.finger.Intersects(About_hitbox))
                {
                    theGame.sfx.buttonSound.Play();
                    theGame.screen.Unload();
                    theGame.screen = theGame.screens.About;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is a frame counter used for logo animation.
        /// </summary>
        private int x;

        private bool swap;

        /// <summary>
        /// Handle the logo animation!
        /// </summary>
        public void animateLogo()
        {
            //logic for animation
            if (x >= 30)
            {
                swap = true;
            }
            else if (x <= 0)
            {
                swap = false;
            }
            if (swap == false)
            {
                x++;
            }
            else
            {
                x--;
            }
            if (x < 10)
            {
                theGame.spriteBatch.Draw(main_logo_left, new Rectangle(-10, 0, main_logo_left.Width, main_logo_left.Height), Color.White);
            }
            else if (x >= 10 && x < 20)
            {
                theGame.spriteBatch.Draw(main_logo, new Rectangle(-10, 0, main_logo.Width, main_logo.Height), Color.White);
            }
            else if (x >= 20 && x <= 30)
            {
                theGame.spriteBatch.Draw(main_logo_right, new Rectangle(-10, 0, main_logo_right.Width, main_logo_right.Height), Color.White);

            }

            //animation end
        }
    }
}
