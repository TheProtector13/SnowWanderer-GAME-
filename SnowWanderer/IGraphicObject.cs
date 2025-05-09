using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal interface IGraphicObject : IDrawable {
        /// <summary>
        /// Identifier of the object. Default is 0. Only can be set in contructor.
        /// </summary>
        public Int16 ID { get; init; }
        /// <summary>
        /// Location of the object. Left top corner.
        /// If the object has IsCentered boolean property and it is set to true, then this is the center of the object.
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// Size of the object.
        /// X = Width, Y = Height
        /// </summary>
        public Point Size { get; }
        /// <summary>
        /// Default size, if exists.
        /// X = Width, Y = Height
        /// </summary>
        public static Point DefSize { get; }
        /// <summary>
        /// Default margin, if exists. Else its Point.Zero.
        /// X = Top, Y = Bottom
        /// </summary>
        public static Point DefMargin { get; }
        /// <summary>
        /// Returns the current frame of the animation, if exists.
        /// The input boolean value is generic, most commonly can be used for incrementing the frame counter.
        /// If the object dont have a texture, then throws System.NotImplementedException.
        /// </summary>
        public Texture2D GetTexture(bool _ = false);
    }
}
