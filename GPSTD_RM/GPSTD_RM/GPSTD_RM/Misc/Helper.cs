using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    /// Contains a number of miscellaneous static helper methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Shuffles any list.
        /// </summary>
        public static void Shuffle<E>(List<E> list)
        {
            Random r = new Random();

            List<E> newList = new List<E>(list.Count);

            while (list.Count > 0)
            {
                int idx = r.Next() % list.Count;
                newList.Add(list[idx]);
                list.RemoveAt(idx);
            }

            list.AddRange(newList);
        }

        #region Math

        /// <summary>
        /// Gets the angle from a to b clockwise.  Negative it counter-clockwise.
        /// </summary>
        public static float GetAngle(Vector2 a, Vector2 b)
        {
            //float ret = (float)(Math.Acos(Vector2.Dot(a, b)));

            float Dot = Vector2.Dot(a, b);
            float den = a.Length() * b.Length();
            float ret = (float)Math.Acos(Dot / den);

            // Take the cross product to determine whether the angle is positive or negative
            Vector3 crs = Vector3.Cross(new Vector3(a, 0), new Vector3(b, 0));

            ret *= Math.Sign(crs.Z);

            return ret;
        }

        public static bool Intersects(BoundingBox b, Vector2 line1, Vector2 line2)
        {
            return Intersects(new Vector2(b.Min.X, b.Min.Y), new Vector2(b.Max.X, b.Max.Y), line1, line2);
        }

        //TODO: Re-implement with standard form or point-vector form
        public static bool Intersects(Vector2 min, Vector2 max, Vector2 line1, Vector2 line2)
        {

            float minx = Math.Min(line1.X, line2.X);
            float maxx = Math.Max(line1.X, line2.X);
            float miny = Math.Min(line1.Y, line2.Y);
            float maxy = Math.Max(line1.Y, line2.Y);

            // Early-out box check
            if (min.X > maxx || max.X < minx || min.Y > maxy || max.Y < miny)
            {
                return false;
            }


            // Next, do intersection tests
            float dx = line2.X - line1.X;
            float dy = line2.Y - line1.Y;
            float slope = dy / dx;
            float slopeY = -dx / dy;
            float intercept = line1.Y - slope * line1.X;

            // Left
            if (slope * min.X + intercept >= min.Y && slope * min.X + intercept <= max.Y)
            {
                return true;
            }

            // Right
            if (slope * max.X + intercept >= min.Y && slope * max.X + intercept <= max.Y)
            {
                return true;
            }

            // Top
            if (slopeY * (intercept + min.Y) >= min.X && slopeY * (intercept + min.Y) <= max.X)
            {
                return true;
            }

            // Bottom
            if (slopeY * (intercept + max.Y) >= min.X && slopeY * (intercept + max.Y) <= max.X)
            {
                return true;
            }

            return false;
        }
        public static Vector2 intersectionPoint(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            // See http://local.wasp.uwa.edu.au/~pbourke/geometry/pointline/
            float u = ((point.X - linePoint1.X) * (linePoint2.X - linePoint1.X) + (point.Y - linePoint1.Y) * (linePoint2.Y - linePoint1.Y)) / (linePoint2 - linePoint1).LengthSquared();

            return linePoint1 + (u * (linePoint2 - linePoint1));
        }


        public static float pointLineDistanceSquared(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            // If the box doesn't contain the point, 
            /*if (Math.Abs(Helper.GetAngle(linePoint1 - point, linePoint2 - point)) < Math.PI / 4f)
            {
                return Math.Min(Vector2.DistanceSquared(point, linePoint1), Vector2.DistanceSquared(point, linePoint2));
            }*/
            if (Vector2.Dot(point - linePoint1, linePoint2 - linePoint1) < 0)
            {
                return Math.Min(Vector2.DistanceSquared(point, linePoint1), Vector2.DistanceSquared(point, linePoint2));
            }

            return (point - intersectionPoint(point, linePoint1, linePoint2)).LengthSquared();
        }

        #endregion

        #region Content Loading and Streaming

        public static Texture2D LoadTextureStream(string loc, GraphicsDevice gDev)
        {
            Texture2D file = null;
            RenderTarget2D result = null;
            using (Stream titleStream = TitleContainer.OpenStream(loc))
            {
                file = Texture2D.FromStream(gDev, titleStream);
            }
            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(gDev, file.Width, file.Height);
            gDev.SetRenderTarget(result);
            gDev.Clear(Color.Black);

            //Multiply each color by the source alpha, and write in just the color values into the final texture
            BlendState blendColor = new BlendState();
            blendColor.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;
            blendColor.AlphaDestinationBlend = Blend.Zero;
            blendColor.ColorDestinationBlend = Blend.Zero;
            blendColor.AlphaSourceBlend = Blend.SourceAlpha;
            blendColor.ColorSourceBlend = Blend.SourceAlpha;
            SpriteBatch spriteBatch = new SpriteBatch(gDev);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
            BlendState blendAlpha = new BlendState();
            blendAlpha.ColorWriteChannels = ColorWriteChannels.Alpha;
            blendAlpha.AlphaDestinationBlend = Blend.Zero;
            blendAlpha.ColorDestinationBlend = Blend.Zero;
            blendAlpha.AlphaSourceBlend = Blend.One;
            blendAlpha.ColorSourceBlend = Blend.One;
            spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            //Release the GPU back to drawing to the screen
            gDev.SetRenderTarget(null);
            return result as Texture2D;
        }

        #endregion
    }
}
