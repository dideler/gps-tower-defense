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
    public class ScoresScreen : Screen
    {

        public Texture2D Highscores;

        public ScoresScreen(Game1 game)
            : base(game)
        {
        }

        public override void Load(GraphicsDevice gDev)
        {
            Highscores = Helper.LoadTextureStream("Content\\highscores.png", gDev);
        }

        public override void Unload()
        {
            Highscores = null;
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.spriteBatch.Draw(theGame.screens.Main.main, new Rectangle(0, 0, theGame.GraphicsDevice.Viewport.Width, theGame.GraphicsDevice.Viewport.Height), Color.White);
            theGame.screens.Main.animateLogo();
            theGame.spriteBatch.Draw(Highscores, new Rectangle(0, 0, Highscores.Width, Highscores.Height), Color.White);
            {
                List<int> highscores = theGame.LoadScores();

                int rank = 1;
                foreach (int highscore in highscores)
                {
                    int location = 94 + (rank * 27);
                    theGame.spriteBatch.DrawString(theGame.font, rank.ToString(), new Vector2(80, location), Color.Black);
                    theGame.spriteBatch.DrawString(theGame.font, highscore.ToString(), new Vector2(160, location), Color.Black);
                    rank++;
                }
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            Rectangle Newgame_hitbox = new Rectangle(590, 15, theGame.screens.Start.Newgame_btn.Width, theGame.screens.Start.Newgame_btn.Height);
            Rectangle Resumegame_hitbox = new Rectangle(590, 105, theGame.screens.Start.Resumegame_btn.Width, theGame.screens.Start.Resumegame_btn.Height);
            Rectangle Back_hitbox = new Rectangle(590, 195, theGame.screens.Start.Back_btn.Width, theGame.screens.Start.Back_btn.Height);

            Back_hitbox = new Rectangle(584, 396, 148, 32);
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
