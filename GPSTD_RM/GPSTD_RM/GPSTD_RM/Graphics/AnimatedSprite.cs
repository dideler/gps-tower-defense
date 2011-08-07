using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
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
    /// A single frame of the spritely sprites.
    /// </summary>
    public struct SpriteFrame
    {
        public Texture2D texture;
        public TimeSpan duration;

        public SpriteFrame(Texture2D tex, TimeSpan dur)
        {
            texture = tex;
            duration = dur;
        }

    }

    /// <summary>
    /// Any animations are stored as loops.  Loops advance until the end, and then return to a specified point.
    /// </summary>
    public class AnimationLoop
    {
        /// <summary>
        /// The name identifying the current animation loop.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An array of textures defining the animation.
        /// </summary>
        public SpriteFrame[] Frames { get; set; }

        /// <summary>
        /// The index where the animation resumes after the
        /// loop has finished.
        /// </summary>
        public int ReturnIndex { get; set; }


        /// <summary>
        /// Initialize the loop!
        /// </summary>
        /// <param name="name">What is the loop called?</param>
        /// <param name="frames">The list of frames in the loop.</param>
        /// <param name="rIdx">The return index.</param>
        public AnimationLoop(string name, SpriteFrame[] frames, int rIdx)
        {

            Frames = frames;

            Name = name;
            ReturnIndex = rIdx;
        }

        /// <summary>
        /// Get the next frame based on the current frame.
        /// </summary>
        /// <param name="currentFrame">The current position in the list.</param>
        /// <returns>The next frame in the sequence.</returns>
        public int NextFrame(int currentFrame)
        {
            if (currentFrame == Frames.Length - 1)
            {
                return ReturnIndex;
            }

            return currentFrame + 1;
        }
    }

    /// <summary>
    /// This class represents a set of animation loops.
    /// </summary>
    public class AnimatedSprite
    {

        /// <summary>
        /// The animation loops present in this sprite.
        /// </summary>
        public AnimationLoop[] Loops { get; set; }


        /// <summary>
        /// Create a sprite with an array of loops.
        /// </summary>
        /// <param name="loops">An array of loops.</param>
        public AnimatedSprite(AnimationLoop[] loops)
        {
            Loops = loops;
        }

        /// <summary>
        /// Returns the animation loop that matches up with the loop name.
        /// </summary>
        /// <param name="loopName">The loop you're looking for.</param>
        /// <returns>The loop that corresponds to the string.</returns>
        public AnimationLoop GetLoop(string loopName)
        {
            foreach (AnimationLoop l in Loops)
            {
                if (l.Name.Equals(loopName))
                {
                    return l;
                }
            }

            return null;
        }

        #region Reading from files

        /// <summary>
        /// Read a file that specifies each of the animation loops.
        /// </summary>
        /// <param name="filename">The file specifying the sprite.</param>
        /// <returns>A sprite, or null if there was a problem reading the file.</returns>
        public static AnimatedSprite SpriteFromFile(string filename, GraphicsDevice gDev)
        {

            XmlReader reader;
            //FileStream inFile;

            //try
            //{
            //inFile = new FileStream(filename, FileMode.Open);
            //}
            //catch (Exception)
            //{
            //    return null;
            //}

            //reader = XmlReader.Create(inFile);
            reader = XmlReader.Create(filename);
            
            AnimatedSprite ret = null;
            string tempName = null;
            int tempRidx = 0;
            List<AnimationLoop> loopList = null;
            List<SpriteFrame> frames = null;

            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Sprite should be the first elment encountered
                        if (reader.Name.Equals("sprite", StringComparison.OrdinalIgnoreCase))
                        {
                            loopList = new List<AnimationLoop>();
                        }
                        // Next is loop
                        if (reader.Name.Equals("loop", StringComparison.OrdinalIgnoreCase))
                        {
                            if (loopList == null)
                            {
                                throw new XmlException("Improperly formatted sprite file : "+filename);
                            }

                            frames = new List<SpriteFrame>();

                            // Attempt to read the name
                            if (reader.MoveToAttribute("name"))
                            {
                                tempName = reader.Value;
                            }
                            else
                            {
                                throw new XmlException("Loop specification needs name attribute.");
                            }

                            // Attempt to read the return index
                            if (reader.MoveToAttribute("retidx"))
                            {
                                tempRidx = int.Parse(reader.Value);
                            }
                            else
                            {
                                throw new XmlException("Loop specification needs retidx attribute.");
                            }
                        }
                        // Last is frame
                        if (reader.Name.Equals("frame", StringComparison.OrdinalIgnoreCase))
                        {

                            if (frames == null)
                            {
                                throw new XmlException("Improperly formatted sprite file : "+filename);
                            }

                            Texture2D tempTex = null;
                            TimeSpan ts;

                            // Attempt to read and load the texture
                            if (reader.MoveToAttribute("texture"))
                            {
                                // Stream the texture
                                tempTex = Helper.LoadTextureStream(Path.GetDirectoryName(filename) + "\\" + reader.Value, gDev);
                            }
                            else
                            {
                                throw new XmlException("Frame specification needs texture attribute.");
                            }

                            // Attempt to read the duration
                            if (reader.MoveToAttribute("duration"))
                            {
                                // Get the time span
                                ts = TimeSpan.FromSeconds(float.Parse(reader.Value));
                            }
                            else
                            {
                                throw new XmlException("Frame specification needs duration attribute.");
                            }

                            // Construct the frame and put it on the list
                            frames.Add(new SpriteFrame(tempTex, ts));
                        }
                        break;

                    case XmlNodeType.EndElement:
                        // When each sprite finishes, construct a sprite based on the loops
                        if (reader.Name.Equals("sprite", StringComparison.OrdinalIgnoreCase))
                        {
                            ret = new AnimatedSprite(loopList.ToArray());
                            loopList.Clear();

                            // Don't need this open anymore
                            reader.Close();
                        }
                        // When each loop finishes, use the frame list to construct a loop and put it on the list
                        if (reader.Name.Equals("loop", StringComparison.OrdinalIgnoreCase))
                        {
                            loopList.Add(new AnimationLoop(tempName, frames.ToArray(), tempRidx));
                            tempName = null;
                            frames = null;
                            tempRidx = 0;
                        }
                        break;
                }
            }


            return ret;
        }
        #endregion
    }

        

    /// <summary>
    /// Maintains a reference to a sprite, frame number, and animation loop.
    /// </summary>
    public class AnimatedSpriteInstance
    {
        /// <summary>
        /// The sprite which this instance is attached to.
        /// </summary>
        public AnimatedSprite Sprite { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AnimationLoop Loop { get; set; }

        public int FrameNumber { get; set; }

        /// <summary>
        /// Time that has passed since the last frame switch.
        /// </summary>
        private TimeSpan sinceLastSwitch;

        /// <summary>
        /// Create a new sprite instance with a sprite and a default loop name.
        /// </summary>
        /// <param name="sourceSprite">The sprite to which this is attached.</param>
        /// <param name="defaultLoop">Default animation loop for this instance.</param>
        public AnimatedSpriteInstance(AnimatedSprite sourceSprite, string defaultLoop)
        {
            Sprite = sourceSprite;
            SwitchLoop(defaultLoop);
            sinceLastSwitch = TimeSpan.Zero;
        }

        /// <summary>
        /// Get the current texture, based on the animations loops.
        /// </summary>
        /// <returns>The currently active texture in the animation.  Null if there's a problem.</returns>
        public Texture2D CurrentTexture()
        {
            try
            {
                return Loop.Frames[FrameNumber].texture;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// Switch to a new animation loop.  Reset the frame number to 
        /// zero.
        /// </summary>
        /// <param name="loopName">The name of the new loop.</param>
        public void SwitchLoop(string loopName)
        {
            Loop = Sprite.GetLoop(loopName);
            FrameNumber = 0;
        }

        /// <summary>
        /// Called every update cycle to advance the frame when necessary.
        /// </summary>
        /// <param name="time">The current game time.</param>
        public void Update(GameTime time)
        {
            // Add the time difference
            sinceLastSwitch += time.ElapsedGameTime;

            // Check to see if we're over the time diff
            if (sinceLastSwitch >= Loop.Frames[FrameNumber].duration)
            {
                // If so, switch frames
                FrameNumber = Loop.NextFrame(FrameNumber);

                // Reset the timer
                // Make sure the excess is included to keep the frame size as accurate as possible.
                sinceLastSwitch -= Loop.Frames[FrameNumber].duration;
            }
        }
    }
}
