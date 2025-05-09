using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnowWanderer {
    internal class Slider : IGraphicObject {
        private readonly Texture2D textureSlider;
        private readonly Texture2D textureBall;
        private MouseState mouseState;
        private MouseState prevMouseState;
        private bool isHovered;
        private bool isDragging;
        private Point ballLocation = Point.Zero;
        private Point size = new(192, 32);
        private Point ballSize;

        public Point Location { get; set; } = Point.Zero;

        // A méret beállítása automatikusan frissíti a ballSize-ot.
        public Point Size
        {
            get => size;
            set {
                size = value;
                ballSize = new Point(value.Y + 10);
            }
        }
        public static Point DefSize { get; } = new(192, 32);
        public static Point DefMargin { get; } = new(16);
        public event EventHandler? OnChange;
        public float Value { get; set; } = 1.0f;
        public short ID { get; init; } = 0;
        public bool IsVertical { get; set; } = false;

        // Konstruktor, amely közös beállításokat tartalmaz.
        public Slider(TextureManager manager)
        {
            textureSlider = manager.SliderBar;
            textureBall = manager.SliderBall;
            Size = new Point(textureSlider.Width, textureSlider.Height);
        }

        // Második konstruktor, amely az elsőt hívja meg és beállítja az ID-t.
        public Slider(TextureManager manager, short id) : this(manager)
        {
            ID = id;
        }

        /// <summary>
        /// Visszaadja a megfelelő textúrát a Slider számára.
        /// </summary>
        public Texture2D GetTexture(bool useBall = false)
        {
            return useBall ? textureBall : textureSlider;
        }

        /// <summary>
        /// Kirajzolja a csúszkát.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVertical) {
                // A slider bar 90 fokos elforgatása, ha függőleges.
                spriteBatch.Draw(
                    textureSlider,
                    new Rectangle(new(Location.X + size.Y, Location.Y), size),
                    null,
                    Color.White,
                    MathHelper.ToRadians(90.0f),
                    Vector2.Zero,
                    SpriteEffects.None,
                    0f
                );
            }
            else {
                spriteBatch.Draw(textureSlider, new Rectangle(Location, size), Color.White);
            }

            Color ballColor = isHovered ? Color.DarkCyan : Color.White;
            spriteBatch.Draw(textureBall, new Rectangle(ballLocation, ballSize), ballColor);
        }

        /// <summary>
        /// Frissíti a csúszka állapotát.
        /// </summary>
        public void Update()
        {
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (isDragging) {
                int half = ballSize.X / 2;
                int newCoord;
                if (IsVertical) {
                    // Az Y koordinátát clamp-ezzuk a megfelelő tartományban.
                    newCoord = Math.Clamp(mouseState.Position.Y - half, Location.Y, Location.Y + Size.X - ballSize.X);
                    ballLocation = new Point(Location.X - 5, newCoord);
                    Value = (float)(newCoord - Location.Y) / (Size.X - ballSize.X);
                }
                else {
                    // Az X koordinátát clamp-ezzuk a megfelelő tartományban.
                    newCoord = Math.Clamp(mouseState.Position.X - half, Location.X, Location.X + Size.X - ballSize.X);
                    ballLocation = new Point(newCoord, Location.Y - 5);
                    Value = (float)(newCoord - Location.X) / (Size.X - ballSize.X);
                }
                if (mouseState.LeftButton == ButtonState.Released) {
                    isDragging = false;
                    OnChange?.Invoke(this, EventArgs.Empty);
                }
            }
            else {
                // Hover detektálás.
                Rectangle ballRect = new(ballLocation, ballSize);
                isHovered = ballRect.Contains(mouseState.Position);

                // Ha rákattintottunk a csúszka elemre, elindítjuk a drag módot.
                if (isHovered && mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {
                    isDragging = true;
                }

                // A csúszka aktuális pozíciójának frissítése az érték alapján.
                if (IsVertical) {
                    int newY = (int)(Location.Y + Value * (Size.X - ballSize.X));
                    ballLocation = new Point(Location.X - 5, newY);
                }
                else {
                    int newX = (int)(Location.X + Value * (Size.X - ballSize.X));
                    ballLocation = new Point(newX, Location.Y - 5);
                }
            }
        }
    }
}
