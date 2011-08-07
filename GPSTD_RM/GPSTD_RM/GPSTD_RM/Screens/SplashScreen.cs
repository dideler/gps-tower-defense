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
    public class SplashScreen : Screen
    {

        public Texture2D logo;

        public SplashScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            logo = Helper.LoadTextureStream("Content\\brocket.png", gDev);
        }

        public override void Unload()
        {
            logo = null;
        }

        public override void Draw(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds < 2)
            {
                theGame.time += 10;
            }
            else if (gameTime.TotalGameTime.TotalSeconds > 2)
            {
                theGame.time -= 15;
            }
            Color c = new Color(theGame.time, theGame.time, theGame.time, 100);
            theGame.spriteBatch.Draw(logo, new Rectangle(0, 0, 800, 480), c);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > 3)
            {
                theGame.screen.Unload();
                theGame.screen = theGame.screens.Main;
                theGame.screen.Load(theGame.GraphicsDevice);
                // Music ends when game started.
                theGame.sfx.menuSound = theGame.Content.Load<SoundEffect>("menu");
                theGame.sfx.menuSoundInstance = theGame.sfx.menuSound.CreateInstance(); // So we have more control over the sound.
                theGame.sfx.menuSoundInstance.IsLooped = true;
                theGame.sfx.menuSoundInstance.Play();
            }

            base.Update(gameTime);
        }
    }
}
