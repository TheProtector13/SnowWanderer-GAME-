using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class Command : IGraphicObject {
        private readonly Texture2D[] textures;
        private readonly SpriteFont font;
        private readonly Button[] buttons = new Button[3];
        private readonly Point[] defButtonSize = new Point[3];
        private Vector2 textLocation = Vector2.Zero;
        private Point defInputSize;
        private Point size = new(192, 48);
        private Point framesize = new(192, 16);
        private Point defFramesize = new(192, 16);
        private Int32 frameHeight = 72;
        private readonly Point[] frameLoc = [Point.Zero, Point.Zero];

        public ControlPanel? SubCommands { get; private set; } = null;
        public IGraphicObject InputBox { get; init; }
        public Point Location { get; set; } = Point.Zero;
        public Point Size
        {
            get => size;
            set {
                size = value;
                double percent = (double)size.X / (double)DefSize.X;
                if (InputBox is NumericInputBox) {
                    ((NumericInputBox)InputBox).Size = new Point((int)(defInputSize.X * percent), (int)(defInputSize.Y * percent));
                }
                else {
                    ((DropdownSelector)InputBox).Size = new Point((int)(defInputSize.X * percent), (int)(defInputSize.Y * percent));
                }
                for (int i = 0; i < buttons.Length; i++) {
                    if (buttons[i] != null) {
                        buttons[i].Size = new Point((int)(defButtonSize[i].X * percent), (int)(defButtonSize[i].Y * percent));
                    }
                }
                framesize = new Point((int)(defFramesize.X * percent), (int)(defFramesize.Y * percent));
                frameHeight = (Int32)(size.Y * 1.5);
                if (IsAppendable) {
                    SubCommands.Size = new(size.X - framesize.Y, (Int32)(size.Y * 0.8));
                }
            }
        }
        public static Point DefSize { get; } = new(192, 48);
        public static Point DefMargin { get; } = Point.Zero;
        public string Text { get; set; } = string.Empty;
        public Color PenColour { get; set; } = Color.Black;
        public Int16 ID { get; init; } = 0;
        public bool Enabled { get; set; } = true;
        public bool ButtonsEnabled { get; set; } = true;
        /// <summary>
        /// Meghatározza, hogy a parancshoz lehet-e még alparancsokat csatolni.
        /// </summary>
        public bool IsAppendable { get; init; }

        enum ButtonType {
            Append,
            Remove
        }

        // gombok méret fugg a defsize tol valo eltéréstől, létrejottekor állítjuk
        public Command(TextureManager textureManager, Texture2D texture, Texture2D frametexture, SpriteFont font, Point screensize, Point controlsize, Point margin, Int16 id = 0, bool IsFOR = false)
        {
            this.textures = [texture, frametexture];
            this.font = font;
            this.IsAppendable = true;
            this.buttons[0] = new Button(textureManager.AddButton, font, (short)ButtonType.Append);
            this.buttons[1] = new Button(textureManager.MenuNokButton, font, (short)ButtonType.Remove);
            this.buttons[2] = new Button(textureManager.AddButton, font, (short)ButtonType.Append);
            if (IsFOR) {
                this.InputBox = new NumericInputBox(font, textureManager.FieldBG) {
                    IsCentered = false
                };
            }
            else {
                this.InputBox = new DropdownSelector(font, textureManager.FieldBG, ["Tárgy", "Ellen.sz", "Ellenség"], [0, 1, 2]) {
                    IsCentered = false
                };
            }
            this.SubCommands = new(textureManager, font, this.buttons[2], screensize, controlsize);
            this.size = new Point(textures[0].Width, textures[0].Height);
            this.SubCommands.Size = new(size.X - framesize.Y, (Int32)(size.Y * 0.8));
            this.SubCommands.Margin = margin;
            this.defInputSize = InputBox.Size;
            buttons[0].Size = new Point(40);
            buttons[1].Size = new Point(40);
            buttons[2].Size = new Point(40);
            this.defButtonSize[0] = buttons[0].Size;
            this.defButtonSize[1] = buttons[1].Size;
            this.defButtonSize[2] = buttons[2].Size;
            this.ID = id;
        }

        public Command(TextureManager textureManager, Texture2D texture, SpriteFont font, Int16 id = 0)
        {
            this.textures = [texture];
            this.font = font;
            this.buttons[0] = new Button(textureManager.AddButton, font, (short)ButtonType.Append);
            this.buttons[1] = new Button(textureManager.MenuNokButton, font, (short)ButtonType.Remove);
            this.InputBox = new DropdownSelector(font, textureManager.FieldBG, ["Fel", "Le", "Ballra", "Jobbra", "Felvesz", "Vár"], [0, 1, 2, 3, 4, 5]) {
                IsCentered = false
            };
            this.IsAppendable = false;
            this.size = new Point(textures[0].Width, textures[0].Height);
            this.defInputSize = InputBox.Size;
            buttons[0].Size = new Point(40);
            buttons[1].Size = new Point(40);
            this.defButtonSize[0] = buttons[0].Size;
            this.defButtonSize[1] = buttons[1].Size;
            this.ID = id;
        }

        public Int32 GetSizeY()
        {
            if (IsAppendable) {
                return size.Y / 2 + frameHeight;
            }
            return size.Y;
        }

        /// <summary>
        /// Adds an event handler to a button's click event at a specified index in the buttons array.
        /// 0 is the append button, 1 is the remove button.
        /// </summary>
        /// <param name="handler">The event handler to be invoked when the button is clicked.</param>
        /// <param name="buttonindex">Specifies the index of the button in the buttons array to which the event handler is added.</param>
        public void AddEventHandler(EventHandler handler, Int32 buttonindex)
        {
            buttons[buttonindex].Click += handler;
        }

        /// <summary>
        /// Returns the specified texture of the Command.
        /// </summary>
        public Texture2D GetTexture(bool _ = false)
        {
            return textures[0];
        }

        /// <summary>
        /// Draws the Button.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAppendable) {
                buttons[2].Draw(spriteBatch);
                buttons[0].Draw(spriteBatch);
                spriteBatch.Draw(textures[1], new Rectangle(frameLoc[0], new(frameHeight, framesize.Y)), null, Color.White, MathHelper.ToRadians(90.0f), Vector2.Zero, SpriteEffects.None, 0f);
                spriteBatch.Draw(textures[1], new Rectangle(frameLoc[1], framesize), Color.White);
            }
            else {
                buttons[0].Draw(spriteBatch);
            }
            spriteBatch.Draw(textures[0], new Rectangle(Location, Size), Color.White);
            buttons[1].Draw(spriteBatch);
            if (!string.IsNullOrEmpty(Text)) {
                spriteBatch.DrawString(font, Text, textLocation, PenColour);
            }
            if (IsAppendable) {
                SubCommands.Draw(spriteBatch);
            }
            InputBox.Draw(spriteBatch);
        }

        public void Update()
        {
            if (!string.IsNullOrEmpty(Text)) {
                float x = Location.X + 5;
                float y = (Location.Y + (size.Y / 2)) - (font.MeasureString(Text).Y / 2);
                textLocation = new Vector2(x, y);
            }
            InputBox.Location = new Point(Location.X + size.X - 10 - InputBox.Size.X - buttons[1].Size.X, Location.Y + size.Y / 2 - InputBox.Size.Y / 2);
            if (InputBox is NumericInputBox) {
                ((NumericInputBox)InputBox).Enabled = Enabled;
            }
            else {
                ((DropdownSelector)InputBox).Enabled = Enabled;
            }
            InputBox.Update();
            buttons[1].Location = new Point(InputBox.Location.X + InputBox.Size.X + 5, Location.Y + size.Y / 2 - buttons[1].Size.Y / 2);
            for (int i = 0; i < buttons.Length - 1; i++) {
                buttons[i].Enabled = Enabled && ButtonsEnabled;
                buttons[i].Update();
            }
            if (IsAppendable) {
                SubCommands.Location = new Point(Location.X, Location.Y + size.Y);
                frameHeight = (Int32)(size.Y * 1.5) + SubCommands.GetAbsSizeY();
                frameLoc[0] = new Point(Location.X + size.X, Location.Y + size.Y / 2);
                frameLoc[1] = new Point(Location.X, Location.Y + size.Y / 2 + frameHeight - framesize.Y);
                buttons[0].Location = new Point(Location.X + size.X - 15 - buttons[0].Size.X, Location.Y + size.Y / 2 + frameHeight - framesize.Y);
                buttons[2].Location = new Point(Location.X + size.X - 15 - buttons[0].Size.X, Location.Y + size.Y - buttons[0].Size.Y / 2);
                SubCommands.Update();
                buttons[2].Update();
            }
            else {
                buttons[0].Location = new Point(Location.X + size.X - 15 - buttons[0].Size.X, Location.Y + size.Y - buttons[0].Size.Y / 2);
            }
        }
    }
}
