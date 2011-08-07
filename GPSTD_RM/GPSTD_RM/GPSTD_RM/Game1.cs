using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using System.Device.Location;
using System.Net;
using System.Xml.Linq;
using System.Xml;


using System.Text;
using System.IO.IsolatedStorage;

using BingMaps;
using System.IO;

namespace GPSTD_RM
{


    public struct Screens
    {
        public SplashScreen Splash;
        public MainScreen Main;
        public StartScreen Start;
        public ScoresScreen Scores;
        public AchievementsScreen Achievements;
        public HelpScreen Help;
        public AboutScreen About;
        public NewGameScreen NewGame;
        public BlackScreen Black;
        public PlayingScreen Playing;
        //KB
    }
    //enum Screens { Splash, Main, Start, Scores, Achievements, Help, About, NewGame, Black, Playing, KB, NUM_ELEMS };
    public enum Selected { None, Bomb, Robot1, Robot2, Robot3, Robot4, NUM_ELEMS };

    #region Field Structs

    /// <summary>
    /// All sound effects that need to be loaded.  Kept in a separate
    /// struct for organization
    /// </summary>
    public struct SFX
    {
        public SoundEffect menuSound;
        public SoundEffectInstance menuSoundInstance;
        public SoundEffect buttonSound;
    }

    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        // There are quite a few fields that need to stay organized in this class.
        #region Fields

        #region Graphics

        public GraphicsDeviceManager graphics;
        
        public SpriteBatch spriteBatch;

        public SpriteFont font;

        #endregion

        #region Sound

        public SFX sfx;

        #endregion

        #region Input

        /// <summary>
        /// Current location of the finger on screen.
        /// </summary>
        public Vector2 fingerPosition;

        /// <summary>
        /// A hitbox for the finger!
        /// </summary>
        public Rectangle finger;

        #endregion

        #region Gameplay Fields

        public double money_text = 10.0;
        public int score = 0;
        public int total_lives = 20;

        public bool paused;

        /// <summary>
        /// The current level being played.
        /// </summary>
        public Level CurrentLevel { get; set; }

        #endregion

        #region GPS and Maps Fields

        public GeoCoordinate startingCoordinate;
        public Vector2 startingCoordinateXY;
        public BingMapsViewer bingMapsViewer;
        public const string BingAppKey = "AmvMeUxDlhSw1sGEbHqgDT7qyUoTRhSP7Ta0MFlVcwhWhcEILdMXK2wgi2hhXNr_";
        WebClient locationWebClient;
        WebClient searchWebClient;

        public int ZoomLevel { get; private set; }

        public String locationToFocusOn;

        #region Geolocation

        public GeoCoordinateWatcher watcher;

        public GeoCoordinate currentLocation;

        public GeoPositionStatus currentState;

        #endregion

        #endregion

        #region Screen Fields

        public Screens screens;

        public Screen screen;
        
        public Selected selected;
        
        #endregion

        #region Other Fields

        public int time = 0;
        public bool display_victory = false;

        #endregion

        #endregion

        #region Constructors and Initialization

        public Game1()
        {

            // Graphics initialization

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            graphics.IsFullScreen = true;
            Guide.IsScreenSaverEnabled = false;

            // Input initialization
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.FreeDrag | GestureType.DragComplete;

            // Web Client Initialization
            locationWebClient = new WebClient();
            locationWebClient.OpenReadCompleted += new OpenReadCompletedEventHandler(ReceivedLocationCoordinates);

            searchWebClient = new WebClient();
            searchWebClient.OpenReadCompleted += new OpenReadCompletedEventHandler(GotSearchResult);

            ZoomLevel = 16;

            // The coordinate that the app will focus
            // TODO: This must be changed somehow!
            startingCoordinate = new GeoCoordinate(47.639597, -122.12845); //TODO load me

            // Geolocation Initialization
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(geoWatcher_Position);
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(geoWatcher_StatusChanged);
            watcher.Start();

            // Gameplay Initialization
            CreepFactory.Game = this;

            paused = false;

            // Loading the screens
            screens.Main = new MainScreen(this);
            screens.Help = new HelpScreen(this);
            screens.NewGame = new NewGameScreen(this);
            screens.Playing = new PlayingScreen(this);
            screens.Scores = new ScoresScreen(this);
            screens.Splash = new SplashScreen(this);
            screens.Start = new StartScreen(this);
            screens.About = new AboutScreen(this);
            screens.Achievements = new AchievementsScreen(this);
            screens.Black = new BlackScreen(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();

            screen = screens.Splash;
            screen.Load(GraphicsDevice);
        }

        #endregion

        #region Content

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sfx.buttonSound = Content.Load<SoundEffect>("button");

            font = Content.Load<SpriteFont>("myFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Content Streaming


        #endregion

        #region Drawing and Graphics

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (screen != null)
            {
                screen.Draw(gameTime);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion

        #region Updating


        public double gameStartTime = 0.0;
        public DateTime StartTime = DateTime.Now;
        public int numSpawners = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (screen != screens.Playing)
            {
                fingerPosition.X = 0;
                fingerPosition.Y = 0;
            }
            touch();

            finger = new Rectangle((int)fingerPosition.X, (int)fingerPosition.Y, 1, 1);

            // Update the current screen
            if (screen != null)
            {
                screen.Update(gameTime);
            }

            base.Update(gameTime);
        }

        #endregion

        #region Input Handling Methods

        /// <summary>
        /// Process touch events.
        /// </summary>
        public void touch()
        {
            TouchCollection curTouches = TouchPanel.GetState();

            // Process touch locations
            foreach (TouchLocation location in curTouches)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                    case TouchLocationState.Released:
                    case TouchLocationState.Moved:
                        fingerPosition.X = location.Position.X;
                        fingerPosition.Y = location.Position.Y;
                        break;
                }
            }
        }

       

        #endregion

        #region GPS and Maps Methods

        /// <summary>
        /// Exit handler.
        /// </summary>
        protected override void OnExiting(object sender, EventArgs args)
        {
            if (bingMapsViewer != null) // If we never loaded the map, we don't have to dispose of it.
                bingMapsViewer.ActiveTiles.Dispose();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Event handler called when location coordinates are received.
        /// </summary>
        private void ReceivedLocationCoordinates(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            if (e.Error != null)
            {
                Guide.BeginShowMessageBox("Error focusing on location", e.Error.Message, new string[] { "OK" },
                    0, MessageBoxIcon.Error, null, null);
                return;
            }

            GeoCoordinate receivedCoordinate;

            try
            {
                // Parse the response XML to get the location's geo-coordinate
                XDocument locationResponseDoc = XDocument.Load(e.Result);
                XNamespace docNamespace = locationResponseDoc.Root.GetDefaultNamespace();
                var locationNodes = locationResponseDoc.Descendants(XName.Get("Location", docNamespace.NamespaceName));

                if (locationNodes.Count() == 0)
                {
                    Guide.BeginShowMessageBox("Invalid location",
                        "The requested location was not recognized by the system.", new string[] { "OK" },
                        0, MessageBoxIcon.Error, null, null);
                    return;
                }

                XElement pointNode = locationNodes.First().Descendants(
                    XName.Get("Point", docNamespace.NamespaceName)).FirstOrDefault();

                if (pointNode == null)
                {
                    Guide.BeginShowMessageBox("Invalid location result", "The location result is missing data.",
                        new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }

                XElement longitudeNode = pointNode.Element(XName.Get("Longitude", docNamespace.NamespaceName));
                XElement latitudeNode = pointNode.Element(XName.Get("Latitude", docNamespace.NamespaceName));

                if (longitudeNode == null || latitudeNode == null)
                {
                    Guide.BeginShowMessageBox("Invalid location result", "The location result is missing data.",
                        new string[] { "OK" }, 0, MessageBoxIcon.Error, null, null);
                    return;
                }

                receivedCoordinate = new GeoCoordinate(double.Parse(latitudeNode.Value),
                    double.Parse(longitudeNode.Value));

            }
            catch (Exception err)
            {
                Guide.BeginShowMessageBox("Error getting location coordinates", err.Message, new string[] { "OK" },
                    0, MessageBoxIcon.Error, null, null);
                return;
            }
            startingCoordinate = receivedCoordinate;
            bingMapsViewer.CenterOnLocation(receivedCoordinate);
        }

        #region GeoWatcher

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void geoWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            currentState = e.Status;

            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        string[] strings = { "ok" };
                        //Guide.BeginShowMessageBox("Info", "Please turn on geo-location service in the settings tab.", strings, 0, 0, ExitGame, null);
                    }
                    else
                        if (watcher.Permission == GeoPositionPermission.Granted)
                        {
                            string[] strings = { "ok" };
                            //Guide.BeginShowMessageBox("Error", "Your device doesn't support geo-location service.", strings, 0, 0, ExitGame, null);
                        }
                    break;

                case GeoPositionStatus.Ready:
                    currentLocation = watcher.Position.Location;
                    break;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void geoWatcher_Position(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            switch (currentState)
            {
                case GeoPositionStatus.Ready:
                    currentLocation = watcher.Position.Location;
                    break;
            }
        }

        #endregion

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="result"></param>
        public void LocationSelected(IAsyncResult result)
        {
            locationToFocusOn = Guide.EndShowKeyboardInput(result);

            if (String.IsNullOrEmpty(locationToFocusOn))
            {
                return;
            }

            FocusOnLocationAsync(locationToFocusOn);

            // Cancel any ongoing request, or do nothing if there was none
            //locationWebClient.CancelAsync();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="locationToFocusOn"></param>
        public void FocusOnLocationAsync(string locationToFocusOn)
        {
            searchWebClient.OpenReadAsync(
                new Uri(String.Format(@"http://dev.virtualearth.net/REST/v1/Locations?o=xml&q={0}&key={1}",
                    locationToFocusOn, BingAppKey), UriKind.Absolute));
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GotSearchResult(object sender, OpenReadCompletedEventArgs e)
        {
            Texture2D defaultImage = Helper.LoadTextureStream("Content\\gpsloading", GraphicsDevice);
            Texture2D unavailableImage = Helper.LoadTextureStream("Content\\noImage", GraphicsDevice);
            bingMapsViewer = new BingMapsViewer(BingAppKey, defaultImage, unavailableImage,
                        startingCoordinate, 5, ZoomLevel, spriteBatch);
            ReceivedLocationCoordinates(sender, e);

            screen = screens.Playing;
            screen.Load(GraphicsDevice);
        }

        #endregion

        #region Scores

        /// <summary>
        /// Write scores to a file.
        /// </summary>
        /// <param name="scores">A list of scores.</param>
        public void SaveScores(List<int> scores)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var writeStream = new IsolatedStorageFileStream("scores.txt", FileMode.Create, store))
            using (var writer = new StreamWriter(writeStream))
            {
                foreach (int score in scores)
                {
                    writer.WriteLine(score.ToString());
                }
            }
        }

        /// <summary>
        /// Add a score value to the list and sort the list.
        /// </summary>
        /// <param name="newscore">The new score value.</param>
        public void AddScore(int newscore)
        {
            List<int> highscores = LoadScores();

            highscores.Add(newscore);

            highscores.Sort();
            highscores.Reverse();

            while (highscores.Count > 10)
            {
                highscores.RemoveAt(10);
            }

            SaveScores(highscores);
        }

        /// <summary>
        /// Read scores from a file.
        /// </summary>
        /// <returns>The list of scores.</returns>
        public List<int> LoadScores()
        {
            // load scores
            LinkedList<string> sscores = LoadLinesFromFile("scores.txt");
            List<int> iscores = new List<int>();
            if (sscores != null)
            {
                foreach (string sscore in sscores)
                {
                    iscores.Add(int.Parse(sscore));
                }
            }
            return iscores;
        }

        #endregion

        #region File IO

        /// <summary>
        /// Get a list of strings from a file, corresponding to each line.
        /// </summary>
        /// <param name="fileName">The filename to be read.</param>
        /// <returns>A linkedlist of all lines from the file.</returns>
        public LinkedList<string> LoadLinesFromFile(string fileName)
        {
            try
            {
                LinkedList<string> lines = new LinkedList<string>();

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var readStream = new IsolatedStorageFileStream(fileName, FileMode.Open, store))
                using (var reader = new StreamReader(readStream))
                {
                    while (!reader.EndOfStream)
                    {
                        lines.AddLast(reader.ReadLine());
                    }
                }
                return lines;
            }
            catch (IsolatedStorageException)
            {
                //IsolatedStorageException catch if File cant be opened
                return null;
            }
        }

        #endregion
    }
}
