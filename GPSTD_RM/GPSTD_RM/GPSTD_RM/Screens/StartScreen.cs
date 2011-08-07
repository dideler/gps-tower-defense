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
    public class StartScreen : Screen
    {

        public Texture2D Back_btn;
        public Texture2D Resumegame_btn;
        public Texture2D Newgame_btn;

        public StartScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            Back_btn = Helper.LoadTextureStream("Content\\back.png", gDev);
            Resumegame_btn = Helper.LoadTextureStream("Content\\resumegame.png", gDev);
            Newgame_btn = Helper.LoadTextureStream("Content\\newgame.png", gDev);
        }

        public override void Unload()
        {
            Back_btn = null;
            Resumegame_btn = null;
            Newgame_btn = null;
        }

        public override void Draw(GameTime gameTime)
        {

            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.spriteBatch.Draw(Newgame_btn, new Rectangle(590, 15, Newgame_btn.Width, Newgame_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Resumegame_btn, new Rectangle(590, 105, Resumegame_btn.Width, Resumegame_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Back_btn, new Rectangle(590, 190, Back_btn.Width, Back_btn.Height), Color.White);
            theGame.screens.Main.animateLogo();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle Newgame_hitbox = new Rectangle(590, 15, Newgame_btn.Width, Newgame_btn.Height);
            Rectangle Resumegame_hitbox = new Rectangle(590, 105, Resumegame_btn.Width, Resumegame_btn.Height);
            Rectangle Back_hitbox = new Rectangle(590, 195, Back_btn.Width, Back_btn.Height);
            if (TouchPanel.IsGestureAvailable && TouchPanel.ReadGesture().GestureType == GestureType.Tap)
            {
                if (theGame.finger.Intersects(Newgame_hitbox))
                {
                    theGame.sfx.buttonSound.Play();

                    theGame.screen.Unload();
                    theGame.screen = theGame.screens.NewGame;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
                else if (theGame.finger.Intersects(Resumegame_hitbox))
                {
                    theGame.sfx.buttonSound.Play();

                    if (theGame.sfx.menuSoundInstance != null) theGame.sfx.menuSoundInstance.Stop();
                    theGame.screen = theGame.screens.Playing;
                    theGame.screen.Load(theGame.GraphicsDevice);
                }
                else if (theGame.finger.Intersects(Back_hitbox))
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
