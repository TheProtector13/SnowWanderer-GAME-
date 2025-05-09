using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class TextBox : IGraphicObject {
        private readonly SpriteFont font;
        private string text = string.Empty;
        private string[] lines = [string.Empty];
        private Vector2[] textLocation = [Vector2.Zero];
        public Point Location { get; set; } = Point.Zero;
        public string Text
        {
            get => text;
            set {
                text = value;
                lines = text.Split('\n');
                textLocation = new Vector2[lines.Length];
                Array.Fill(textLocation, Vector2.Zero);
                string longest = lines.OrderByDescending(s => s.Length).First();
                Size = new Point((Int32)font.MeasureString(longest).X, (Int32)(font.MeasureString(longest).Y * (lines.Length - 1) * 1.1f + font.MeasureString(longest).Y));
            }
        }
        public Color PenColour { get; set; } = Color.Black;
        public bool IsCentered { get; set; } = true;
        public Int16 ID { get; init; } = 0;
        public Point Size { get; private set; } = Point.Zero;
        public static Point DefSize { get; } = Point.Zero;
        public static Point DefMargin { get; } = Point.Zero;

        public TextBox(SpriteFont font, string text, Int16 id)
        {
            this.font = font;
            this.Text = text;
            this.ID = id;
        }

        public TextBox(SpriteFont font, Int16 id)
        {
            this.font = font;
            this.ID = id;
        }

        public TextBox(SpriteFont font, string text)
        {
            this.font = font;
            this.Text = text;
        }

        public TextBox(SpriteFont font)
        {
            this.font = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(text)) {
                for (Int32 i = 0; i < textLocation.Length; i++) {
                    spriteBatch.DrawString(font, lines[i], textLocation[i], PenColour);
                }
            }
        }

        public void Update()
        {
            if (!string.IsNullOrEmpty(text)) {
                if (IsCentered) {
                    for (Int32 i = 0; i < textLocation.Length; i++) {
                        float x = Location.X - (font.MeasureString(lines[i]).X / 2);
                        float y = Location.Y - (font.MeasureString(lines[i]).Y / 2) + i * (font.MeasureString(lines[i]).Y * 1.1f);
                        textLocation[i] = new Vector2(x, y);
                    }
                }
                else {
                    for (Int32 i = 0; i < textLocation.Length; i++) {
                        textLocation[i] = new Vector2(Location.X, Location.Y - (font.MeasureString(lines[i]).Y / 2) + i * (font.MeasureString(lines[i]).Y * 1.1f));
                    }
                }
            }
        }

        public Texture2D GetTexture(bool _ = false)
        {
            throw new System.NotImplementedException("TextBox has no texture!");
        }
    }
}
