using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Using Statements


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using System.Net;


#endregion
using BingMaps;


namespace GPSTD_RM
{
    public enum BingMapsViewType
    {
        Aerial,
        Road
    }

    public class BingMapsViewer
    {
        #region Fields and Properties

        /// <summary>
        /// API key for this application.
        /// </summary>
        public string BingMapKey { get; private set; }

        /// <summary>
        /// The pixel coordinate of the screen center on the entire map image.
        /// </summary>
        public Vector2 ScreenCenterPixelXY
        {
            get
            {
                // The screen center moves opposite the offset
                return TileSystem.LatLongToPixelXY(CenterGeoCoordinate, ZoomLevel) - Offset / zoomScale;
            }
        }

        /// <summary>
        /// One of several zoom levels used in bing maps.
        /// </summary>
        public int ZoomLevel { get; private set; }

        /// <summary>
        /// Reference to the current graphics device so that operations can be performed.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Reference to the current sprite batch so that sprite drawing operations can be performed.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// This will likely always be aerial.
        /// </summary>
        public BingMapsViewType ViewType { get; set; }

        /// <summary>
        /// Currently active tile collection.
        /// </summary>
        public BingMapsTiles ActiveTiles { get; set; }

        /// <summary>
        /// The geo position at the center of the current center tile.
        /// </summary>
        public GeoCoordinate CenterGeoCoordinate;

        /// <summary>
        /// Offset used for drawing the map tiles on the screen.
        /// </summary>
        public Vector2 Offset = Vector2.Zero;

        /// <summary>
        /// The center pixel of the screen.
        /// </summary>
        public static readonly Vector2 ScreenCenter = new Vector2(400, 240);

        /// <summary>
        /// Image displayed before the map loads.
        /// </summary>
        Texture2D defaultImage;

        /// <summary>
        /// Image displayed if the current map can't be displayed.
        /// </summary>
        Texture2D unavailableImage;

        /// <summary>
        /// TODO
        /// </summary>
        readonly Rectangle screenBounds;

        /// <summary>
        /// TODO
        /// </summary>
        readonly Vector2 screenCenterVector;

        /// <summary>
        /// The size of each tile
        /// </summary>
        readonly Vector2 tileDimensions;
        
        /// <summary>
        /// TODO
        /// </summary>
        readonly Vector2 maxOffsetAbs;

        /// <summary>
        /// The current user-defined zoom level.  Note that this is simply
        /// a factor by which to scale the display.  This does not determine
        /// the resolution of the images obtained from bing maps.
        /// </summary>
        float zoomScale = 1;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes new Bing maps viewer.
        /// </summary>
        /// <param name="bingMapKey">The key used to access the Bing maps service.</param>
        /// <param name="defaultImage">The default image to show until actual image data is retrieved.</param>
        /// <param name="unavailableImage">The image to use for tiles where an image is not available.</param>
        /// <param name="location">The initial location to use.</param>
        /// <param name="zoomlevel">The zoom level (1 - 22).</param>
        /// <param name="gridSize">The size of the active tile grid around the desired location. For example, 
        /// specifying 3 will retrieve a 3x3 set of images around the desired location. This number must be
        /// positive and odd.</param>
        /// <param name="spriteBatch">A sprite batch that can be used to draw to the display.</param>        
        public BingMapsViewer(string bingMapKey, Texture2D defaultImage, Texture2D unavailableImage,
            GeoCoordinate location, int gridSize, int zoomlevel, SpriteBatch spriteBatch)
        {
            BingMapKey = bingMapKey;
            GraphicsDevice = spriteBatch.GraphicsDevice;
            SpriteBatch = spriteBatch;

            ZoomLevel = zoomlevel;

            screenBounds = GraphicsDevice.Viewport.Bounds;
            tileDimensions = new Vector2(screenBounds.Width, screenBounds.Height);
            screenCenterVector = tileDimensions / 2;

            this.defaultImage = defaultImage;
            this.unavailableImage = unavailableImage;

            // Initialize the tile set object
            ActiveTiles = new BingMapsTiles(bingMapKey, unavailableImage, spriteBatch, tileDimensions, gridSize)
            {
                ZoomLevel = zoomlevel,
                ViewType = BingMapsViewType.Road
            };

            maxOffsetAbs = tileDimensions * ((ActiveTiles.ActiveTilePlaneSize - 1) / 2);

            ViewType = BingMapsViewType.Road;

            // Initialize the tile set with the appropriate tile images
            CenterGeoCoordinate = location;
            ActiveTiles.InitializeActiveTilePlane(location);
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draw the map on the screen. Assumes that SpriteBatch.Begin has been called.
        /// </summary>
        public void Draw()
        {
            int centerIndex = (ActiveTiles.ActiveTilePlaneSize - 1) / 2;

            Vector2 centerPixelXY = TileSystem.LatLongToPixelXY(CenterGeoCoordinate, ZoomLevel);
            Vector2 adjustedPixelXY = new Vector2(
                (int)(centerPixelXY.X / tileDimensions.X) * tileDimensions.X,
                (int)(centerPixelXY.Y / tileDimensions.Y) * tileDimensions.Y);

            for (int i = 0; i < ActiveTiles.ActiveTilePlaneSize; i++)
            {
                for (int j = 0; j < ActiveTiles.ActiveTilePlaneSize; j++)
                {
                    // Add an offset to the drawn image depending on its position in the tile matrix
                    Vector2 extraOffset = new Vector2((i - centerIndex) * tileDimensions.X,
                        (j - centerIndex) * tileDimensions.Y);

                    Texture2D image = ActiveTiles[i, j].Image;
                    if (image == null)
                    {
                        image = defaultImage;
                    }

                    SpriteBatch.Draw(image, screenCenterVector + Offset + extraOffset, null, Color.White, 0,
                        //centerPixelXY-adjustedPixelXY, 1, SpriteEffects.None, 0);
                        tileDimensions / 2f + (centerPixelXY - adjustedPixelXY), 1, SpriteEffects.None, 0);
                    //tileDimensions / 2f, 1, SpriteEffects.None, 0);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the displayed images by getting them again from the REST services.
        /// </summary>
        public void RefreshImages()
        {
            ActiveTiles.ViewType = ViewType;
            ActiveTiles.InitializeActiveTilePlane(CenterGeoCoordinate);
        }

        /// <summary>
        /// Centers the view on a specified geo-coordinate.
        /// </summary>
        /// <param name="location">Geo-coordinate to center the view on.</param>
        public void CenterOnLocation(GeoCoordinate location)
        {
            CenterGeoCoordinate = location;
            Offset = Vector2.Zero;
            ActiveTiles.InitializeActiveTilePlane(location);
        }

        /// <summary>
        /// Move the map by a specified offset, but prevent a move that would transition to a tile that is off the
        /// active tile set.
        /// </summary>
        /// <param name="gestureOffset">The offset to move the map by.</param>
        public void MoveByOffset(Vector2 gestureOffset)
        {
            Offset += gestureOffset;

            Vector2 fixOffset = Vector2.Zero;

            if (Offset.X < -maxOffsetAbs.X || Offset.X > maxOffsetAbs.X)
            {
                fixOffset.X = -gestureOffset.X;
            }
            if (Offset.Y < -maxOffsetAbs.Y || Offset.Y > maxOffsetAbs.Y)
            {
                fixOffset.Y = -gestureOffset.Y;
            }

            Offset += fixOffset;
        }


        /// <summary>
        /// Convert a position on the device's display to the corresponding geo-coordinate.
        /// </summary>
        /// <param name="screenPosition">Screen position to convert.</param>
        /// <returns>Geo-coordinate which represents the screen position.</returns>
        public GeoCoordinate ConvertScreenPositionToGeoCoordinate(Vector2 screenPosition)
        {
            // Get vector from the screen center to the screen position, but on the entire map image
            Vector2 fromScreenCenterToPositionNonScaled = (screenPosition - screenCenterVector) / zoomScale;

            Vector2 screenPositionPixelXY = ScreenCenterPixelXY + fromScreenCenterToPositionNonScaled;

            return TileSystem.PixelXYToLatLong(screenPositionPixelXY, ZoomLevel);
        }

        /// <summary>
        /// Convert a geo-coordinate to a coordinate on the device's display.
        /// </summary>
        /// <param name="location">Geo-coordinate to convert.</param>
        /// <returns>Screen position which represents the geo-coordinate.</returns>
        public Vector2 ConvertGeoCoordinateToScreenPosition(GeoCoordinate location)
        {
            Vector2 locationPixelXY = TileSystem.LatLongToPixelXY(location, ZoomLevel);

            Vector2 pixelXYCenterToLocationScaled = (locationPixelXY - ScreenCenterPixelXY) * zoomScale;

            // Get vector to the screen center (remember that tiles are the size of the screen, ignoring zoom)
            Vector2 vectorToScreenCenter = screenCenterVector;

            return vectorToScreenCenter + pixelXYCenterToLocationScaled;
        }

        #endregion
    }
}
