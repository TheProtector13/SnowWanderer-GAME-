using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    class ControlPanel : IGraphicObject, IDisposable {
        private readonly TextureManager textureManager;
        private readonly SpriteFont font;
        private readonly List<Command> Commands = [];
        private readonly Point controlAreaSize;
        private readonly Point screenSize;
        private readonly Button startbutton;
        private readonly Menu menu;
        private readonly DropdownSelector dropdownSelector;
        private bool addMode = false;

        // Új slider a parancsok görgetéséhez
        private readonly Slider verticalSlider;

        /// <summary>
        /// Ha true, a slider (és így a scroll logika) aktív – alapértelmezett érték: false
        /// </summary>
        public bool SliderEnabled { get; set; } = false;

        public Int16 ID { get; init; } = 0;
        public Point Location { get; set; } = Point.Zero;

        /// <summary>
        /// A ControlPanel mérete (itt a teljes panel keretét várjuk, ide értendő a commandok területe + esetleges slider)
        /// </summary>
        public Point Size { get; set; } = new(192, 32);
        /// <summary>
        /// A belső margó, amelyet mindkét oldalon levonunk – példányosításnál lesz megadva.
        /// </summary>
        public Point Margin { get; set; } = Point.Zero;
        public static Point DefSize { get; } = new(192, 48);
        public static Point DefMargin { get; } = Point.Zero;

        public enum CommandType {
            For,
            While,
            If,
            OP
        }

        public enum IfType {
            Standonobj,
            Enemyinfront,
            Enemyinrange
        }

        public enum OPType {
            up,
            down,
            left,
            right,
            pickup,
            wait
        }

        // Paraméterként kapott controlAreaSize megadja azt a területet (például a panel tartalma), ahol a parancsok látszanak.
        // A slider és a commandok egyaránt ebbe a területbe esnek.
        public ControlPanel(TextureManager textureManager, SpriteFont font, Button startbutton, Point screenSize, Point controlAreaSize)
        {
            this.textureManager = textureManager;
            this.font = font;
            this.startbutton = startbutton;
            menu = new(textureManager, Menu.MenuType.Warning2, font);
            dropdownSelector = new(font, textureManager.FieldBG, ["Ciklus", "Amíg", "Ha", "Művelet"], [0, 1, 2, 3]);
            this.screenSize = screenSize;
            this.controlAreaSize = controlAreaSize;
            Init();

            // A slider példányosítása (függőleges mód)
            verticalSlider = new Slider(textureManager) {
                IsVertical = true
            };
        }

        public ControlPanel(TextureManager textureManager, SpriteFont font, Button startbutton, Point screenSize, Point controlAreaSize, Int16 id)
            : this(textureManager, font, startbutton, screenSize, controlAreaSize)
        {
            this.ID = id;
        }

        private void Init()
        {
            // A menu és a dropdown beállítása
            menu.Location = new Point(screenSize.X / 2 - menu.Size.X / 2, screenSize.Y / 2 - menu.Size.Y / 2);
            menu.WarnText = "Válassz objektumot:";
            dropdownSelector.Location = new Point(menu.Location.X + menu.Size.X / 2, menu.Location.Y + menu.Size.Y / 2);

            startbutton.Click += AddCommand;
        }

        private void AddCommand(object? sender, EventArgs e)
        {
            addMode = true;
        }

        private void RemoveCommand(object? sender, EventArgs e)
        {
            if (Commands.Count > 0) {
                Commands.RemoveAt(Commands.Count - 1);
                if (Commands.Count != 0) {
                    Commands[^1].ButtonsEnabled = true;
                }
            }
        }

        public void LockControls()
        {
            for (int i = 0; i < Commands.Count; i++) {
                Commands[i].Enabled = false;
                Commands[i].SubCommands?.LockControls();
            }
        }

        public void UnLockControls()
        {
            for (int i = 0; i < Commands.Count; i++) {
                Commands[i].Enabled = true;
                Commands[i].SubCommands?.UnLockControls();
            }
        }

        public List<Tuple<CommandType, short, ControlPanel?>> GetCommands()
        {
            List<Tuple<CommandType, short, ControlPanel?>> outp = [];
            foreach (Command cmd in Commands) {
                short val = 0;
                if (cmd.InputBox is NumericInputBox numBox) {
                    val = numBox.Value;
                }
                else if (cmd.InputBox is DropdownSelector selector) {
                    val = selector.Value;
                }
                outp.Add(new Tuple<CommandType, short, ControlPanel?>((CommandType)cmd.ID, val, cmd.SubCommands));
            }
            return outp;
        }

        public int GetCommandCount()
        {
            return Commands.Count;
        }

        //scorehoz kell
        public int GetTotalCommandCount()
        {
            int count = 0;
            foreach (Command cmd in Commands) {
                if (cmd.SubCommands != null) {
                    count += cmd.SubCommands.GetTotalCommandCount();
                }
                count++;
            }
            return count;
        }

        public int GetAbsSizeY()
        {
            int size = 0;
            for (int i = 0; i < Commands.Count; i++) {
                size += Commands[i].GetSizeY();
            }
            return size;
        }

        public void Update()
        {
            if (addMode) {
                menu.Update();
                dropdownSelector.Update();
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    if (menu.BPressed == Menu.ButtonPressed.OK) {
                        switch (dropdownSelector.Value) {
                            case 0:
                                Commands.Add(new Command(textureManager, textureManager.ControlFOR[0], textureManager.ControlFOR[1], font, screenSize, controlAreaSize, Margin, 0, true) { Text = "Ciklus" });
                                break;
                            case 1:
                                Commands.Add(new Command(textureManager, textureManager.ControlWHILE[0], textureManager.ControlWHILE[1], font, screenSize, controlAreaSize, Margin, 1) { Text = "Amíg" });
                                break;
                            case 2:
                                Commands.Add(new Command(textureManager, textureManager.ControlIF[0], textureManager.ControlIF[1], font, screenSize, controlAreaSize, Margin, 2) { Text = "Ha" });
                                break;
                            case 3:
                                Commands.Add(new Command(textureManager, textureManager.ControlOP, font, 3) { Text = "OP" });
                                break;
                            default:
                                break;
                        }
                        // Ha új command jön létre, beállítjuk a méretét.
                        Commands[^1].Size = this.Size;
                        Commands[^1].AddEventHandler(AddCommand, 0);
                        Commands[^1].AddEventHandler(RemoveCommand, 1);
                        if (Commands.Count != 1) {
                            Commands[^2].ButtonsEnabled = false;
                        }
                    }
                    menu.Reset();
                    addMode = false;
                }
            }
            else {
                // A képernyő jobb oldalán van a control panel (ahol a commandok látszanak)
                // Számoljuk ki a látható területet – figyelembe véve a marginokat.
                // Ha a slider aktív, a parancsok területéből levonjuk a slider szélességét (10 px) + 5 px rést.
                int sliderWidth = 10, gap = 5;
                bool showSlider = SliderEnabled && GetAbsSizeY() + startbutton.Size.Y > controlAreaSize.Y - Margin.Y;
                int commandAreaWidth = controlAreaSize.X - 2 * Margin.X - (showSlider ? (sliderWidth + gap - Margin.X) : 0);

                // A commandok rajzolási területének bal felső sarka:
                // Feltételezzük, hogy a startbutton alatti terület tartalmazza őket.
                Point commandAreaOrigin = new(screenSize.X - controlAreaSize.X + Margin.X, startbutton.Location.Y + startbutton.Size.Y / 2);
                int visibleHeight = controlAreaSize.Y - Margin.Y;

                // Ha görgetés szükséges, számoljuk ki a scroll offset-et.
                int totalCommandsHeight = GetAbsSizeY() + startbutton.Size.Y;
                int scrollOffset = 0;
                if (showSlider) {
                    // Inicializáljuk (vagy frissítjük) a slider méretét és helyét:
                    verticalSlider.Location = new Point(commandAreaOrigin.X + commandAreaWidth + gap, commandAreaOrigin.Y);
                    verticalSlider.Size = new Point(visibleHeight, sliderWidth);
                    verticalSlider.Update();

                    scrollOffset = (int)(verticalSlider.Value * (totalCommandsHeight - visibleHeight));
                }
                else {
                    // Ha nincs szükséges görgetés, reseteljük a slider értékét
                    if (verticalSlider != null) {
                        verticalSlider.Value = 0;
                    }
                }

                // Ha nincs command, engedélyezzük a startbutton-t; ha van, módosítjuk a commandok elhelyezkedését
                if (Commands.Count == 0) {
                    startbutton.Enabled = true;
                }
                else {
                    startbutton.Enabled = false;
                    // Elhelyezzük az első commandot a látható terület tetejére (görgetés figyelembevételével)
                    int curY = commandAreaOrigin.Y - scrollOffset;
                    Commands[0].Location = new Point(commandAreaOrigin.X, curY);
                    Commands[0].Update();
                    // A többi command egymás alá kerül
                    for (int i = 1; i < Commands.Count; i++) {
                        curY += Commands[i - 1].GetSizeY();
                        Commands[i].Location = new Point(commandAreaOrigin.X, curY);
                        Commands[i].Update();
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (addMode) {
                menu.Draw(spriteBatch);
                dropdownSelector.Draw(spriteBatch);
            }
            else {
                // Rajzoljuk a commandokat
                foreach (Command cmd in Commands) {
                    cmd.Draw(spriteBatch);
                }
                // Ha a slider aktív, rajzoljuk azt is
                if (SliderEnabled && GetAbsSizeY() + startbutton.Size.Y > (controlAreaSize.Y - Margin.Y)) {
                    verticalSlider.Draw(spriteBatch);
                }
            }
        }

        public Texture2D GetTexture(bool _ = false)
        {
            throw new NotImplementedException("ControlPanel has no texture!");
        }

        public void Dispose()
        {
            Commands.Clear();
        }
    }
}
