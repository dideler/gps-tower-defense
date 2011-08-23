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
    public class PlayingScreen : Screen
    {

        #region Menu Textures

        public Texture2D bomb_icon;
        public Texture2D robot1_icon;
        public Texture2D robot2_icon;
        public Texture2D robot3_icon;
        public Texture2D robot4_icon;

        public Texture2D time_img;
        public Texture2D health;
        public Texture2D money;
        public Texture2D trash;

        #endregion

        #region Sprites

        

        #endregion

        #region Other Textures

        public Texture2D spawnTexture2D;
        public Texture2D blank;

        #endregion

        #region Hitboxes

        Rectangle trash_hitbox;
        Rectangle bomb_hitbox;
        Rectangle robot1_hitbox;
        Rectangle robot2_hitbox;
        Rectangle robot3_hitbox;
        Rectangle robot4_hitbox;

        #endregion

        Selected selected;

        public PlayingScreen(Game1 game)
            : base(game)
        {
        }

        /// <summary>
        /// Load all the sprites and their textures from the appropriate files.
        /// </summary>
        /// <param name="gDev">To stream the sprites, the graphics device is needed.</param>
        public override void Load(GraphicsDevice gDev)
        {

            bomb_icon = Helper.LoadTextureStream("Content\\bomb_icon.png", gDev);
            robot1_icon = Helper.LoadTextureStream("Content\\robot1.png", gDev);
            robot2_icon = Helper.LoadTextureStream("Content\\robot2.png", gDev);
            robot3_icon = Helper.LoadTextureStream("Content\\robot3.png", gDev);
            robot4_icon = Helper.LoadTextureStream("Content\\robot4.png", gDev);

            time_img = Helper.LoadTextureStream("Content\\time.png", gDev);
            health = Helper.LoadTextureStream("Content\\health.png", gDev);
            money = Helper.LoadTextureStream("Content\\money.png", gDev);
            trash = Helper.LoadTextureStream("Content\\trashcan-small.png", gDev);


            // Towers
            GraphicsPool.BasicTower = AnimatedSprite.SpriteFromFile("Content\\towers\\basic_tower.xml", gDev);
            GraphicsPool.SlowTower = AnimatedSprite.SpriteFromFile("Content\\towers\\slow_tower.xml", gDev);
            GraphicsPool.AOETower = AnimatedSprite.SpriteFromFile("Content\\towers\\aoe_tower.xml", gDev);
            GraphicsPool.LaserTower = AnimatedSprite.SpriteFromFile("Content\\towers\\laser_tower.xml", gDev);
            GraphicsPool.BombTower = AnimatedSprite.SpriteFromFile("Content\\towers\\bomb_tower.xml", gDev);

            // Creeps
            GraphicsPool.Car1 = AnimatedSprite.SpriteFromFile("Content\\creeps\\car1.xml", gDev);
            GraphicsPool.Car2 = AnimatedSprite.SpriteFromFile("Content\\creeps\\car2.xml", gDev);
            GraphicsPool.Car3 = AnimatedSprite.SpriteFromFile("Content\\creeps\\car3.xml", gDev);
            GraphicsPool.Car4 = AnimatedSprite.SpriteFromFile("Content\\creeps\\car4.xml", gDev);

            // Bullets
            GraphicsPool.Bullet1 = AnimatedSprite.SpriteFromFile("Content\\projectiles\\bullet1.xml", gDev);
            GraphicsPool.Bullet2 = AnimatedSprite.SpriteFromFile("Content\\projectiles\\bullet2.xml", gDev);
            GraphicsPool.Bullet3 = AnimatedSprite.SpriteFromFile("Content\\projectiles\\bullet3.xml", gDev);
            GraphicsPool.Bullet4 = AnimatedSprite.SpriteFromFile("Content\\projectiles\\bullet4.xml", gDev);
            GraphicsPool.Bullet5 = AnimatedSprite.SpriteFromFile("Content\\projectiles\\bullet5.xml", gDev);

            // Explosion
            GraphicsPool.Ex = AnimatedSprite.SpriteFromFile("Content\\projectiles\\ex.xml", gDev);

            // Laser
            GraphicsPool.Laser = AnimatedSprite.SpriteFromFile("Content\\projectiles\\laser.xml", gDev);

            spawnTexture2D = Helper.LoadTextureStream("Content\\spawner.png", gDev);

            GraphicsPool.Spawner = spawnTexture2D;

            blank = Helper.LoadTextureStream("Content\\blank.png", gDev);

            

            

            // Hitboxes so we can detect when finger selects an item
            trash_hitbox = new Rectangle(trash.Width / 2, gDev.Viewport.Height - trash.Height, trash.Width, trash.Height);
            bomb_hitbox = new Rectangle(gDev.Viewport.Width - bomb_icon.Width, gDev.Viewport.Height - bomb_icon.Height, bomb_icon.Width, bomb_icon.Height);
            robot1_hitbox = new Rectangle(gDev.Viewport.Width - (robot1_icon.Width * 2), gDev.Viewport.Height - robot1_icon.Height, robot1_icon.Width, robot1_icon.Height);
            robot2_hitbox = new Rectangle(gDev.Viewport.Width - (robot2_icon.Width * 3), gDev.Viewport.Height - robot2_icon.Height, robot2_icon.Width, robot2_icon.Height);
            robot3_hitbox = new Rectangle(gDev.Viewport.Width - (robot3_icon.Width * 4), gDev.Viewport.Height - robot3_icon.Height, robot3_icon.Width, robot3_icon.Height);
            robot4_hitbox = new Rectangle(gDev.Viewport.Width - (robot4_icon.Width * 5), gDev.Viewport.Height - robot4_icon.Height, robot4_icon.Width, robot4_icon.Height);


            // TODO: Coords
            GameState.Singleton.TopLeftScreen = new Vector2(0, 0);
            GameState.Singleton.BottomRightScreen = new Vector2(400, 240);
            GameState.Singleton.PixelTopLeft = Point.Zero;
            GameState.Singleton.PixelBottomRight = new Point(gDev.Viewport.Width, gDev.Viewport.Height);

            // Bing maps viewer setup
            Texture2D defaultImage = Helper.LoadTextureStream("Content\\gpsloading.png", gDev);
            Texture2D unavailableImage = Helper.LoadTextureStream("Content\\noImage.png", gDev);

            theGame.gameStartTime = 0.0;
            theGame.numSpawners = 0;

            theGame.bingMapsViewer = new BingMapsViewer(Game1.BingAppKey, defaultImage, unavailableImage,
                        theGame.startingCoordinate, 5, theGame.ZoomLevel, theGame.spriteBatch);

            theGame.startingCoordinateXY = TileSystem.LatLongToPixelXY(theGame.startingCoordinate,
                theGame.bingMapsViewer.ZoomLevel);

            // Initialize the level
            GameState.Singleton.CurrentLevel = new Level(theGame, new Vector2(0, 0), new Vector2(800, 480))
            {
                LevelTexture = Helper.LoadTextureStream("Content\\LevelTest.png", gDev),
                Batch = theGame.spriteBatch,
            };

            GameState.Singleton.CurrentLevel.InitializeSpawnPoints(5, 5, 5, 5, 1, 1);
            Random random = new Random();
            for (int i = 0; i < 1; i++)
            {
                for (int attempt = 0; attempt < 15; attempt++)
                {
                    int r = random.Next(GameState.Singleton.CurrentLevel.NewSpawners.Count);
                    SpawnPoint spawner = GameState.Singleton.CurrentLevel.NewSpawners.ElementAt(r);
                    if (!spawner.started)
                    {
                        spawner.Prepare();
                        spawner.emitFrequency = 1000;
                        break;
                    }
                }
            }

            GameState.Singleton.CurrentLevel.PlaceTower(BasicTower.FactoryMake(theGame),
                    new Vector2(300, 300), 20);
        }

        /// <summary>
        /// Set all the sprite values to null.
        /// Note: All objects referencing the sprites must be destroyed
        /// for this to work!
        /// </summary>
        public override void Unload()
        {
            bomb_icon = null;
            robot1_icon = null;
            robot2_icon = null;
            robot3_icon = null;
            robot4_icon = null;

            time_img = null;
            health = null;
            money = null;
            trash = null;

            // Towers
            GraphicsPool.BasicTower = null;
            GraphicsPool.SlowTower = null;
            GraphicsPool.AOETower = null;
            GraphicsPool.LaserTower = null;
            GraphicsPool.BombTower = null;

            // Creeps
            GraphicsPool.Car1 = null;
            GraphicsPool.Car2 = null;
            GraphicsPool.Car3 = null;
            GraphicsPool.Car4 = null;

            // Bullets
            GraphicsPool.Bullet1 = null;
            GraphicsPool.Bullet2 = null;
            GraphicsPool.Bullet3 = null;
            GraphicsPool.Bullet4 = null;
            GraphicsPool.Bullet5 = null;

            // Explosion
            GraphicsPool.Ex = null;

            // Laser
            GraphicsPool.Laser = null;
        }

        public override void Draw(GameTime gameTime)
        {
            theGame.bingMapsViewer.Draw();
            GameState.Singleton.CurrentLevel.Draw(gameTime);
            //testBoy.Draw(gameTime);
            if (GameState.Singleton.CurrentLevel.Creeps != null)
            {
                foreach (Creep creep in GameState.Singleton.CurrentLevel.Creeps)
                {
                    creep.Draw(gameTime);
                }
            }
            // Draws score, money, health, etc.
            theGame.spriteBatch.Draw(money, new Rectangle(2, 2, money.Width, money.Height), Color.White);
            string formatted = String.Format("{0:00.00}", GameState.Singleton.Money);
            theGame.spriteBatch.DrawString(theGame.font, formatted, new Vector2(40, 7), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, GameState.Singleton.Score.ToString(), new Vector2(400, 7), Color.Black);
            theGame.spriteBatch.DrawString(theGame.font, GameState.Singleton.Lives.ToString(), new Vector2(766 - health.Width, 7), Color.Black);
            theGame.spriteBatch.Draw(health, new Rectangle(795 - health.Width, 5, health.Width, health.Height), Color.White);
            theGame.spriteBatch.Draw(trash, new Rectangle(trash.Width / 2, theGame.GraphicsDevice.Viewport.Height - trash.Height, trash.Width, trash.Height), Color.White);

            // TODO: Finish the in-game menu interface
            // TODO: Lower robot menu when dragging
            // TODO: Make robot menu part transparent during gameplay
            // The tower/item menu. If item is affordable, draw it fully, otherwise somewhat transparent.
            if (GameState.Singleton.Money >= BombTower.PRICE)
                theGame.spriteBatch.Draw(bomb_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - bomb_icon.Width, theGame.GraphicsDevice.Viewport.Height - bomb_icon.Height, bomb_icon.Width, bomb_icon.Height), Color.White);
            else
                theGame.spriteBatch.Draw(bomb_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - bomb_icon.Width, theGame.GraphicsDevice.Viewport.Height - bomb_icon.Height, bomb_icon.Width, bomb_icon.Height), new Color(255, 255, 255, 127));
            if (GameState.Singleton.Money >= BasicTower.PRICE)
                theGame.spriteBatch.Draw(robot1_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot1_icon.Width * 2), theGame.GraphicsDevice.Viewport.Height - robot1_icon.Height, robot1_icon.Width, robot1_icon.Height), Color.White);
            else
                theGame.spriteBatch.Draw(robot1_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot1_icon.Width * 2), theGame.GraphicsDevice.Viewport.Height - robot1_icon.Height, robot1_icon.Width, robot1_icon.Height), new Color(255, 255, 255, 127));
            if (GameState.Singleton.Money >= SlowTower.PRICE)
                theGame.spriteBatch.Draw(robot2_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot2_icon.Width * 3), theGame.GraphicsDevice.Viewport.Height - robot2_icon.Height, robot1_icon.Width, robot2_icon.Height), Color.White);
            else
                theGame.spriteBatch.Draw(robot2_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot2_icon.Width * 3), theGame.GraphicsDevice.Viewport.Height - robot2_icon.Height, robot1_icon.Width, robot2_icon.Height), new Color(255, 255, 255, 127));
            if (GameState.Singleton.Money >= AOETower.PRICE)
                theGame.spriteBatch.Draw(robot3_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot3_icon.Width * 4), theGame.GraphicsDevice.Viewport.Height - robot3_icon.Height, robot1_icon.Width, robot3_icon.Height), Color.White);
            else
                theGame.spriteBatch.Draw(robot3_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot3_icon.Width * 4), theGame.GraphicsDevice.Viewport.Height - robot3_icon.Height, robot1_icon.Width, robot3_icon.Height), new Color(255, 255, 255, 127));
            if (GameState.Singleton.Money >= LaserTower.PRICE)
                theGame.spriteBatch.Draw(robot4_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot4_icon.Width * 5), theGame.GraphicsDevice.Viewport.Height - robot4_icon.Height, robot1_icon.Width, robot4_icon.Height), Color.White);
            else
                theGame.spriteBatch.Draw(robot4_icon, new Rectangle(theGame.GraphicsDevice.Viewport.Width - (robot4_icon.Width * 5), theGame.GraphicsDevice.Viewport.Height - robot4_icon.Height, robot1_icon.Width, robot4_icon.Height), new Color(255, 255, 255, 127));

            // Draw any dragged items if needed.
            Texture2D selectedtex = null;
            switch (selected)
            {
                case (Selected.None):
                    // NOP
                    break;
                case Selected.Bomb:
                    selectedtex = BombTower.defaultTexture;
                    break;
                case Selected.Robot1:
                    selectedtex = BasicTower.defaultTexture;
                    break;
                case Selected.Robot2:
                    selectedtex = SlowTower.defaultTexture;
                    break;
                case Selected.Robot3:
                    selectedtex = AOETower.defaultTexture;
                    break;
                case Selected.Robot4:
                    selectedtex = LaserTower.defaultTexture;
                    break;
            }
            if (selectedtex != null)
            {
                // Clamps the item to the edge if it tries to go past.
                if (theGame.fingerPosition.X > theGame.graphics.GraphicsDevice.Viewport.Width - (selectedtex.Width / 2)) // Right.
                {
                    theGame.fingerPosition.X = theGame.graphics.GraphicsDevice.Viewport.Width - (selectedtex.Width / 2);
                }
                if (theGame.fingerPosition.Y > theGame.graphics.GraphicsDevice.Viewport.Height - (selectedtex.Height / 2)) // Bottom.
                {
                    theGame.fingerPosition.Y = theGame.graphics.GraphicsDevice.Viewport.Height - (selectedtex.Height / 2);
                }
                if (theGame.fingerPosition.X < 0 + (selectedtex.Width / 2)) // Left.
                {
                    theGame.fingerPosition.X = 0 + (selectedtex.Width / 2);
                }
                if (theGame.fingerPosition.Y < 0 + (selectedtex.Height / 2)) // Top.
                {
                    theGame.fingerPosition.Y = 0 + (selectedtex.Height / 2);
                }
                theGame.spriteBatch.Draw(selectedtex, new Vector2(theGame.fingerPosition.X - (selectedtex.Width / 2), theGame.fingerPosition.Y - (selectedtex.Height / 2)), Color.White);
            }

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {


            if (theGame.gameStartTime != 0.0 && gameTime.TotalGameTime.TotalSeconds - theGame.gameStartTime > 120)  // 90 seconds  
            {
                        
                //spriteBatch.DrawString(font, formatted, new Vector2(40, 7), Color.Black);
                theGame.display_victory = true;
                theGame.AddScore(GameState.Singleton.Score);


                theGame.screen.Unload();
                theGame.screen = theGame.screens.Main;
                theGame.screen.Load(theGame.GraphicsDevice);        
            }
            
            if (theGame.gameStartTime == 0.0)
            {
                theGame.gameStartTime = gameTime.TotalGameTime.Seconds;    
            }

                    if (theGame.numSpawners < (gameTime.TotalGameTime.Seconds - theGame.gameStartTime) / 15)
                    {
                        theGame.numSpawners++;

                        for (int attempt = 0; attempt < 1; attempt++)
                        {
                            int r = (new Random()).Next(GameState.Singleton.CurrentLevel.NewSpawners.Count);
                            SpawnPoint spawner = GameState.Singleton.CurrentLevel.NewSpawners.ElementAt(r);
                            if (!spawner.started)
                            {
                                spawner.Prepare();
                                spawner.emitFrequency = 1000;
                                break;
                            }
                        }
                    }


                    // Cleen up all the dead creeps
                    if (GameState.Singleton.CurrentLevel.Creeps != null)
                    {
                        List<Creep> removes = new List<Creep>();
                        foreach (Creep c in GameState.Singleton.CurrentLevel.Creeps)
                        {
                            if (!c.Alive)
                                removes.Add(c);
                        }
                        foreach (Creep c in removes)
                        {
                            GameState.Singleton.CurrentLevel.Creeps.Remove(c);
                            theGame.Components.Remove(c);
                        }
                    }

                    if (GameState.Singleton.CurrentLevel.ReadySpawners.Count > 0)
                    {

                        foreach (SpawnPoint spawner in GameState.Singleton.CurrentLevel.ReadySpawners)
                        {
                            if (gameTime.TotalGameTime.TotalMilliseconds - spawner.lastEmissionMS > spawner.emitFrequency)
                            {
                                spawner.makeCreep(gameTime);
                            }

                        }

                        foreach (Creep creep in GameState.Singleton.CurrentLevel.Creeps)
                        {
                            creep.Update(gameTime);
                            //does not fully work
                        }

                        // End-of-game conditions

                        // Die
                        if (GameState.Singleton.Lives <= 0)
                        {
                            theGame.screen.Unload();
                            theGame.AddScore(GameState.Singleton.Score);
                            theGame.screen = theGame.screens.Main;
                            theGame.screen.Load(theGame.GraphicsDevice);
                        }

                        // End of Wave Count
                        if (GameState.Singleton.WaveNumber >= 20)
                        {
                            // TODO: Victory
                        }
                    }
                    
            // Allows the game to exit        
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                theGame.Exit();

            handleInput(theGame.finger);
            
            // TODO: Level should be in the game state
            GameState.Singleton.CurrentLevel.Update(gameTime);
         
            base.Update(gameTime);
        }

        /// <summary>
        /// This function processes the input, currently
        /// adding items and towers (using the towers class).
        /// We will have specific towers so be sure to use the sub classes.
        /// The Draw function depends on this function on what item/tower to draw.
        /// </summary>
        /// <param name="finger"></param>
        public void handleInput(Rectangle finger)
        {
            if (TouchPanel.IsGestureAvailable) // if or while?
            {
                GestureSample sample = TouchPanel.ReadGesture();

                switch (selected)
                {
                    case Selected.None: // Check if user has selected an affordable item to drag.
                        if (sample.GestureType == GestureType.FreeDrag && finger.Intersects(bomb_hitbox) && GameState.Singleton.Money >= BombTower.PRICE)
                        {
                            selected = Selected.Bomb;
                        }
                        else if (sample.GestureType == GestureType.FreeDrag && finger.Intersects(robot1_hitbox) && GameState.Singleton.Money >= BasicTower.PRICE)
                        {
                            selected = Selected.Robot1;
                        }
                        else if (sample.GestureType == GestureType.FreeDrag && finger.Intersects(robot2_hitbox) && GameState.Singleton.Money >= SlowTower.PRICE)
                        {
                            selected = Selected.Robot2;
                        }
                        else if (sample.GestureType == GestureType.FreeDrag && finger.Intersects(robot3_hitbox) && GameState.Singleton.Money >= AOETower.PRICE)
                        {
                            selected = Selected.Robot3;
                        }
                        else if (sample.GestureType == GestureType.FreeDrag && finger.Intersects(robot4_hitbox) && GameState.Singleton.Money >= LaserTower.PRICE)
                        {
                            selected = Selected.Robot4;
                        }
                        break;
                    case Selected.Bomb: // Check if user has dropped the dragged bomb.
                        if (sample.GestureType == GestureType.DragComplete) // Note: GestureType.Position is 0,0 for drag.
                        {
                            // Check if it's been placed in the trash can.
                            if (finger.Intersects(trash_hitbox))
                            {
                                // Do nothing; item is trashed.
                            }
                            else // Safe to build.
                            {
                                Tower tower = BombTower.FactoryMake(theGame);
                                GameState.Singleton.CurrentLevel.PlaceTower(tower, theGame.fingerPosition, tower.ObjectTexture.Width);
                                GameState.Singleton.Money -= BombTower.PRICE;
                            }
                            selected = Selected.None; // Either way, we can safely discard the item now.
                        }
                        break;
                    case Selected.Robot1: // Check if user has dropped the dragged robot 1.
                        if (sample.GestureType == GestureType.DragComplete)
                        {
                            if (finger.Intersects(trash_hitbox))
                            {
                                // Do nothing; item is trashed.
                            }
                            else // Safe to build.
                            {
                                Tower tower = BasicTower.FactoryMake(theGame);
                                GameState.Singleton.CurrentLevel.PlaceTower(tower, theGame.fingerPosition, tower.ObjectTexture.Width);
                                GameState.Singleton.Money -= BasicTower.PRICE;
                            }
                            selected = Selected.None;
                        }
                        break;
                    case Selected.Robot2: // Check if user has dropped the dragged robot 2.
                        if (sample.GestureType == GestureType.DragComplete)
                        {
                            if (finger.Intersects(trash_hitbox))
                            {
                                // Do nothing; item is trashed.
                            }
                            else // Safe to build.
                            {
                                Tower tower = SlowTower.FactoryMake(theGame);
                                GameState.Singleton.CurrentLevel.PlaceTower(tower, theGame.fingerPosition, tower.ObjectTexture.Width);
                                GameState.Singleton.Money -= SlowTower.PRICE;
                            }
                            selected = Selected.None;
                        }
                        break;
                    case Selected.Robot3: // Check if user has dropped the dragged robot 3.
                        if (sample.GestureType == GestureType.DragComplete)
                        {
                            if (finger.Intersects(trash_hitbox))
                            {
                                // Do nothing; item is trashed.
                            }
                            else // Safe to build.
                            {
                                Tower tower = AOETower.FactoryMake(theGame);
                                GameState.Singleton.CurrentLevel.PlaceTower(tower, theGame.fingerPosition, tower.ObjectTexture.Width);
                                GameState.Singleton.Money -= AOETower.PRICE;
                            }
                            selected = Selected.None;
                        }
                        break;
                    case Selected.Robot4: // Check if user has dropped the dragged robot 4.
                        if (sample.GestureType == GestureType.DragComplete)
                        {
                            if (finger.Intersects(trash_hitbox))
                            {
                                // Do nothing; item is trashed.
                            }
                            else // Safe to build.
                            {
                                Tower tower = LaserTower.FactoryMake(theGame);
                                GameState.Singleton.CurrentLevel.PlaceTower(tower, theGame.fingerPosition, tower.ObjectTexture.Width);
                                GameState.Singleton.Money -= LaserTower.PRICE;
                            }
                            selected = Selected.None;
                        }
                        break;
                }
            }
        }
    }
}
