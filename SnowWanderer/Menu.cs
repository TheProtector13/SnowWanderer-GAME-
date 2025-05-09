using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    class Menu : IGraphicObject, IDisposable {
        private readonly TextureManager textureManager;
        private readonly List<IGraphicObject> controls = [];
        private readonly List<TextBox> textBoxes = [];
        private readonly SpriteFont font;
        private Texture2D? background;
        private Int32 score = 0;
        private string warnText = string.Empty;
        private float localmusicVolume = AppSettings.MusicVolume, localsfxvolume = AppSettings.SFXVolume;
        private bool localFullscreen = AppSettings.Fullscreen;

        public ButtonPressed BPressed { get; private set; } = ButtonPressed.NaN;
        public Int16 SelectedLevel { get; private set; } = 0;
        public Int16 ID { get; init; } = 0;
        public Int32 Score
        {
            get => score;
            set {
                if (Type == MenuType.Achievement) {
                    score = value;
                    textBoxes[2].Text = value.ToString();
                }
                else {
                    throw new InvalidOperationException("Score can only be set in Achievement menu!");
                }
            }
        }
        public string WarnText
        {
            get => warnText;
            set {
                if (Type == MenuType.Warning || Type == MenuType.Warning2) {
                    warnText = value;
                    textBoxes[1].Text = value;
                }
                else {
                    throw new InvalidOperationException("WarnText can only be set in Warning menu!");
                }
            }
        }
        public MenuType Type { get; init; }
        public Point Location { get; set; } = Point.Zero;
        public Point Size { get; set; } = new(640, 650);
        public static Point DefSize { get; } = new(640, 650);
        public static Point DefMargin { get; } = new(64, 98);

        public enum MenuType {
            MainMenu,
            LevelSelect,
            Options,
            Achievement,
            Warning,
            Warning2
        }

        public enum ButtonPressed {
            Play,
            Options,
            OK,
            NOK,
            NaN
        }

        public Menu(TextureManager textureManager, MenuType type, SpriteFont font)
        {
            this.textureManager = textureManager;
            Type = type;
            this.font = font;
            Init();
        }

        public Menu(TextureManager textureManager, MenuType type, SpriteFont font, Int16 id)
        {
            this.textureManager = textureManager;
            Type = type;
            this.font = font;
            this.ID = id;
            Init();
        }

        private void Init()
        {
            //létrehozás, és eseménykezelők felrakása
            background = textureManager.MenuWindow;
            Size = new((Int32)(background.Width * 0.7), (Int32)(background.Height * 0.7));
            switch (Type) {
                case MenuType.MainMenu:
                    textBoxes.Add(new TextBox(font, "SnowWanderer"));
                    controls.Add(new Button(textureManager.MenuCustomButton, font, (Int16)ButtonPressed.Play) {
                        Text = "Játék!"
                    });
                    ((Button)controls[0]).Size = new((Int32)(controls[0].Size.X * 0.6), (Int32)(controls[0].Size.Y * 0.6));
                    ((Button)controls[0]).Click += Button_Click;
                    controls.Add(new Button(textureManager.MenuCustomButton, font, (Int16)ButtonPressed.Options) {
                        Text = "Beállítások"
                    });
                    ((Button)controls[1]).Size = new((Int32)(controls[1].Size.X * 0.6), (Int32)(controls[1].Size.Y * 0.6));
                    ((Button)controls[1]).Click += Button_Click;
                    break;
                case MenuType.LevelSelect:
                    Int32 margin = (Int32)Math.Floor(this.Size.X * 0.8 * 0.02);
                    Int32 buttonsize = (Int32)Math.Round(this.Size.X * 0.2 - 5 * margin);
                    textBoxes.Add(new TextBox(font, "Válassz szintet!"));
                    for (short i = 0; i < LevelData.Levels.Length; i++) {
                        controls.Add(new Button(textureManager.MenuCustomButton, font, i) {
                            Text = (i + 1).ToString(),
                            Size = new(buttonsize)
                        });
                        ((Button)controls[i]).Click += LevelButton_Click;
                    }
                    controls.Add(new Button(textureManager.MenuNokButton, font, (Int16)ButtonPressed.NOK));
                    ((Button)controls[LevelData.Levels.Length]).Size = new((Int32)(controls[LevelData.Levels.Length].Size.X * 0.7),
                        (Int32)(controls[LevelData.Levels.Length].Size.Y * 0.7));
                    ((Button)controls[LevelData.Levels.Length]).Click += Button_Click;
                    break;
                case MenuType.Options:
                    textBoxes.Add(new TextBox(font, "Beállítások", 0));
                    textBoxes.Add(new TextBox(font, "Háttérzene hangereje:", 1) {
                        IsCentered = false
                    });
                    textBoxes.Add(new TextBox(font, "SFX hangereje:", 2) {
                        IsCentered = false
                    });
                    textBoxes.Add(new TextBox(font, localmusicVolume.ToString("P2"), 3));
                    textBoxes.Add(new TextBox(font, localsfxvolume.ToString("P2"), 4));
                    controls.Add(new Slider(textureManager, 0) { Value = localmusicVolume });
                    ((Slider)controls[0]).Size = new((Int32)(Size.X * 0.675), controls[0].Size.Y);
                    ((Slider)controls[0]).OnChange += Slider_Event;
                    controls.Add(new Slider(textureManager, 1) { Value = localsfxvolume });
                    ((Slider)controls[1]).Size = new((Int32)(Size.X * 0.675), controls[1].Size.Y);
                    ((Slider)controls[1]).OnChange += Slider_Event;
                    controls.Add(new Button(textureManager.MenuNokButton, font, (Int16)ButtonPressed.NOK));
                    ((Button)controls[2]).Size = new((Int32)(controls[2].Size.X * 0.7),
                        (Int32)(controls[2].Size.Y * 0.7));
                    ((Button)controls[2]).Click += Button_Click;
                    controls.Add(new Button(textureManager.MenuOkButton, font, (Int16)ButtonPressed.OK));
                    ((Button)controls[3]).Size = new((Int32)(controls[3].Size.X * 0.7),
                        (Int32)(controls[3].Size.Y * 0.7));
                    ((Button)controls[3]).Click += Button_Click;
                    break;
                case MenuType.Achievement:
                    textBoxes.Add(new TextBox(font, "Pálya teljesítve!", 0));
                    textBoxes.Add(new TextBox(font, "Elért pontszám:", 1) {
                        IsCentered = false
                    });
                    textBoxes.Add(new TextBox(font, "0", 2));
                    controls.Add(new Button(textureManager.MenuNokButton, font, (Int16)ButtonPressed.NOK));
                    ((Button)controls[0]).Size = new((Int32)(controls[0].Size.X * 0.7),
                        (Int32)(controls[0].Size.Y * 0.7));
                    ((Button)controls[0]).Click += Button_Click;
                    controls.Add(new Button(textureManager.MenuOkButton, font, (Int16)ButtonPressed.OK));
                    ((Button)controls[1]).Size = new((Int32)(controls[1].Size.X * 0.7),
                        (Int32)(controls[1].Size.Y * 0.7));
                    ((Button)controls[1]).Click += Button_Click;
                    break;
                case MenuType.Warning:
                    textBoxes.Add(new TextBox(font, "Figyelem!", 0));
                    textBoxes.Add(new TextBox(font, "Hibaüzenet", 1));
                    controls.Add(new Button(textureManager.MenuOkButton, font, (Int16)ButtonPressed.OK));
                    ((Button)controls[0]).Size = new((Int32)(controls[0].Size.X * 0.7),
                        (Int32)(controls[0].Size.Y * 0.7));
                    ((Button)controls[0]).Click += Button_Click;
                    break;
                case MenuType.Warning2:
                    textBoxes.Add(new TextBox(font, "Figyelem!", 0));
                    textBoxes.Add(new TextBox(font, "Hibaüzenet", 1));
                    controls.Add(new Button(textureManager.MenuNokButton, font, (Int16)ButtonPressed.NOK));
                    ((Button)controls[0]).Size = new((Int32)(controls[0].Size.X * 0.7),
                        (Int32)(controls[0].Size.Y * 0.7));
                    ((Button)controls[0]).Click += Button_Click;
                    controls.Add(new Button(textureManager.MenuOkButton, font, (Int16)ButtonPressed.OK));
                    ((Button)controls[1]).Size = new((Int32)(controls[1].Size.X * 0.7),
                        (Int32)(controls[1].Size.Y * 0.7));
                    ((Button)controls[1]).Click += Button_Click;
                    break;
                default:
                    break;
            }
        }

        public void SaveSettings()
        {
            if (MenuType.Options != Type) {
                return;
            }
            AppSettings.MusicVolume = localmusicVolume;
            AppSettings.SFXVolume = localsfxvolume;
            AppSettings.Fullscreen = localFullscreen;
            AppSettings.Save();
        }

        public void Reset()
        {
            BPressed = ButtonPressed.NaN;
        }

        private void Slider_Event(object? sender, System.EventArgs e)
        {
            if (sender is Slider slider) {
                switch (slider.ID) {
                    case 0:
                        textBoxes[3].Text = slider.Value.ToString("P2");
                        localmusicVolume = slider.Value;
                        break;
                    case 1:
                        textBoxes[4].Text = slider.Value.ToString("P2");
                        localsfxvolume = slider.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Button_Click(object? sender, System.EventArgs e)
        {
            if (sender is Button button) {
                BPressed = (ButtonPressed)button.ID;
            }
        }

        private void LevelButton_Click(object? sender, System.EventArgs e)
        {
            if (sender is Button button) {
                SelectedLevel = button.ID;
                BPressed = ButtonPressed.OK;
            }
        }

        public void Update()
        {
            double modifierX = Size.X / (double)DefSize.X;
            double modifierY = Size.Y / (double)DefSize.Y;
            Point margin = new((Int32)Math.Round(DefMargin.X * modifierX), (Int32)Math.Round(DefMargin.Y * modifierY));
            Int32 middle = Location.X + Size.X / 2;
            switch (Type) {
                case MenuType.MainMenu:
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    controls[0].Location = new(middle - controls[0].Size.X / 2, (Int32)(Location.Y + Size.Y * 0.425));
                    controls[1].Location = new(middle - controls[0].Size.X / 2, (Int32)(Location.Y + Size.Y * 0.625));
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                case MenuType.LevelSelect:
                    Int32 buttonmargin = (Int32)Math.Floor(this.Size.X * 0.8 * 0.02);
                    Point startlocation;
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    startlocation = new(Location.X + margin.X, Location.Y + margin.Y + 50);
                    Point sizeofbutton = controls[0].Size;
                    for (Int32 i = 0; i < controls.Count - 1; i++) {
                        int col = i % 6;
                        int row = i / 6;
                        controls[i].Location = new(startlocation.X + col * (sizeofbutton.X + buttonmargin),
                            startlocation.Y + row * (sizeofbutton.Y + buttonmargin));
                    }
                    controls[controls.Count - 1].Location = new(middle - controls[controls.Count - 1].Size.X / 2,
                        Location.Y + Size.Y - margin.Y - controls[controls.Count - 1].Size.Y);
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                case MenuType.Options:
                    Int32 begin = (Int32)(Location.X + Size.X * 0.1625);
                    Int32 end = (Int32)(Location.X + Size.X * 0.8375);
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    textBoxes[1].Location = new(begin, (Int32)(Location.Y + Size.Y * 0.2308));
                    textBoxes[2].Location = new(begin, (Int32)(Location.Y + Size.Y * 0.4769));
                    textBoxes[3].Location = new(end - textBoxes[3].Size.X / 2, (Int32)(Location.Y + Size.Y * 0.2308));
                    textBoxes[4].Location = new(end - textBoxes[3].Size.X / 2, (Int32)(Location.Y + Size.Y * 0.4769));
                    controls[0].Location = new(begin, (Int32)(Location.Y + Size.Y * 0.2769));
                    controls[1].Location = new(begin, (Int32)(Location.Y + Size.Y * 0.5231));
                    controls[2].Location = new((Int32)(Location.X + Size.X * 0.2), Location.Y + (Int32)(Size.Y * 0.89) - controls[2].Size.Y);
                    controls[3].Location = new((Int32)(Location.X + Size.X * 0.8 - controls[3].Size.X), Location.Y + (Int32)(Size.Y * 0.89) - controls[2].Size.Y);
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                case MenuType.Achievement:
                    Int32 begin0 = (Int32)(Location.X + Size.X * 0.1625);
                    Int32 end0 = (Int32)(Location.X + Size.X * 0.8375);
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    textBoxes[1].Location = new(begin0, (Int32)(Location.Y + Size.Y * 0.308));
                    textBoxes[2].Location = new(end0, (Int32)(Location.Y + Size.Y * 0.308));
                    controls[0].Location = new((Int32)(Location.X + Size.X * 0.2), Location.Y + Size.Y - margin.Y - controls[0].Size.Y);
                    controls[1].Location = new((Int32)(Location.X + Size.X * 0.8 - controls[1].Size.X), Location.Y + Size.Y - margin.Y - controls[1].Size.Y);
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                case MenuType.Warning:
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    textBoxes[1].Location = new(middle, (Int32)(Location.Y + Size.Y * 0.23));
                    controls[0].Location = new(middle - controls[0].Size.X / 2,
                        Location.Y + Size.Y - margin.Y + 15 - controls[0].Size.Y);
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                case MenuType.Warning2:
                    textBoxes[0].Location = new(middle, Location.Y + margin.Y);
                    textBoxes[1].Location = new(middle, (Int32)(Location.Y + Size.Y * 0.308));
                    controls[0].Location = new((Int32)(Location.X + Size.X * 0.2), Location.Y + Size.Y - margin.Y - controls[0].Size.Y);
                    controls[1].Location = new((Int32)(Location.X + Size.X * 0.8 - controls[1].Size.X), Location.Y + Size.Y - margin.Y - controls[1].Size.Y);
                    foreach (var element in controls) {
                        element.Update();
                    }
                    foreach (var textBox in textBoxes) {
                        textBox.Update();
                    }
                    break;
                default:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(Location, Size), Color.White);
            foreach (var element in controls) {
                element.Draw(spriteBatch);
            }
            foreach (var textBox in textBoxes) {
                textBox.Draw(spriteBatch);
            }
        }

        public Texture2D GetTexture(bool _ = false)
        {
            throw new NotImplementedException("Menu has no texture!"); ;
        }

        public void Dispose()
        {
            textBoxes.Clear();
            controls.Clear();
        }
    }
}
