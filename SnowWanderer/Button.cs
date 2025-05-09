using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#nullable enable

namespace SnowWanderer {
    internal class Button : IGraphicObject {
        private readonly Texture2D[] textures;
        private readonly SpriteFont font;
        private MouseState mouseState = Mouse.GetState();
        private MouseState prevMouseState;
        private bool IsHovered = false;
        private Vector2 textLocation = Vector2.Zero;
        public Point Location { get; set; } = Point.Zero;
        public Point Size { get; set; } = new(128, 128);
        public static Point DefSize { get; } = new(128, 128);
        public static Point DefMargin { get; } = Point.Zero;
        public event EventHandler? Click;
        public bool IsClicked { get; private set; } = false;
        public bool Enabled { get; set; } = true;
        public string Text { get; set; } = string.Empty;
        public Color PenColour { get; set; } = Color.Black;
        public Int16 ID { get; init; } = 0;

        public Button(Texture2D[] textures, SpriteFont font)
        {
            this.textures = textures;
            this.font = font;
            this.Size = new Point(textures[0].Width, textures[0].Height);
        }

        public Button(Texture2D[] textures, SpriteFont font, Int16 id)
        {
            this.textures = textures;
            this.font = font;
            this.Size = new Point(textures[0].Width, textures[0].Height);
            this.ID = id;
        }

        /// <summary>
        /// Returns the specified texture of the Button.
        /// </summary>
        public Texture2D GetTexture(bool hovered = false)
        {
            if (hovered) {
                return textures[1];
            }
            return textures[0];
        }

        /// <summary>
        /// Draws the Button.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsHovered) {
                spriteBatch.Draw(textures[1], new Rectangle(Location, Size), Color.White);
            }
            else {
                spriteBatch.Draw(textures[0], new Rectangle(Location, Size), Color.White);
            }

            if (!string.IsNullOrEmpty(Text)) {
                spriteBatch.DrawString(font, Text, textLocation, PenColour);
            }
        }

        public void Update()
        {
            if (!string.IsNullOrEmpty(Text)) {
                float x = (Location.X + (Size.X / 2)) - (font.MeasureString(Text).X / 2);
                float y = (Location.Y + (Size.Y / 2)) - (font.MeasureString(Text).Y / 2);
                textLocation = new Vector2(x, y);
            }
            if (Enabled) {
                prevMouseState = mouseState;
                mouseState = Mouse.GetState();
                Rectangle bounds = new(Location, Size);
                if (bounds.Contains(mouseState.Position)) {
                    IsHovered = true;
                    if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {
                        Click?.Invoke(this, EventArgs.Empty);
                        IsClicked = true;
                    }
                }
                else {
                    IsHovered = false;
                }
            }
            else {
                IsHovered = false;
            }
        }
    }
}
