using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSTD_RM
{
    public static class CreepFactory
    {
        public static Game1 Game { get; set; }

        #region Default functions

        public static int defaultStats(Creep c)
        {
            return c.Wavenum;
        }

        public static float defaultSpeed(Creep c)
        {
            return 1f;
        }

        #endregion

        //TODO : Replace all of these with sprites..!

        #region Creep Creation

        public static Creep GreenTank(int waveNum)
        {
            return null;
        }

        public static Creep TransportTruck(int waveNum)
        {
            Creep ret = new Creep(Game);

            ret.Wavenum = waveNum;
            ret.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Car4, GameObject.DEFAULT_LOOP);
            ret.Speed = 1.5f;
            ret.hp = 20;
            ret.moneyValue = 0.3;
            ret.pointValue = 1;

            //TODO: Calculate stats!!

            return ret;
        }

        public static Creep Car1(int waveNum)
        {
            Creep ret = new Creep(Game);

            ret.Wavenum = waveNum;
            ret.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Car3, GameObject.DEFAULT_LOOP);
            ret.Speed = 2;
            ret.hp = 10;
            ret.moneyValue = 0.3;
            ret.pointValue = 1;

            //TODO: Calculate stats!!

            return ret;
        }

        public static Creep Car2(int waveNum)
        {
            Creep ret = new Creep(Game);

            ret.Wavenum = waveNum;
            ret.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Car4, GameObject.DEFAULT_LOOP);
            ret.Speed = 2;
            ret.hp = 10;
            ret.moneyValue = 0.3;
            ret.pointValue = 1;

            //TODO: Calculate stats!!

            return ret;
        }

        public static Creep Car3(int waveNum)
        {
            Creep ret = new Creep(Game);

            ret.Wavenum = waveNum;
            ret.ObjectSprite = new AnimatedSpriteInstance(GraphicsPool.Car3, GameObject.DEFAULT_LOOP);
            ret.Speed = 2;
            ret.hp = 10;
            ret.moneyValue = 0.3;
            ret.pointValue = 1;

            //TODO: Calculate stats!!

            return ret;
        }

        #endregion

    }
}
