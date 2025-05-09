using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnowWanderer {
    internal class NumericInputBox : IGraphicObject {
        // Statikus alapértelmezett értékek
        public static Point DefSize { get; } = new Point(48, 48);
        public static Point DefMargin { get; } = Point.Zero;

        // Rajzoláshoz szükséges adatok
        private readonly SpriteFont font;
        private readonly Texture2D backgroundTexture;
        private string inputText;

        // Számként értelmezhető értékek
        public short Value { get; private set; }
        private short previousValidValue;

        // IGraphicObject tulajdonságok
        public short ID { get; init; }
        public Point Location { get; set; } = Point.Zero;
        public Point Size { get; set; }  // A texture mérete alapján kerül beállításra a konstruktorban

        // Egyéb property-k
        public bool IsCentered { get; set; } = true;
        public Color PenColour { get; set; } = Color.White;
        public bool Enabled { get; set; } = true;

        // Fókusz: csak az osztály módosíthatja
        public bool IsFocused { get; private set; } = false;

        // Globális rajzolási változók (Update-ben kalkulált értékek)
        private Vector2 texturePosition = Vector2.Zero;
        private Vector2 textPosition = Vector2.Zero;
        private string displayText = "";

        // Kurzor villogásához
        private double cursorBlinkTimer;
        private bool showCursor;
        private DateTime lastUpdateTime;

        // Billentyűzet és egér állapotának követése
        private KeyboardState previousKeyboardState;
        private MouseState previousMouseState;

        /// <summary>
        /// Konstruktor, ahol megadható az ID.
        /// A Size a backgroundTexture mérete alapján kerül beállításra.
        /// </summary>
        /// <param name="font">SpriteFont a szöveg rajzolásához</param>
        /// <param name="backgroundTexture">Háttérként használt texture</param>
        /// <param name="id">Az objektum azonosítója</param>
        public NumericInputBox(SpriteFont font, Texture2D backgroundTexture, short id)
        {
            this.font = font;
            this.backgroundTexture = backgroundTexture;
            this.ID = id;
            this.Size = new Point(backgroundTexture.Width, backgroundTexture.Height);
            inputText = "1";
            Value = 1;
            previousValidValue = 1;

            lastUpdateTime = DateTime.Now;
            cursorBlinkTimer = 0;
            showCursor = true;
            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Konstruktor ID nélkül, ekkor az alapértelmezett ID 0.
        /// </summary>
        public NumericInputBox(SpriteFont font, Texture2D backgroundTexture)
            : this(font, backgroundTexture, 0)
        { }

        /// <summary>
        /// Visszaadja a háttérként használt texture-ot.
        /// </summary>
        public Texture2D GetTexture(bool _ = false)
        {
            return backgroundTexture ?? throw new NotImplementedException("Background texture is not available.");
        }

        /// <summary>
        /// Validálja a beviteli szöveget: az érték nem lehet 1-nél kisebb,
        /// illetve nem lehet nagyobb a short típus maximumánál.
        /// </summary>
        public void ValidateInput()
        {
            if (int.TryParse(inputText, out int result)) {
                if (result < 1)
                    result = 1;
                else if (result > short.MaxValue)
                    result = short.MaxValue;

                Value = (short)result;
                previousValidValue = Value;
            }
            else {
                inputText = previousValidValue.ToString();
            }
        }

        /// <summary>
        /// Az Update metódus:
        /// - Ellenőrzi az egér pozícióját, és ha az input mező fölött van, kiemeli a hátteret.
        /// - Egy kattintás fókuszt ad, vagy fókuszt vesz el (ha már fókuszban van).
        /// - Ha fókuszban van, kezeli a billentyűzet bemenetét.
        /// - Számolja a rajzolási koordinátákat és a displayText-et.
        /// </summary>
        public void Update()
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - lastUpdateTime).TotalSeconds;
            lastUpdateTime = now;

            // Számoljuk ki a rajzolási pozíciót
            texturePosition = IsCentered
                ? new Vector2(Location.X - Size.X / 2, Location.Y - Size.Y / 2)
                : new Vector2(Location.X, Location.Y);
            if (Enabled) {
                // Határozzuk meg az input mező bounding rectangle-jét
                Rectangle inputRect = new(texturePosition.ToPoint(), Size);
                MouseState currentMouseState = Mouse.GetState();
                Point mousePos = new(currentMouseState.X, currentMouseState.Y);
                bool isHover = inputRect.Contains(mousePos);

                // Egér kattintásának kezelése (transition: released -> pressed)
                if (currentMouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released) {
                    // Ha nincs fókuszban és az egér fölé kattintanak, akkor fókuszt kap
                    if (!IsFocused && isHover) {
                        IsFocused = true;
                    }
                    // Ha fókuszban van (függetlenül attól, hogy fölötte vagy sem), akkor validálunk és elveszítjük a fókuszt
                    else if (IsFocused) {
                        ValidateInput();
                        IsFocused = false;
                    }
                }
                previousMouseState = currentMouseState;

                // Fókusz esetén billentyűzet kezelése
                if (IsFocused) {
                    cursorBlinkTimer += deltaTime;
                    if (cursorBlinkTimer >= 0.5) {
                        showCursor = !showCursor;
                        cursorBlinkTimer = 0;
                    }

                    KeyboardState currentKeyboardState = Keyboard.GetState();
                    foreach (Keys key in currentKeyboardState.GetPressedKeys()) {
                        if (!previousKeyboardState.IsKeyDown(key)) {
                            if (key == Keys.Back) {
                                if (inputText.Length > 0)
                                    inputText = inputText.Substring(0, inputText.Length - 1);
                            }
                            else if (key == Keys.Enter) {
                                ValidateInput();
                                IsFocused = false;
                            }
                            else if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9)) {
                                char digitChar = (char)((int)'0' + (key <= Keys.D9 ? key - Keys.D0 : key - Keys.NumPad0));
                                inputText += digitChar;
                            }
                        }
                    }
                    previousKeyboardState = Keyboard.GetState();
                }
                else {
                    // Ha nincs fókuszban, a kurzor nem villog
                    showCursor = false;
                }
            }
            else {
                if (IsFocused) {
                    ValidateInput();
                    IsFocused = false;
                }
            }

            // Számoljuk ki a megjelenítendő szöveget
            displayText = inputText + (IsFocused && showCursor ? "|" : "");
            Vector2 measuredText = font.MeasureString(displayText);
            textPosition = texturePosition + new Vector2((Size.X - measuredText.X) / 2, (Size.Y - measuredText.Y) / 2);
        }

        /// <summary>
        /// A Draw metódus:
        /// - A háttér texture-t rajzolja, ha az egér fölött van (és nem fókuszban), akkor kiemelve (például halványabb árnyalattal).
        /// - A szöveget a displayText alapján rajzolja.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Ha az egér fölött van és nincs fókuszban, alkalmazzunk egy halványbb árnyalatot
            Color bgColor = (!IsFocused && new Rectangle(texturePosition.ToPoint(), Size).Contains(Mouse.GetState().Position))
                ? Color.Gray
                : Color.White;

            spriteBatch.Draw(backgroundTexture, new Rectangle(texturePosition.ToPoint(), Size), bgColor);
            spriteBatch.DrawString(font, displayText, textPosition, PenColour);
        }
    }
}
