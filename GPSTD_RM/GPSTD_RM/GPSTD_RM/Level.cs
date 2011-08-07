using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
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
    /// Represents a single position on the level's grid.  Positions
    /// may or may not be roads, and may or may not be occupied by
    /// a tower.
    /// </summary>
    public struct GridPosition
    {
        public GridPosition(Tower t, bool ir)
        {
            theTower = t;
            isRoad = ir;
        }

        public bool isRoad;
        public Tower theTower;
    }

    /// <summary>
    /// Represents a single level of the game.  Based on geo-coordinates.
    /// </summary>
    public class Level
    {
        #region Constants

        public const int defaultXGridResolution = 800 / 16;
        public const int defaultYGridResolution = 480 / 16;

        #endregion

        #region Size and Position

        /// <summary>
        /// Top left point of the level's bounding rectangle in geo co-ordinates.
        /// </summary>
        public Vector2 TopLeft { get; private set; }

        /// <summary>
        /// Bottom right point of the level's bounding rectangle in geo co-ordinates.
        /// </summary>
        public Vector2 BottomRight { get; private set; }

        /// <summary>
        /// Number of columns in the level grid.
        /// </summary>
        public int XGridResolution { get; set; }

        /// <summary>
        /// Number of rows in the level grid.
        /// </summary>
        public int YGridResolution { get; set; }

        #endregion

        #region Grid Fields

        /// <summary>
        /// The level grid, stored here as a 2-dimensional
        /// array of grid positions.
        /// </summary>
        public GridPosition[,] Grid { get; private set; }


        public Vector2 GridSize { get; private set; }

        private int numRoadSquares;

        #endregion

        #region Player Fields

        public Vector2 PlayerPosition { get; private set; }

        #endregion

        #region Game Object Fields

        /// <summary>
        /// A list of all currently placed towers.
        /// </summary>
        public List<Tower> Towers { get; set; }

        /// <summary>
        /// A list of all currently existing projectiles.
        /// </summary>
        public LinkedList<Projectile> Projectiles = new LinkedList<Projectile>();// TODO: Do this in constructor??

        /// <summary>
        /// The creeps!!!
        /// </summary>
        public LinkedList<Creep> Creeps = new LinkedList<Creep>();

        /// <summary>
        /// TODO
        /// </summary>
        public LinkedList<SpawnPoint> NewSpawners;

        /// <summary>
        /// TODO
        /// </summary>
        public LinkedList<SpawnPoint> ReadySpawners;

        #endregion

        #region Graphics Fields

        private BasicEffect effect;

        private GraphicsDevice graphicsDevice;

        private VertexBuffer vb;

        private bool updateBuffer;

        /// <summary>
        /// A reference to a sprite batch so that the level can
        /// be drawn.
        /// </summary>
        public SpriteBatch Batch { get; set; }

        /// <summary>
        /// A texture representing the background of this entire level.
        /// </summary>
        public Texture2D LevelTexture { get; set; }

        #endregion

        #region Other Fields

        /// <summary>
        /// The Game1 object containing this level.
        /// </summary>
        private Game1 game;

        /// <summary>
        /// The name of the location on which to focus (when requesting this
        /// level from a server).
        /// </summary>
        public string FocusLocation { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// This constructor uses the default x and y grid resolutions, and
        /// the midpoint as the player position.
        /// </summary>
        /// <param name="game">The containing game.</param>
        /// <param name="topLeft">Top left of the bounding rectangle.</param>
        /// <param name="bottomRight">Bottom right of the bounding rectangle.</param>
        public Level(Game1 game, Vector2 topLeft, Vector2 bottomRight)
            : this(game, topLeft, bottomRight, defaultXGridResolution, defaultYGridResolution)
        {
        }

        /// <summary>
        /// This constructor defaults the player position to the level's mid-point.
        /// </summary>
        /// <param name="game">The containing game.</param>
        /// <param name="topLeft">Top left of the bounding rectangle.</param>
        /// <param name="bottomRight">Bottom right of the bounding rectangle.</param>
        /// <param name="xgr">X Grid Resolution</param>
        /// <param name="ygr">Y Grid Resolution</param>
        public Level(Game1 game, Vector2 topLeft, Vector2 bottomRight, int xgr, int ygr)
            : this(game, topLeft, bottomRight, (topLeft + bottomRight) / 2.0f, xgr, ygr)
        {
        }

        /// <summary>
        /// The full constructor.
        /// </summary>
        /// <param name="game">The containing game.</param>
        /// <param name="topLeft">Top left of the bounding rectangle.</param>
        /// <param name="bottomRight">Bottom right of the bounding rectangle.</param>
        /// <param name="playerPosition">The player's location.</param>
        /// <param name="xgr">X Grid Resolution</param>
        /// <param name="ygr">Y Grid Resolution</param>
        public Level(Game1 game, Vector2 topLeft, Vector2 bottomRight, Vector2 playerPosition, int xgr, int ygr)
        {

            this.game = game;
            this.Batch = game.spriteBatch;
            this.graphicsDevice = game.GraphicsDevice;


            Grid = new GridPosition[xgr, ygr];
            GridSize = new Vector2((bottomRight.X - topLeft.X) / xgr, (bottomRight.Y - topLeft.Y) / ygr);
            numRoadSquares = 0;

            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
            this.XGridResolution = xgr;
            this.YGridResolution = ygr;

            this.PlayerPosition = playerPosition;

            NewSpawners = new LinkedList<SpawnPoint>();
            ReadySpawners = new LinkedList<SpawnPoint>();

            updateBuffer = true;

            Towers = new List<Tower>();
        }

        #endregion

        #region Spawn Points

        /// <summary>
        /// Create the list of spawn point arrays randomly.  This method should only be used
        /// when random spawn points are desired.
        /// </summary>
        /// <param name="numOnTop">Number of spawn points along the top edge.</param>
        /// <param name="numOnLeft">Number of spawn points along the left edge.</param>
        /// <param name="numOnRight">Number of spawn points along the right edge.</param>
        /// <param name="numOnBottom">Number of spawn points along the bottom edge.</param>
        /// <param name="numPerLevel">Number of spawn-points activated per activation.</param>
        /// <param name="levelsBetweenActivation">Number of levels it waits before activating more spawn points.</param>
        public void InitializeSpawnPoints(
            int numOnTop, int numOnLeft, int numOnRight, int numOnBottom,
            int numPerLevel, int levelsBetweenActivation)
        {
            Vector2 tl = TopLeft;
            Vector2 br = BottomRight;

            float width = BottomRight.X - TopLeft.X;
            float height = BottomRight.Y - TopLeft.Y;

            float xOffsetTop = width / numOnTop;
            float xOffsetBottom = width / numOnBottom;
            float yOffsetLeft = height / numOnLeft;
            float yOffsetRight = height / numOnRight;

            #region Creating the Location Lists

            // Top locations
            //List<SpawnPoint> allLocations = new List<SpawnPoint>(numOnTop + numOnLeft + numOnRight + numOnBottom);
            for (int i = 0; i < numOnTop; i++)
            {
                Vector2 current = new Vector2(tl.X + (xOffsetTop * (i + 0.5f)), tl.Y);
                NewSpawners.AddLast(new SpawnPoint(game, current, PlayerPosition));
            }
            // Left locations
            for (int i = 0; i < numOnLeft; i++)
            {
                Vector2 current = new Vector2(tl.X, tl.Y + (yOffsetLeft * (i + 0.5f)));
                NewSpawners.AddLast(new SpawnPoint(game, current, PlayerPosition));
            }
            // Right locations
            for (int i = 0; i < numOnRight; i++)
            {
                Vector2 current = new Vector2(br.X, tl.Y + (yOffsetRight * (i + 0.5f)));
                NewSpawners.AddLast(new SpawnPoint(game, current, PlayerPosition));
            }
            // Bottom locations
            for (int i = 0; i < numOnRight; i++)
            {
                Vector2 current = new Vector2(tl.X + (xOffsetBottom * (i + 0.5f)), br.Y);
                NewSpawners.AddLast(new SpawnPoint(game, current, PlayerPosition));
            }
            #endregion

            effect = new BasicEffect(graphicsDevice);
            effect.VertexColorEnabled = true;
            effect.Alpha = 1f;
            effect.View = Matrix.Identity;
            effect.World = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0f, graphicsDevice.Viewport.Width,
                                    graphicsDevice.Viewport.Height, 0f, 0f, -1.0F);
        }

        #endregion

        #region Drawing and Graphics

        /// <summary>
        /// Draw the level.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Draw(GameTime gameTime)
        {

            // Calculate pixels per world unit
            float xPPWU = LevelTexture.Bounds.Width / (BottomRight.X - TopLeft.X);
            float yPPWU = LevelTexture.Bounds.Height / (BottomRight.Y - TopLeft.Y);

            foreach (SpawnPoint sp in ReadySpawners)
            {
                sp.Draw(gameTime);
            }

            // TODO: Does this actually do anything...?
            /*
            if (numRoadSquares > 0)
            {
                foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    /*
                    graphicsDevice.SetVertexBuffer(vb);
                    graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, numRoadSquares * 6 - 1);
                     //* /
                }
            }*/

            foreach (Tower t in Towers)
            {
                t.Draw(gameTime);
            }

            foreach (Projectile p in Projectiles)
            {
                p.Draw(gameTime);
            }
        }

        /// <summary>
        /// Initialize the vertex buffer.
        /// </summary>
        private void MakeBuffer()
        {
            if (numRoadSquares == 0) return;
            if (!updateBuffer) return;

            VertexPositionColor[] verts = new VertexPositionColor[numRoadSquares * 6];

            int count = 0;
            for (int x = 0; x < XGridResolution; x++)
            {
                for (int y = 0; y < YGridResolution; y++)
                {
                    if (Grid[x, y].isRoad)
                    {
                        //Vector2 placeOnScreen = (position - center) + BingMapsViewer.ScreenCenter + game.bingMapsViewer.Offset;

                        //TODO: Deal with making sure the grid works properly!
                        verts[6 * count] = new VertexPositionColor(new Vector3(GridSize.X * x, GridSize.Y * y, 0), Color.Blue);
                        verts[6 * count + 1] = new VertexPositionColor(new Vector3(GridSize.X * (x + 1), GridSize.Y * y, 0), Color.Blue);
                        verts[6 * count + 2] = new VertexPositionColor(new Vector3(GridSize.X * x, GridSize.Y * (y + 1), 0), Color.Blue);

                        verts[6 * count + 3] = new VertexPositionColor(new Vector3(GridSize.X * (x + 1), GridSize.Y * (y + 1), 0), Color.Blue);
                        verts[6 * count + 5] = new VertexPositionColor(new Vector3(GridSize.X * (x + 1), GridSize.Y * y, 0), Color.Blue);
                        verts[6 * count + 4] = new VertexPositionColor(new Vector3(GridSize.X * x, GridSize.Y * (y + 1), 0), Color.Blue);

                        count++;
                    }
                }
            }

            vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 6 * numRoadSquares, BufferUsage.None);

            vb.SetData(verts);

            updateBuffer = false;

        }

        #endregion

        #region Updating

        public void Update(GameTime gameTime)
        {

            // Certain towers have been flagged for removal.
            // This is where they are removed.
            List<Tower> towerRemoves = new List<Tower>();
            foreach (Tower t in Towers)
            {
                if (!t.Alive) towerRemoves.Add(t);
                t.Update(gameTime);
            }
            foreach (Tower t in towerRemoves)
            {
                Towers.Remove(t);
            }

            // Certain projectiles have been flagged for removal.
            // This is where they are removed.
            List<Projectile> removes = new List<Projectile>();
            foreach (Projectile p in Projectiles)
            {
                p.Update(gameTime);
                if (!p.Alive)
                    removes.Add(p);
            }

            foreach (Projectile p in removes)
            {
                Projectiles.Remove(p);
            }

            CreateGrid();

            MakeBuffer();
        }

        #endregion

        #region Tower Placement and Removal

        public void PlaceTower(Tower t, Vector2 m, float width)
        {
            PlaceTower(t, (int)(m.X / GridSize.X), (int)(m.Y / GridSize.Y), (int)(width / GridSize.X), (int)(width / GridSize.Y));
        }

        public void PlaceTower(Tower t, int x, int y, int width, int height)
        {

            int lx = (int)(x - width / 2);
            int ly = (int)(y - width / 2);
            int hx = lx + width;
            int hy = ly + height;

            // Attempt to place the tower, if possible
            // TODO: Handle this better?
            try
            {
                // Don't do anything if there is a tower or a road in the way.
                for (int i = lx + 1; i < hx - 1; i++)
                {
                    for (int j = ly + 1; j < hy - 1; j++)
                    {
                        if (Grid[i, j].isRoad)
                        {
                            return;
                        }
                        if (Grid[i, j].theTower != null)
                            return;
                    }
                }
            }
            // If part of the tower goes out of range, just return
            catch (IndexOutOfRangeException)
            {
                return;
            }

            // Put it in the collection
            Towers.Add(t);

            t.Position = new Vector2(x * GridSize.X, y * GridSize.Y);

            // Don't do anything if there is a tower or a road in the way.

            //TODO: Does this actually work?
            // If not, should it be removed from the collection..?
            for (int i = lx; i < hx; i++)
            {
                for (int j = ly; j < hy; j++)
                {
                    Grid[i, j].theTower = t;
                }
            }
        }

        /// <summary>
        /// Remove a tower and set all of the grid squares'
        /// references to it to null.
        /// </summary>
        /// <param name="x">Grid x coordinate for removal.</param>
        /// <param name="y">Grid y coordinate for removal.</param>
        public void RemoveTower(int x, int y)
        {
            Tower t = Grid[x, y].theTower;
            if (t == null) return;

            for (; y >= 0 && Grid[x, y].theTower == t; y--) ;
            y++;
            for (; x >= 0 && Grid[x, y].theTower == t; x--) ;
            x++;

            for (int y1 = y; Grid[x, y1].theTower == t; y1++)
            {
                for (int x1 = x; Grid[x1, y1].theTower == t; x1++)
                {
                    Grid[x1, y1].theTower = null;
                }
            }
        }

        #endregion

        #region Grid Methods

        /// <summary>
        /// Creates the grid after all the path lines have been loaded.  It scans each line, and determines
        /// whether or not each cell of the grid is on the line.
        /// </summary>
        private void CreateGrid()
        {
            foreach (SpawnPoint s in ReadySpawners)
            {
                if (!s.ready) continue;
                if (s.hasBeenChecked) continue;
                //System.Diagnostics.Debug.WriteLine("Checking " + s.wayPoints.First.Value.first.X);

                foreach (Link l in s.WayPoints)
                {
                    #region The Walking Algorithm

                    if (l.first.Equals(l.last)) continue;
                    Vector2 p1 = new Vector2((l.first.X + 400) / GridSize.X, (l.first.Y + 240) / GridSize.Y);
                    Vector2 p2 = new Vector2((l.last.X + 400) / GridSize.X, (l.last.Y + 240) / GridSize.Y);

                    //Vector2 gp1 = new Vector2( l.first.
                    float dxa = Math.Abs(p1.X - p2.X);
                    float dya = Math.Abs(p1.Y - p2.Y);

                    //System.Diagnostics.Debug.WriteLine("Doing line fill");
                    if (dxa > dya)
                    {
                        // System.Diagnostics.Debug.WriteLine("Dependant variable y");

                        float x1, x2, y, dy;
                        if (p1.X < p2.X)
                        {
                            x1 = p1.X;
                            x2 = p2.X;
                            y = p1.Y;
                            dy = (p2.Y - p1.Y) / dxa;
                        }
                        else
                        {
                            x1 = p2.X;
                            x2 = p1.X;
                            y = p2.Y;
                            dy = (p1.Y - p2.Y) / dxa;
                        }

                        if (x1 >= 0 && x1 < XGridResolution && y >= 0 && y < YGridResolution)
                        {
                            //System.Diagnostics.Debug.WriteLine(" ->" + (int)x1 + "|" + (int)y);
                            Grid[(int)x1, (int)y].isRoad = true;
                        }

                        y += dy * ((float)Math.Ceiling(x1) - x1);

                        for (int x = (int)Math.Ceiling(x1); x < (int)Math.Ceiling(x2); x++)
                        {
                            if (x >= 0 && x < XGridResolution && y >= 0 && y < YGridResolution)
                            {
                                // System.Diagnostics.Debug.WriteLine(" ->" + (int)x + "|" + (int)y);
                                Grid[(int)x, (int)y].isRoad = true;
                            }
                            y += dy;
                        }
                    }
                    else
                    {
                        float y1, y2, x, dx;
                        if (p1.Y < p2.Y)
                        {
                            y1 = p1.Y;
                            y2 = p2.Y;
                            x = p1.X;
                            dx = (p2.X - p1.X) / dya;
                        }
                        else
                        {
                            y1 = p2.Y;
                            y2 = p1.Y;
                            x = p2.X;
                            dx = (p1.X - p2.X) / dya;
                        }

                        if (y1 >= 0 && y1 < YGridResolution && x >= 0 && x < XGridResolution)
                        {
                            Grid[(int)x, (int)y1].isRoad = true;
                        }

                        x += dx * ((float)Math.Ceiling(y1) - y1);

                        for (int y = (int)Math.Ceiling(y1); y < (int)Math.Ceiling(y2); y++)
                        {
                            if (y >= 0 && y < YGridResolution && x >= 0 && x < XGridResolution)
                            {
                                Grid[(int)x, (int)y].isRoad = true;
                            }
                            x += dx;
                        }
                    }

                    #endregion
                }
                s.hasBeenChecked = true;
                updateBuffer = true;
            }
        }

        #endregion
    }
}
