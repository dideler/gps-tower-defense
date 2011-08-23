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
using System.Device.Location;

namespace GPSTD_RM
{

    /// <summary>
    /// A singleton class containing all information relevant to the player.
    /// </summary>
    public class GameState
    {

        #region Constants

        public const int TOTAL_LIVES = 20;
        public const double INITIAL_MONEY = 20;

        #endregion

        #region Player-Related Fields

        public int Score { get; set; }
        public double Money { get; set; }
        public int Lives { get; set; }

        #endregion

        #region Gameplay Fields

        public int MaxWaves { get; set; }
        public int WaveNumber { get; set; }
        public Level CurrentLevel { get; set; }
        public TimeSpan TimeToNextWave { get; set; }

        #endregion

        #region Game State Fields

        public bool Paused { get; set; }

        #endregion

        #region Screen Fields

        public Vector2 TopLeftScreen { get; set; }
        public Vector2 BottomRightScreen { get; set; }
        public Point PixelTopLeft { get; set; }
        public Point PixelBottomRight { get; set; }

        #endregion

        #region Static Properties


        private static GameState singleton = null;

        /// <summary>
        /// The only available object of this class.
        /// </summary>
        public static GameState Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new GameState();
                }

                return singleton;
            }
            private set
            {
                singleton = value;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.  It's private to comply with
        /// the singleton design pattern.
        /// </summary>
        private GameState()
        {
            Score = 0;
            Money = 0;
            Lives = TOTAL_LIVES;
            WaveNumber = 0;
        }

        #endregion

        /// <summary>
        /// Convert a location in level space to a location in screen space.
        /// </summary>
        /// <param name="levelCoord">Location in level space.</param>
        /// <returns>Pixel location in screen space.</returns>
        public Vector2 LevelToPixel(Vector2 levelCoord)
        {
            float xPPWU = (PixelBottomRight.X - PixelTopLeft.X) / (BottomRightScreen.X - TopLeftScreen.X);
            float yPPWU = (PixelBottomRight.Y - PixelTopLeft.Y) / (BottomRightScreen.Y - TopLeftScreen.Y);

            Vector2 ret = levelCoord - TopLeftScreen;
            ret.X *= xPPWU;
            ret.Y *= yPPWU;

            return ret;
        }
    }
}
