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
    /// <summary>
    /// This is a static collection of all sprites and textures used by game objects.
    /// </summary>
    public class GraphicsPool
    {

        #region Properties

        // Towers
        private AnimatedSprite basicTower;
        public static AnimatedSprite BasicTower
        {
            get
            {
                return Singleton.basicTower;
            }
            set
            {
                Singleton.basicTower = value;
            }
        }

        private AnimatedSprite slowTower;
        public static AnimatedSprite SlowTower
        {
            get
            {
                return Singleton.slowTower;
            }
            set
            {
                Singleton.slowTower = value;
            }
        }

        private AnimatedSprite aoeTower;
        public static AnimatedSprite AOETower
        {
            get
            {
                return Singleton.aoeTower;
            }
            set
            {
                Singleton.aoeTower = value;
            }
        }

        private AnimatedSprite laserTower;
        public static AnimatedSprite LaserTower
        {
            get
            {
                return Singleton.laserTower;
            }
            set
            {
                Singleton.laserTower = value;
            }
        }

        private AnimatedSprite bombTower;
        public static AnimatedSprite BombTower
        {
            get
            {
                return Singleton.bombTower;
            }
            set
            {
                Singleton.bombTower = value;
            }
        }

        // Creeps
        private AnimatedSprite car1;
        public static AnimatedSprite Car1
        {
            get
            {
                return Singleton.car1;
            }
            set
            {
                Singleton.car1 = value;
            }
        }

        private AnimatedSprite car2;
        public static AnimatedSprite Car2
        {
            get
            {
                return Singleton.car2;
            }
            set
            {
                Singleton.car2 = value;
            }
        }

        private AnimatedSprite car3;
        public static AnimatedSprite Car3
        {
            get
            {
                return Singleton.car3;
            }
            set
            {
                Singleton.car3 = value;
            }
        }

        private AnimatedSprite car4;
        public static AnimatedSprite Car4
        {
            get
            {
                return Singleton.car4;
            }
            set
            {
                Singleton.car4 = value;
            }
        }

        // Buelettes
        private AnimatedSprite bullet1;
        public static AnimatedSprite Bullet1
        {
            get
            {
                return Singleton.bullet1;
            }
            set
            {
                Singleton.bullet1 = value;
            }
        }

        private AnimatedSprite bullet2;
        public static AnimatedSprite Bullet2
        {
            get
            {
                return Singleton.bullet2;
            }
            set
            {
                Singleton.bullet2 = value;
            }
        }

        private AnimatedSprite bullet3;
        public static AnimatedSprite Bullet3
        {
            get
            {
                return Singleton.bullet3;
            }
            set
            {
                Singleton.bullet3 = value;
            }
        }

        private AnimatedSprite bullet4;
        public static AnimatedSprite Bullet4
        {
            get
            {
                return Singleton.bullet4;
            }
            set
            {
                Singleton.bullet4 = value;
            }
        }

        private AnimatedSprite bullet5;
        public static AnimatedSprite Bullet5
        {
            get
            {
                return Singleton.bullet5;
            }
            set
            {
                Singleton.bullet5 = value;
            }
        }

        // Explosion animation
        private AnimatedSprite ex;
        public static AnimatedSprite Ex
        {
            get
            {
                return Singleton.ex;
            }
            set
            {
                Singleton.ex = value;
            }
        }

        // Layzor
        private AnimatedSprite laser;
        public static AnimatedSprite Laser
        {
            get
            {
                return Singleton.laser;
            }
            set
            {
                Singleton.laser = value;
            }
        }

        private Texture2D spawner;
        public static Texture2D Spawner
        {
            get
            {
                return Singleton.spawner;
            }
            set
            {
                Singleton.spawner = value;
            }
        }

        #endregion

        private static GraphicsPool singleton = null;
        private static GraphicsPool Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new GraphicsPool();
                }

                return singleton;
            }
        }

        private GraphicsPool()
        {
        }

    }
}
