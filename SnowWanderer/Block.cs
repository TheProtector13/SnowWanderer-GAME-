using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class Block : IGraphicObject {
        private protected readonly Texture2D[] textures;
        private protected Int16 callCount = 0;
        public Point Location { get; set; } = Point.Zero;
        public virtual Point Size { get; set; } = new(32, 58);
        public static Point DefSize { get; } = new(32, 58);
        /// <summary>
        /// X = Top, Y = Bottom
        /// </summary>
        public static Point DefMargin { get; } = new(10, 16);
        public bool IsAnimated { get; init; }
        /// <summary>
        /// Frame changing interval. Default is 15.
        /// </summary>
        public virtual Int16 FrameRate { get; set; } = 25;
        public Int16 CurrentFrame { get; private protected set; } = 0;
        public Int16 ID { get; init; } = 0;

        public Block(Texture2D[] textures)
        {
            IsAnimated = true;
            this.textures = textures;
        }

        public Block(Texture2D texture)
        {
            IsAnimated = false;
            this.textures = [texture];
        }

        public Block(Texture2D[] textures, Int16 id)
        {
            IsAnimated = true;
            this.textures = textures;
            this.ID = id;
        }

        public Block(Texture2D texture, Int16 id)
        {
            IsAnimated = false;
            this.textures = [texture];
            this.ID = id;
        }

        /// <summary>
        /// Returns the current frame of the animation,
        /// and increments the frame counter. (IF the input is true! By default false.)
        /// </summary>
        public Texture2D GetTexture(bool increment = false)
        {
            if (IsAnimated) {
                if (increment) {
                    callCount++;
                    if (callCount > FrameRate) {
                        callCount = 0;
                        CurrentFrame++;
                        if (CurrentFrame >= textures.Length) {
                            CurrentFrame = 0;
                        }
                    }
                }
                return textures[CurrentFrame];
            }
            return textures[0];
        }

        /// <summary>
        /// Draws the block, with the correct frame of the animation.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsAnimated) {
                spriteBatch.Draw(textures[CurrentFrame], new Rectangle(Location, Size), Color.White);
            }
            else {
                spriteBatch.Draw(textures[0], new Rectangle(Location, Size), Color.White);
            }
        }

        public virtual void Update()
        {
            if (IsAnimated) {
                callCount++;
                if (callCount > FrameRate) {
                    callCount = 0;
                    CurrentFrame++;
                    if (CurrentFrame >= textures.Length) {
                        CurrentFrame = 0;
                    }
                }
            }
        }
    }
}
