using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class Objective : Block {
        private Point realLocation = Point.Zero;
        public Point Margin { get; set; } = Point.Zero;
        public override Point Size { get; set; } = new(32, 32);
        public static new Point DefSize { get; } = new(32, 32);
        public static new Point DefMargin { get; } = Point.Zero;
        public override Int16 FrameRate { get; set; } = 5;

        public Objective(Texture2D[] textures) : base(textures) { }
        public Objective(Texture2D texture) : base(texture) { }
        public Objective(Texture2D[] textures, Int16 id) : base(textures, id) { }
        public Objective(Texture2D texture, Int16 id) : base(texture, id) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures[CurrentFrame], new Rectangle(realLocation, Size), Color.White);
        }

        public override void Update()
        {
            realLocation = new(Location.X - Size.X / 2 + Margin.X, Location.Y - Size.Y + Margin.Y);
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
