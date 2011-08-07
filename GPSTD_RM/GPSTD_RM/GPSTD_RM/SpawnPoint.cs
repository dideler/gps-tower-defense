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
using BingMaps;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace GPSTD_RM
{
    /// <summary>
    /// Represents a single link in the chain of points
    /// defining a path.
    /// </summary>
    public struct Link
    {
        /// <summary>
        /// First point in the link.
        /// </summary>
        public Vector2 first;

        /// <summary>
        /// Last point in the link.
        /// </summary>
        public Vector2 last;

        /// <summary>
        /// A unit vector pointing from first to last.
        /// </summary>
        public Vector2 normalized;

        /// <summary>
        /// Initialize the link with a beginning and ending
        /// point.
        /// </summary>
        /// <param name="first">First point in the link.</param>
        /// <param name="last">Last point in the link.</param>
        public Link(Vector2 first, Vector2 last)
        {
            this.first = first;
            this.last = last;
            normalized = last - first;
            normalized.Normalize();
        }

    }

    /// <summary>
    /// Represents a point where enemies spawn.
    /// </summary>
    public class SpawnPoint : GameObject
    {
        /// <summary>
        /// The path to the player associated with this spawn point.
        /// </summary>
        public LinkedList<Link> WayPoints { get; protected set; }

        /// <summary>
        /// The end of the path (the player's location).
        /// </summary>
        public Vector2 FinalPosition { get; protected set; }

        #region Graphics Fields

        /// <summary>
        /// TODO
        /// </summary>
        private BasicEffect effect;

        /// <summary>
        /// A reference to the currently active graphics device for
        /// graphics operations.
        /// </summary>
        GraphicsDevice graphicsDevice;

        /// <summary>
        /// Colour of the lines used to illustrate the path.
        /// </summary>
        Color pathcolor;

        /// <summary>
        /// Vertex buffer for line drawing.
        /// </summary>
        VertexBuffer vb = null;

        #endregion

        #region Position Fields

        /// <summary>
        /// TODO
        /// </summary>
        GeoCoordinate GeoPosition;

        /// <summary>
        /// TODO
        /// </summary>
        GeoCoordinate FinalGeoPosition;


        /// <summary>
        /// The position relative to the center of the current screen.
        /// TODO: Should this be done this way...?
        /// </summary>
        public Vector2 ScreenPosition
        {
            get
            {
                Vector2 center = (GameState.Singleton.TopLeftScreen + GameState.Singleton.BottomRightScreen) / 2;

                Vector2 position = (Position - center);

                return new Vector2((int)position.X, (int)position.Y); ;
            }
        }

        #endregion

        #region Time Fields
        
        /// <summary>
        /// TODO
        /// </summary>
        public double lastEmissionMS = 0;

        /// <summary>
        /// TODO
        /// </summary>
        public double emitFrequency = Double.MaxValue;

        #endregion

        #region Other Fields

        /// <summary>
        /// A reference to the Game1 object containing this spawn point.
        /// </summary>
        Game1 game;

        /// <summary>
        /// TODO
        /// </summary>
        public bool ready;

        /// <summary>
        /// TODO
        /// </summary>
        public bool started;

        /// <summary>
        /// TODO
        /// </summary>
        public bool hasBeenChecked;

        #endregion

        #region Constructors

        public SpawnPoint(Game1 game, Vector2 startPosition, Vector2 finalPosition)
            : base(game)
        {
            this.game = game;
            this.Position = startPosition;
            this.ObjectTexture = GraphicsPool.Spawner;

            this.graphicsDevice = game.GraphicsDevice;

            this.ready = false;
            this.started = false;
            this.hasBeenChecked = false;

            WayPoints = new LinkedList<Link>();

            FinalPosition = finalPosition;


            //TODO: Does this comply with the new co-ordinate system...?
            Vector2 centerXY = TileSystem.LatLongToPixelXY(game.startingCoordinate, game.bingMapsViewer.ZoomLevel);

            GeoPosition = TileSystem.PixelXYToLatLong(centerXY + startPosition - finalPosition, game.bingMapsViewer.ZoomLevel);
            FinalGeoPosition = TileSystem.PixelXYToLatLong(centerXY, game.bingMapsViewer.ZoomLevel);

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
        /// Create the vertex buffer for path drawing.
        /// </summary>
        private void MakeBuffer()
        {

            Vector2 center = TileSystem.LatLongToPixelXY(game.bingMapsViewer.CenterGeoCoordinate, game.bingMapsViewer.ZoomLevel);
            Vector2 position = TileSystem.LatLongToPixelXY(game.startingCoordinate, game.bingMapsViewer.ZoomLevel);

            pathcolor = new Color(0.8f, 0.5f, 0.5f);

            int NumberItems = WayPoints.Count;
            vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), NumberItems, BufferUsage.None);
            
            // Create a list of vertices equal to the number of items
            VertexPositionColor[] verts = new VertexPositionColor[NumberItems];

            // Randomly select the position of this vertex..  Keep it within a range of 5.
            for (int i = 0; i < NumberItems; i++)
            {
                Vector2 v = WayPoints.ElementAt(i).last;
                position = v + game.startingCoordinateXY;

                Vector2 placeOnScreen = (position - center) + BingMapsViewer.ScreenCenter + game.bingMapsViewer.Offset;

                verts[i] = new VertexPositionColor(new Vector3(placeOnScreen, 1f), pathcolor);
            }
            vb.SetData(verts);
        }

        /// <summary>
        /// Draws the spawn point and its path.
        /// </summary>
        /// <param name="time">The current game time.</param>
        public override void Draw(GameTime time)
        {
            if (!ready) // check if path has been loaded yet
                return;

            // draw the sprite
            base.Draw(time);

            // draw the path
            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                if (WayPoints.Count > 0)
                {
                    MakeBuffer();
                    graphicsDevice.SetVertexBuffer(vb);
                    graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, WayPoints.Count - 1);
                }
            }
        }

        #endregion

        #region Static Creation Methods

        /*
        /// <summary>
        /// Static method for creating a spawn point, along with a list of waypoints, from 2 geoCoordinates.
        /// Note that the starting and final locations may not match up perfectly with the requested locations.
        /// </summary>
        /// <param name="spawn">The starting location.</param>
        /// <param name="goal">The player's location.</param>
        /// <return>A new spawnpoint with fully created starting and ending positions.</return>
        public static SpawnPoint CreateFromGeoCoordinates(Game1 game, GeoCoordinate spawn, GeoCoordinate goal)
        {
            // TODO: should this just be a constructor...?
            return null;
        }*/


        #endregion

        #region Getting GPS Routes

        /// <summary>
        /// Requests a route from the server.  It will result in driving directions from
        /// the source coordinates to the target coordinates.
        /// </summary>
        /// <param name="bingMapKey">API key for this application.</param>
        /// <param name="sourceCoord">Beginning of the path.</param>
        /// <param name="TargetCoord">End of the path.</param>
        void GetRouteFromServer(string bingMapKey, GeoCoordinate sourceCoord, GeoCoordinate TargetCoord)
        {
            started = true;

            //ready = false;
            StringBuilder wayPointParameters = new StringBuilder();

            // Creates the source position argument
            wayPointParameters.Append(string.Format("&wp.{0}={1},{2}", 0, sourceCoord.Latitude,
                    sourceCoord.Longitude));

            // Creates the target position argument
            wayPointParameters.Append(string.Format("&wp.{0}={1},{2}", 1, TargetCoord.Latitude,
                TargetCoord.Longitude));

            string requestUri = string.Format(
                 "http://dev.virtualearth.net/REST/V1/Routes/{2}?optmz=distance&output=xml&rpo=points&key={0}{1}",
                 bingMapKey, wayPointParameters, "Driving");


            // Send The request to the bing map routing service
            WebClient wcRoute = new WebClient();
            wcRoute.OpenReadCompleted += new OpenReadCompletedEventHandler(wcRoute_OpenReadCompleted);
            wcRoute.OpenReadAsync(new Uri(requestUri, UriKind.Absolute));
        }

        /// <summary>
        /// Event handler for the completed bing maps request
        /// </summary>
        /// <param name="sender">Required for event handler.</param>
        /// <param name="e">Required for event handler.</param>
        void wcRoute_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {

                // The response is formatted as XML
                XDocument doc = XDocument.Load(e.Result);

                // Using xml reader to read
                XmlReader reader = doc.CreateReader();

                WayPoints.Clear();
                bool first = true;
                Vector2 fromXY = new Vector2(0, 0), toXY;

                Vector2 centerXY = TileSystem.LatLongToPixelXY(game.bingMapsViewer.CenterGeoCoordinate, game.bingMapsViewer.ZoomLevel);

                while (!reader.EOF)
                {
                    // Finds the relevant elements
                    FindElement(reader, "Point");

                    string latitude = GetValue(reader);
                    string longitude = GetValue(reader);
                    if (latitude != null && longitude != null)
                    {
                        toXY = TileSystem.LatLongToPixelXY(new GeoCoordinate(double.Parse(latitude), double.Parse(longitude)),
                            game.bingMapsViewer.ZoomLevel) - centerXY;
                        if (first)
                        {
                            first = false;
                            fromXY = toXY;
                        }
                        WayPoints.AddLast(new Link(fromXY, toXY));
                        fromXY = toXY;
                    }
                }

                //System.Diagnostics.Debug.WriteLine("-----------------------");
                LinkedListNode<Link> l;
                for (l = WayPoints.Last; l.Previous != null; l = l.Previous)
                {
                    if (Math.Abs(l.Value.first.X) > 400 || Math.Abs(l.Value.first.Y) > 240)
                    {
                        l = l.Next;
                        break;
                    }
                }
                while (WayPoints.First.Value.first != l.Value.first)
                {
                    WayPoints.RemoveFirst();
                }

                MakeBuffer();
                Position = WayPoints.First.Value.first + new Vector2(400, 240);
                ready = true;
                GameState.Singleton.CurrentLevel.NewSpawners.Remove(this);
                GameState.Singleton.CurrentLevel.ReadySpawners.AddLast(this);

            }
        }


        /// <summary>
        /// Advances the cursor to the element with the specified name.
        /// </summary>
        /// <param name="reader">An XML reader.</param>
        /// <param name="name">The name of the desired element.</param>
        void FindElement(XmlReader reader, string name)
        {
            while (reader.Read())
            {
                if (reader.Name == name)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Keep reading until it has a value.
        /// </summary>
        /// <param name="reader">An XML reader.</param>
        /// <returns>The string content at the reader's position.</returns>
        string GetValue(XmlReader reader)
        {
            while (!reader.HasValue && !reader.EOF)
            {
                reader.Read();
            }

            // EOF = end of stream
            if (reader.EOF)
            {
                return null;
            }
            return reader.ReadContentAsString();
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Loads the route from the server, if necessary.
        /// </summary>
        public void Prepare()
        {
            if (!started && !ready)
            {
                GetRouteFromServer(Game1.BingAppKey, GeoPosition, FinalGeoPosition);
            }
        }


        /// <summary>
        /// Spawns a Creep using the creep factory.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void makeCreep(GameTime gameTime)
        {
            int r = new Random().Next() % 4;

            Creep ret;

            //TODO: wave nubers???
            //TODO: Allow for some pre-scripted stuff
            switch (r)
            {
                case 0:
                    ret = CreepFactory.Car1(GameState.Singleton.WaveNumber);
                    break;
                case 1:
                    ret = CreepFactory.Car2(GameState.Singleton.WaveNumber);
                    break;
                case 2:
                    ret = CreepFactory.Car3(GameState.Singleton.WaveNumber);
                    break;
                default:
                    ret = CreepFactory.TransportTruck(GameState.Singleton.WaveNumber);
                    break;
            }

            lastEmissionMS = gameTime.TotalGameTime.TotalMilliseconds;

            ret.Position = this.Position;
            ret.SpawnPoint = this;
            ret.CurrentLink = this.WayPoints.First;
            GameState.Singleton.CurrentLevel.Creeps.AddLast(ret);
        }

        #endregion
    }
}
