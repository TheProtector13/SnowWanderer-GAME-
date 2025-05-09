using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SnowWanderer {
    /// <summary>
    /// DropdownSelector egy grafikus legördülő menü, amely lehetővé teszi az opciók kiválasztását egérrel vagy billentyűzettel.
    /// </summary>
    internal class DropdownSelector : IGraphicObject {
        public static Point DefSize { get; } = new Point(80, 32); // Alapértelmezett méret
        public static Point DefMargin { get; } = Point.Zero; // Alapértelmezett margó

        private readonly SpriteFont font; // Betűtípus a szöveghez
        private readonly Texture2D backgroundTexture; // Háttér textúra

        private readonly string[] displayOptions; // Megjelenítendő szövegek
        private readonly short[] valueOptions; // A szövegekhez tartozó értékek

        public short Value { get; private set; } // Jelenleg kiválasztott érték

        public short ID { get; init; }
        public Point Location { get; set; } = Point.Zero; // Elhelyezkedés a képernyőn
        public Point Size { get; set; } // Dropdown doboz mérete

        public bool IsCentered { get; set; } = true; // Középre van-e igazítva
        public Color PenColour { get; set; } = Color.White; // Szöveg színe
        public bool Enabled { get; set; } = true; // Aktív-e a mező
        public bool IsFocused { get; private set; } = false; // Jelenleg fókuszban van-e

        private int selectedIndex = 0; // Kiválasztott elem indexe
        private int hoveredIndex = -1; // Egérrel kijelölt index
        private bool isDropdownOpen = false; // Nyitva van-e a legördülő lista
        private Rectangle boundingBox; // A fő mező téglalapja

        private Vector2 basePosition; // Előre kiszámított pozíció kirajzoláshoz (globális, ne frissüljön Draw-ban)

        private KeyboardState previousKeyboardState, currentKeyboardState = Keyboard.GetState();
        private MouseState previousMouseState, currentMouseState = Mouse.GetState();

        /// <summary>
        /// Inicializálja az új DropdownSelector példányt.
        /// </summary>
        /// <param name="font">A megjelenítendő szöveg betűtípusa.</param>
        /// <param name="backgroundTexture">Háttér textúra a mezőhöz.</param>
        /// <param name="labels">Megjelenítendő opciók nevei.</param>
        /// <param name="values">A címkékhez tartozó értékek.</param>
        /// <param name="id">Azonosító (opcionális).</param>
        public DropdownSelector(SpriteFont font, Texture2D backgroundTexture, string[] labels, short[] values, short id = 0)
        {
            this.font = font;
            this.backgroundTexture = backgroundTexture;
            displayOptions = labels;
            valueOptions = values;
            ID = id;
            Size = DefSize;
            Value = values[0]; // Alapértelmezett érték beállítása

            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Visszaadja a háttér textúrát.
        /// </summary>
        public Texture2D GetTexture(bool _ = false) => backgroundTexture;

        /// <summary>
        /// Frissíti a menü állapotát (pozíció, egér és billentyűk eseményei alapján).
        /// </summary>
        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            basePosition = IsCentered
                ? new Vector2(Location.X - Size.X / 2, Location.Y - Size.Y / 2)
                : new Vector2(Location.X, Location.Y);

            boundingBox = new Rectangle(basePosition.ToPoint(), Size);

            currentMouseState = Mouse.GetState();
            currentKeyboardState = Keyboard.GetState();
            Point mousePos = new(currentMouseState.X, currentMouseState.Y);

            bool mouseOverMain = boundingBox.Contains(mousePos);
            bool clicked = currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;

            Rectangle optionArea = new(boundingBox.X, boundingBox.Y + Size.Y, Size.X, Size.Y * displayOptions.Length);

            if (clicked && Enabled) {
                if (mouseOverMain) {
                    IsFocused = true;
                    isDropdownOpen = !isDropdownOpen;
                }
                else if (isDropdownOpen && optionArea.Contains(mousePos)) {
                    int newHoverIndex = (mousePos.Y - optionArea.Y) / Size.Y;
                    if (newHoverIndex >= 0 && newHoverIndex < displayOptions.Length) {
                        selectedIndex = newHoverIndex;
                        Value = valueOptions[selectedIndex];
                    }
                    IsFocused = false;
                    isDropdownOpen = false;
                    hoveredIndex = -1;
                }
                else {
                    IsFocused = false;
                    isDropdownOpen = false;
                    hoveredIndex = -1;
                }
            }

            if (IsFocused && Enabled) {
                if (isDropdownOpen) {
                    if (optionArea.Contains(mousePos)) {
                        int newHoverIndex = (mousePos.Y - optionArea.Y) / Size.Y;
                        if (newHoverIndex >= 0 && newHoverIndex < displayOptions.Length) {
                            hoveredIndex = newHoverIndex;
                        }
                    }
                    else {
                        hoveredIndex = selectedIndex;
                    }
                }

                // Esc bezárja a menüt
                if (currentKeyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape)) {
                    hoveredIndex = -1;
                    isDropdownOpen = false;
                    IsFocused = false;
                }

                // Le nyíl
                if (currentKeyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down)) {
                    hoveredIndex = (hoveredIndex + 1) % displayOptions.Length;
                }
                // Fel nyíl
                else if (currentKeyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up)) {
                    hoveredIndex = (hoveredIndex - 1 + displayOptions.Length) % displayOptions.Length;
                }

                // Enter kiválasztás
                if (currentKeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter)) {
                    if (hoveredIndex != -1) {
                        selectedIndex = hoveredIndex;
                        Value = valueOptions[selectedIndex];
                        isDropdownOpen = false;
                        IsFocused = false;
                    }
                }
            }
        }


        /// <summary>
        /// Kirajzolja a legördülő mezőt és annak opcióit, ha nyitva van.
        /// </summary>
        /// <param name="spriteBatch">A rajzoláshoz használt SpriteBatch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle drawRect = new(basePosition.ToPoint(), Size); // A fő doboz pozíciója
            spriteBatch.Draw(backgroundTexture, drawRect, Color.White); // Háttér kirajzolása
            spriteBatch.DrawString(font, displayOptions[selectedIndex], basePosition + new Vector2(4, 4), PenColour); // Aktuális érték kirajzolása

            if (IsFocused && isDropdownOpen) {
                for (int i = 0; i < displayOptions.Length; i++) {
                    Rectangle itemRect = new(drawRect.X, drawRect.Y + Size.Y * (i + 1), Size.X, Size.Y);
                    Color bgColor = (i == hoveredIndex) ? Color.Blue : Color.White; // Hover szín kiemelés
                    spriteBatch.Draw(backgroundTexture, itemRect, bgColor); // Háttér
                    spriteBatch.DrawString(font, displayOptions[i], new Vector2(itemRect.X + 4, itemRect.Y + 4), PenColour); // Szöveg
                }
            }
        }
    }
}
