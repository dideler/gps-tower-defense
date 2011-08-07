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
    public class NewGameScreen : Screen
    {

        public Texture2D Tour_btn;
        public Texture2D Customgame_btn;
        public Texture2D Mylocation_btn;
        public Texture2D Back_btn;

        public NewGameScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            Tour_btn = Helper.LoadTextureStream("Content\\tour.png", gDev);
            Customgame_btn = Helper.LoadTextureStream("Content\\customgame.png", gDev);
            Mylocation_btn = Helper.LoadTextureStream("Content\\mylocation.png", gDev);
            Back_btn = Helper.LoadTextureStream("Content\\back.png", gDev);
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.screens.Main.animateLogo();
            theGame.spriteBatch.Draw(Tour_btn, new Rectangle(590, 15, Tour_btn.Width, Tour_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Customgame_btn, new Rectangle(590, 105, Customgame_btn.Width, Customgame_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Mylocation_btn, new Rectangle(590, 195, Mylocation_btn.Width, Mylocation_btn.Height), Color.White);
            theGame.spriteBatch.Draw(Back_btn, new Rectangle(590, 285, Back_btn.Width, Back_btn.Height), Color.White);

            base.Draw(gameTime);
        }

        public override void Unload()
        {
            Tour_btn = null;
            Customgame_btn = null;
            Mylocation_btn = null;
            Back_btn = null;
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle tour_hitbox = new Rectangle(590, 15, Tour_btn.Width, Tour_btn.Height);
            Rectangle customgame_hitbox = new Rectangle(590, 105, Customgame_btn.Width, Customgame_btn.Height);
            Rectangle mylocation_hitbox = new Rectangle(590, 195, Mylocation_btn.Width, Mylocation_btn.Height);
            Rectangle Back_hitbox = new Rectangle(590, 285, Back_btn.Width, Back_btn.Height);

            System.Diagnostics.Debug.WriteLine("NEW GAME SCREEN");
            if (theGame.finger.Intersects(tour_hitbox))
            {
                theGame.sfx.buttonSound.Play();

                theGame.screen.Unload();
                if (theGame.sfx.menuSoundInstance != null) theGame.sfx.menuSoundInstance.Stop();
                theGame.screen = theGame.screens.Playing;
                theGame.screen.Load(theGame.GraphicsDevice);
            }
            else if (theGame.finger.Intersects(customgame_hitbox))
            {
                theGame.sfx.buttonSound.Play();
                theGame.screen.Unload();
                if (theGame.sfx.menuSoundInstance != null) theGame.sfx.menuSoundInstance.Stop();
                // Get the search location
                Guide.BeginShowKeyboardInput(PlayerIndex.One, "Search", "Search for a location", "", theGame.LocationSelected, null);
            }
            else if (theGame.finger.Intersects(mylocation_hitbox))
            {
                theGame.sfx.buttonSound.Play();

                theGame.screen.Unload();
                if (theGame.sfx.menuSoundInstance != null) theGame.sfx.menuSoundInstance.Stop();
                theGame.screen = theGame.screens.Playing;

                // Busy loop waiting for current location
                while (theGame.currentLocation == null)
                {
                    ;
                }

                theGame.startingCoordinate = theGame.currentLocation;

                theGame.screen.Load(theGame.GraphicsDevice);
            }
            else if (theGame.finger.Intersects(Back_hitbox))
            {
                System.Diagnostics.Debug.WriteLine("BACK TO START?");

                theGame.sfx.buttonSound.Play();
                theGame.screen.Unload();
                theGame.screen = theGame.screens.Start;
                theGame.screen.Load(theGame.GraphicsDevice);
            }

            base.Update(gameTime);
        }
    }
}
