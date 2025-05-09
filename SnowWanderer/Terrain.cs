using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using static SnowWanderer.Menu;

namespace SnowWanderer {
    internal class Terrain : IDrawable, IDisposable {
        private readonly Level level;
        private readonly Menu menu;
        private readonly BitArray terrain;
        private readonly Block[] blocks;
        private readonly Block[] ruins;
        private readonly NPC[] npcs;
        private readonly Point[] npcsLoc;
        private short npcSteps = 2;
        private NPC.Directions npcStepsDirection = NPC.Directions.Right;
        private readonly NPC player;
        private readonly BitArray NPCSync; //command syncronization
        private readonly Objective[] sec_Objs;
        private readonly Objective[] sec_Obj_Markers;
        private readonly BitArray sec_Obj_Done;
        private readonly Objective final_Obj, final_Obj_Marker;
        //objectives
        private bool final_Obj_Done = false;
        private readonly Point[] objsLoc;
        private Point playerLoc, finalLoc;
        //command queue
        private readonly List<Tuple<NPC.Actions, Point>>[] commandQueueNPC;
        private readonly List<Tuple<NPC.Actions, Point>> commandQueue = [];
        private readonly List<List<Tuple<ControlPanel.CommandType, short, ControlPanel?>>> Commands = [];
        private readonly List<int> CommandIndexer = [];
        private bool validateCommands = false;
        private bool awaitingCommands = false;
        //end of command queue
        private readonly Button firstcommandbutton;
        private readonly ControlPanel controlPanel;
        private readonly TextureManager textureManager;
        private readonly AudioManager audioManager;
        private readonly SoundEffectInstance windSFX;
        private readonly SoundEffectInstance[] musicSFX;
        private byte musicIndex = 0;
        private readonly Timer timer = new(60000);
        private readonly SpriteFont font;
        private readonly Point canvasSize;
        private readonly Point ControlsSize;
        private readonly Point ScreenSize;
        private Point size, margin, npcSize, objSize;
        private readonly Button[] buttons = new Button[2];
        public readonly Int32 Width = 12;
        public readonly Int32 Height = 8;
        /// <summary>
        /// 1 to X chance of a non default block spawning.
        /// </summary>
        public Int32 Chance { get; set; } = 3;
        public ButtonPressed BPressed { get; private set; } = ButtonPressed.NaN;
        public Int32 Score { get; private set; } = 0;

        public Terrain(TextureManager textureManager, AudioManager audioManager, SpriteFont font, Point screenSize, Level level)
        {
            //assigning values, and define array sizes
            this.textureManager = textureManager;
            this.audioManager = audioManager;
            this.windSFX = audioManager.Wind.CreateInstance();
            this.musicSFX = [audioManager.BGMusicAsSFX[0].CreateInstance(), audioManager.BGMusicAsSFX[1].CreateInstance()];
            this.font = font;
            this.level = level;
            menu = new(textureManager, Menu.MenuType.Warning, font);
            menu.Location = new Point(screenSize.X / 2 - menu.Size.X / 2, screenSize.Y / 2 - menu.Size.Y / 2);
            menu.WarnText = level.HelpText;
            terrain = level.terrain;
            this.Width = level.Width;
            this.Height = level.Height;
            blocks = new Block[Width * Height];
            this.size = Block.DefSize; //block size
            this.npcSize = NPC.DefSize;
            this.objSize = Objective.DefSize;
            this.margin = Block.DefMargin;
            this.canvasSize = new((Int32)Math.Floor(screenSize.X * 0.75), screenSize.Y);
            this.ControlsSize = new((Int32)Math.Floor(screenSize.X * 0.25), screenSize.Y);
            this.ScreenSize = screenSize;
            this.npcs = new NPC[level.npcs.Length];
            this.npcsLoc = new Point[level.npcs.Length];
            this.NPCSync = new(level.npcs.Length + 1, true);
            this.commandQueueNPC = new List<Tuple<NPC.Actions, Point>>[level.npcs.Length];
            this.ruins = new Block[level.ruins.Length];
            this.sec_Objs = new Objective[level.objectives.Length];
            this.sec_Obj_Markers = new Objective[level.objectives.Length];
            this.sec_Obj_Done = new(level.objectives.Length);
            this.objsLoc = new Point[level.objectives.Length];

            //initialize buttons and music
            windSFX.IsLooped = true;
            windSFX.Volume = AppSettings.SFXVolume;
            windSFX.Play();
            musicSFX[0].IsLooped = false;
            musicSFX[0].Volume = AppSettings.MusicVolume * 0.5f;
            musicSFX[1].IsLooped = false;
            musicSFX[1].Volume = AppSettings.MusicVolume * 0.5f;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();
            this.firstcommandbutton = new(textureManager.AddButton, font, (Int16)ButtonPressed.OK) {
                Size = new(40),
            };
            this.firstcommandbutton.Location = new(canvasSize.X + ControlsSize.X / 2 - firstcommandbutton.Size.X / 2, 0);
            buttons[0] = new(textureManager.MenuCustomButton, font, (Int16)ButtonPressed.NOK) {
                Size = new(ControlsSize.X / 3),
                Text = "Vissza"
            };
            buttons[0].Location = new(canvasSize.X + 10, ControlsSize.Y - buttons[0].Size.Y);
            buttons[0].Click += Button_Click;
            buttons[1] = new(textureManager.MenuOkButton, font, (Int16)ButtonPressed.OK) {
                Size = new(ControlsSize.X / 3)
            };
            buttons[1].Location = new(canvasSize.X + ControlsSize.X - buttons[0].Size.X, ControlsSize.Y - buttons[1].Size.Y);
            buttons[1].Click += Run_Click;

            //initialize control panel
            this.controlPanel = new(textureManager, font, firstcommandbutton, screenSize, ControlsSize, (Int16)ButtonPressed.NaN) {
                Location = new(canvasSize.X + 10, firstcommandbutton.Size.Y),
                Size = new(ControlsSize.X - 20, 48),
                Margin = new(10, 150),
                SliderEnabled = true
            };

            //set block locations and sizes
            double percent = ((double)canvasSize.X / Width) / size.X;
            if (Height * size.X * percent + margin.Y + margin.X > canvasSize.Y) {
                percent = ((double)canvasSize.Y / Height) / (size.Y - margin.Y / Height * (Height - 1) - margin.X / Height * (Height - 1));
            }
            //set new sizes
            size = new((Int32)Math.Floor(size.X * percent), (Int32)Math.Floor(size.Y * percent));
            npcSize = new((Int32)Math.Floor(npcSize.X * percent), (Int32)Math.Floor(npcSize.Y * percent));
            objSize = new((Int32)Math.Floor(objSize.X * percent), (Int32)Math.Floor(objSize.Y * percent));
            margin = new((Int32)Math.Floor(margin.X * percent), (Int32)Math.Floor(margin.Y * percent));
            Point begining = new((Int32)Math.Floor(canvasSize.X / 2.0 - (Width / 2.0 * size.X)),
                                 (Int32)Math.Floor(canvasSize.Y / 2.0 - (Height / 2.0 * size.X) - margin.X));
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    if (terrain[y * Width + x]) {
                        //Draw road
                        Texture2D[] texturepack;
                        if (Random.Shared.Next(Chance) == 0) {
                            //Draw non default block
                            if (Random.Shared.Next(2) == 0) {
                                //Draw animated block
                                texturepack = Random.Shared.Next(2) == 0 ? textureManager.IceBlockANIM1 : textureManager.IceBlockANIM2;
                                blocks[y * Width + x] = new(texturepack);
                            }
                            else {
                                //Draw non animated block
                                blocks[y * Width + x] = new(textureManager.IceBlockDEFAULT[Random.Shared.Next(textureManager.IceBlockDEFAULT.Length - 1) + 1]);
                            }
                        }
                        else {
                            //Draw default block
                            blocks[y * Width + x] = new(textureManager.IceBlockDEFAULT[0]);
                        }
                    }
                    else {
                        //Draw snow
                        Texture2D[] texturepack;
                        if (Random.Shared.Next(Chance) == 0) {
                            //Draw non default block
                            if (Random.Shared.Next(2) == 0) {
                                //Draw animated block
                                texturepack = Random.Shared.Next(2) == 0 ? textureManager.SnowBlockANIM1 : textureManager.SnowBlockANIM2;
                                blocks[y * Width + x] = new(texturepack);
                            }
                            else {
                                //Draw non animated block
                                blocks[y * Width + x] = new(textureManager.SnowBlockDEFAULT[Random.Shared.Next(textureManager.SnowBlockDEFAULT.Length - 1) + 1]);
                            }
                        }
                        else {
                            //Draw default block
                            blocks[y * Width + x] = new(textureManager.SnowBlockDEFAULT[0]);
                        }
                    }
                    //Set block location/size
                    blocks[y * Width + x].Location = new(begining.X + x * size.X, begining.Y + y * size.X);
                    blocks[y * Width + x].Size = size;
                }
            }

            //initialize npc locations and sizes
            for (int i = 0; i < level.npcs.Length; i++) {
                if (level.npcs[i].Item2 == "ICE") {
                    Texture2D[] tmp = (Texture2D[])textureManager.NPC_ICEGOLEM_Dying.Clone();
                    Array.Reverse(tmp);
                    npcs[i] = new(audioManager, [], [], [], textureManager.NPC_ICEGOLEM_Walking, textureManager.NPC_ICEGOLEM_Blinking,
                        [textureManager.NPC_ICEGOLEM_Dying, tmp], [NPC.Actions.Die, NPC.Actions.Wake]);
                }
                else {
                    Texture2D[] tmp = (Texture2D[])textureManager.NPC_STONEGOLEM_Dying.Clone();
                    Array.Reverse(tmp);
                    npcs[i] = new(audioManager, [], [], [], textureManager.NPC_STONEGOLEM_Walking, textureManager.NPC_STONEGOLEM_Blinking,
                        [textureManager.NPC_STONEGOLEM_Dying, tmp], [NPC.Actions.Die, NPC.Actions.Wake]);
                }
                npcsLoc[i] = level.npcs[i].Item1;
            }
            for (int i = 0; i < commandQueueNPC.Length; i++) {
                commandQueueNPC[i] = [];
            }

            //set objective locations and sizes
            for (int i = 0; i < level.objectives.Length; i++) {
                sec_Objs[i] = new(textureManager.Stick, (Int16)i) {
                    Location = new(level.objectives[i].X * size.X + begining.X + size.X / 2, level.objectives[i].Y * size.X + begining.Y + size.Y / 2),
                    Size = objSize
                };
                sec_Obj_Markers[i] = new(textureManager.ObjectiveMarker, (Int16)i) {
                    Location = new(level.objectives[i].X * size.X + begining.X + size.X / 2, level.objectives[i].Y * size.X + begining.Y + size.Y / 2),
                    Size = objSize
                };
                objsLoc[i] = level.objectives[i];
            }
            final_Obj = new(textureManager.Camp, (Int16)level.objectives.Length) {
                Location = new(level.finalObjective.X * size.X + begining.X + size.X / 2, level.finalObjective.Y * size.X + begining.Y + size.Y / 2),
                Size = objSize,
                Margin = new(0, 16)
            };
            final_Obj.Margin = new((Int32)Math.Floor(final_Obj.Margin.X * percent), (Int32)Math.Floor(final_Obj.Margin.Y * percent));
            finalLoc = level.finalObjective;
            final_Obj_Marker = new(textureManager.ObjectiveMarker, (Int16)level.objectives.Length) {
                Location = new(level.finalObjective.X * size.X + begining.X + size.X / 2, level.finalObjective.Y * size.X + begining.Y + size.Y / 2),
                Size = objSize
            };

            //set player location and size
            player = new(audioManager, textureManager.NPC_PLAYER_WalkUp, textureManager.NPC_PLAYER_WalkDown, textureManager.NPC_PLAYER_WalkLeft, textureManager.NPC_PLAYER_WalkRight, [],
            [textureManager.NPC_PLAYER_Pickup], [NPC.Actions.Pickup], true);
            playerLoc = level.playerLoc;
            player.Location = new(playerLoc.X * size.X + begining.X + size.X / 2, playerLoc.Y * size.X + begining.Y + size.Y / 2);
            player.Size = npcSize;
            player.Margin = new(0, 4);
            player.Margin = new((Int32)Math.Floor(player.Margin.X * percent), (Int32)Math.Floor(player.Margin.Y * percent));

            //set npc locations and sizes
            for (var i = 0; i < npcs.Length; i++) {
                npcs[i].Location = new(npcsLoc[i].X * size.X + begining.X + size.X / 2, npcsLoc[i].Y * size.X + begining.Y + size.Y / 2);
                npcs[i].Size = npcSize;
                npcs[i].Margin = new(0, 12);
                npcs[i].Margin = new((Int32)Math.Floor(npcs[i].Margin.X * percent), (Int32)Math.Floor(npcs[i].Margin.Y * percent));
            }

            //initialize and set ruin locations and sizes
            for (var i = 0; i < ruins.Length; i++) {
                ruins[i] = new(textureManager.Ruins[level.ruins[i].Item2], (Int16)i) {
                    Location = new(level.ruins[i].Item1.X * size.X + begining.X, level.ruins[i].Item1.Y * size.X + begining.Y),
                    Size = textureManager.Ruins[level.ruins[i].Item2].Bounds.Size
                };
                ruins[i].Size = new((Int32)Math.Floor(ruins[i].Size.X * percent), (Int32)Math.Floor(ruins[i].Size.Y * percent));
            }

            //test
            commandQueue.Add(new(NPC.Actions.Pickup, Point.Zero));
            for (int i = 0; i < commandQueueNPC.Length; i++) {
                commandQueueNPC[i].Add(new(NPC.Actions.Wake, Point.Zero));
            }
        }

        private void Button_Click(object? sender, System.EventArgs e)
        {
            if (sender is Button button) {
                BPressed = (ButtonPressed)button.ID;
            }
        }

        private void Run_Click(object? sender, System.EventArgs e)
        {
            if (sender is Button) {
                buttons[1].Enabled = false;
                Commands.Add(controlPanel.GetCommands());
                CommandIndexer.Add(0);
                validateCommands = true;
                controlPanel.LockControls();
            }
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            musicSFX[musicIndex].Play();
            musicIndex = musicIndex == 0 ? (byte)1 : (byte)0;
            timer.Interval = 240000;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // canvas area
            spriteBatch.Draw(textureManager.Backgrounds[level.bgimage], new Rectangle(Point.Zero, ScreenSize), Color.White);
            BitArray npcdrawn = new(npcs.Length + 1);
            BitArray obj_Drawn = new(sec_Objs.Length + 1);
            BitArray ruinsDrawn = new(ruins.Length);
            for (int i = 0; i < blocks.Length; i++) {
                Point loc = new(i % Width, i / Width);
                //draw objectives
                for (int j = 0; j < sec_Objs.Length; j++) {
                    if (!obj_Drawn[j] && objsLoc[j].Y < loc.Y && !sec_Obj_Done[j]) {
                        sec_Objs[j].Draw(spriteBatch);
                        sec_Obj_Markers[j].Draw(spriteBatch);
                        obj_Drawn[j] = true;
                    }
                }
                if (!obj_Drawn[^1] && finalLoc.Y < loc.Y) {
                    final_Obj.Draw(spriteBatch);
                    final_Obj_Marker.Draw(spriteBatch);
                    obj_Drawn[^1] = true;
                }
                //draw ruins
                for (int j = 0; j < ruins.Length; j++) {
                    if (!ruinsDrawn[j] && ruins[j].Location.Y + ruins[j].Size.Y <= blocks[i].Location.Y + 26) {
                        ruins[j].Draw(spriteBatch);
                        ruinsDrawn[j] = true;
                    }
                }
                //draw npcs
                for (int j = 0; j < npcs.Length; j++) {
                    if (!npcdrawn[j] && npcsLoc[j].Y < loc.Y) {
                        npcs[j].Draw(spriteBatch);
                        npcdrawn[j] = true;
                    }
                }
                if (!npcdrawn[^1] && playerLoc.Y < loc.Y) {
                    player.Draw(spriteBatch);
                    npcdrawn[^1] = true;
                }
                //draw blocks
                blocks[i].Draw(spriteBatch);
            }
            //draw last row
            for (int j = 0; j < sec_Objs.Length; j++) {
                if (!obj_Drawn[j] && !sec_Obj_Done[j]) {
                    sec_Objs[j].Draw(spriteBatch);
                    sec_Obj_Markers[j].Draw(spriteBatch);
                }
            }
            if (!obj_Drawn[^1]) {
                final_Obj.Draw(spriteBatch);
                final_Obj_Marker.Draw(spriteBatch);
            }
            for (int j = 0; j < ruins.Length; j++) {
                if (!ruinsDrawn[j]) {
                    ruins[j].Draw(spriteBatch);
                }
            }
            for (int j = 0; j < npcs.Length; j++) {
                if (!npcdrawn[j]) {
                    npcs[j].Draw(spriteBatch);
                }
            }
            if (!npcdrawn[^1]) {
                player.Draw(spriteBatch);
            }
            // controls area
            firstcommandbutton.Draw(spriteBatch);
            controlPanel.Draw(spriteBatch);
            foreach (var button in buttons) {
                button.Draw(spriteBatch);
            }
            if (menu.BPressed == ButtonPressed.NaN && level.HelpText != "") {
                menu.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            // command translator
            if (validateCommands && awaitingCommands) { // megvárni, amig mindenki idle
                if (CommandIndexer[0] < controlPanel.GetCommandCount()) {
                    if (ValidateCondition()) {
                        int i;
                        if (CommandIndexer.Count > 1) {
                            i = CommandIndexer[^1] % Commands[^2][CommandIndexer[^2]].Item3.GetCommandCount();
                        }
                        else {
                            i = CommandIndexer[^1];
                        }
                        switch (Commands[^1][i].Item1) {
                            case ControlPanel.CommandType.For:
                                if (Commands[^1][i].Item3.GetCommandCount() != 0) {
                                    CommandIndexer.Add(0);
                                    Commands.Add(Commands[^1][i].Item3.GetCommands());
                                }
                                else {
                                    if (CommandIndexer.Count != 0) {
                                        CommandIndexer[^1]++;
                                    }
                                }
                                break;
                            case ControlPanel.CommandType.While:
                                if (Commands[^1][i].Item3.GetCommandCount() != 0) {
                                    CommandIndexer.Add(0);
                                    Commands.Add(Commands[^1][i].Item3.GetCommands());
                                }
                                else {
                                    if (CommandIndexer.Count != 0) {
                                        CommandIndexer[^1]++;
                                    }
                                }
                                break;
                            case ControlPanel.CommandType.If:
                                if (Commands[^1][i].Item3.GetCommandCount() != 0) {
                                    CommandIndexer.Add(0);
                                    Commands.Add(Commands[^1][i].Item3.GetCommands());
                                }
                                else {
                                    if (CommandIndexer.Count != 0) {
                                        CommandIndexer[^1]++;
                                    }
                                }
                                break;
                            case ControlPanel.CommandType.OP:
                                bool IsReset = false;
                                switch ((ControlPanel.OPType)Commands[^1][i].Item2) {
                                    case ControlPanel.OPType.up:
                                        if (playerLoc.Y - 1 < 0) {
                                            ResetTerrain();
                                            break;
                                        }
                                        for (int j = 0; j < npcsLoc.Length; j++) {
                                            if (playerLoc == npcsLoc[j]) {
                                                ResetTerrain();
                                                IsReset = true;
                                                break;
                                            }
                                        }
                                        if (IsReset) {
                                            break;
                                        }
                                        commandQueue.Add(new(NPC.Actions.Move, new(player.Location.X, player.Location.Y - size.X)));
                                        break;
                                    case ControlPanel.OPType.down:
                                        if (playerLoc.Y + 1 > Height) {
                                            ResetTerrain();
                                            break;
                                        }
                                        for (int j = 0; j < npcsLoc.Length; j++) {
                                            if (playerLoc == npcsLoc[j]) {
                                                ResetTerrain();
                                                IsReset = true;
                                                break;
                                            }
                                        }
                                        if (IsReset) {
                                            break;
                                        }
                                        commandQueue.Add(new(NPC.Actions.Move, new(player.Location.X, player.Location.Y + size.X)));
                                        break;
                                    case ControlPanel.OPType.left:
                                        if (playerLoc.X - 1 < 0) {
                                            ResetTerrain();
                                            break;
                                        }
                                        for (int j = 0; j < npcsLoc.Length; j++) {
                                            if (playerLoc == npcsLoc[j]) {
                                                ResetTerrain();
                                                IsReset = true;
                                                break;
                                            }
                                        }
                                        if (IsReset) {
                                            break;
                                        }
                                        commandQueue.Add(new(NPC.Actions.Move, new(player.Location.X - size.X, player.Location.Y)));
                                        break;
                                    case ControlPanel.OPType.right:
                                        if (playerLoc.X + 1 > Width) {
                                            ResetTerrain();
                                            break;
                                        }
                                        for (int j = 0; j < npcsLoc.Length; j++) {
                                            if (playerLoc == npcsLoc[j]) {
                                                ResetTerrain();
                                                IsReset = true;
                                                break;
                                            }
                                        }
                                        if (IsReset) {
                                            break;
                                        }
                                        commandQueue.Add(new(NPC.Actions.Move, new(player.Location.X + size.X, player.Location.Y)));
                                        break;
                                    case ControlPanel.OPType.pickup:
                                        // pickup a camp
                                        commandQueue.Add(new(NPC.Actions.Pickup, Point.Zero));
                                        break;
                                    case ControlPanel.OPType.wait:
                                        commandQueue.Add(new(NPC.Actions.Wait, Point.Zero));
                                        break;
                                    default:
                                        break;
                                }
                                if (CommandIndexer.Count != 0) {
                                    CommandIndexer[^1]++;
                                }
                                else {
                                    break;
                                }
                                //npck mozgása
                                if (npcSteps == 4) {
                                    npcSteps = 0;
                                    if (npcStepsDirection == NPC.Directions.Right) {
                                        npcStepsDirection = NPC.Directions.Left;
                                    }
                                    else {
                                        npcStepsDirection = NPC.Directions.Right;
                                    }
                                }
                                for (int j = 0; j < commandQueueNPC.Length; j++) {
                                    if (npcStepsDirection == NPC.Directions.Right) {
                                        commandQueueNPC[j].Add(new(NPC.Actions.Move, new(npcs[j].Location.X + size.X, npcs[j].Location.Y)));
                                    }
                                    else {
                                        commandQueueNPC[j].Add(new(NPC.Actions.Move, new(npcs[j].Location.X - size.X, npcs[j].Location.Y)));
                                    }
                                }
                                npcSteps++;
                                break;
                            default:
                                break;
                        }
                    }
                    else {
                        CommandIndexer.RemoveAt(CommandIndexer.Count - 1);
                        Commands.RemoveAt(Commands.Count - 1);
                        CommandIndexer[^1]++;
                    }
                    awaitingCommands = false;
                }
                else {
                    if (sec_Obj_Done.Cast<bool>().All(b => b) && final_Obj_Done) {
                        BPressed = ButtonPressed.OK;
                        //fire effect !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    }
                    else {
                        ResetTerrain();
                    }
                    Score = (Int32)Math.Ceiling((double)level.MaxScore / controlPanel.GetTotalCommandCount() * 100);
                }
            }
            // commands
            if (commandQueue.Count != 0) {
                if (NPCSync[^1]) {
                    if (commandQueue[0].Item1 == NPC.Actions.Move) {
                        player.SetAction(commandQueue[0].Item2, ref playerLoc);
                        commandQueue.RemoveAt(0);
                    }
                    else if (commandQueue[0].Item1 == NPC.Actions.Pickup) {
                        player.SetAction(commandQueue[0].Item1);
                        commandQueue.RemoveAt(0);
                        for (int i = 0; i < objsLoc.Length; i++) {
                            if (objsLoc[i] == playerLoc) {
                                SoundEffectInstance sfx = audioManager.Collect[Random.Shared.Next(0, audioManager.Collect.Length)].CreateInstance();
                                sfx.Volume = AppSettings.SFXVolume;
                                sfx.Play();
                                sec_Obj_Done[i] = true;
                            }
                        }
                        if (finalLoc == playerLoc && sec_Obj_Done.Cast<bool>().All(b => b)) {
                            SoundEffectInstance sfx = audioManager.Place[Random.Shared.Next(0, audioManager.Place.Length)].CreateInstance();
                            sfx.Volume = AppSettings.SFXVolume;
                            sfx.Play();
                            final_Obj_Done = true;
                        }
                    }
                    else {
                        player.SetAction(commandQueue[0].Item1);
                        commandQueue.RemoveAt(0);
                    }
                    NPCSync[^1] = false;
                }
            }
            for (int i = 0; i < npcs.Length; i++) {
                if (commandQueueNPC[i].Count != 0) {
                    if (NPCSync[i]) {
                        if (commandQueueNPC[i][0].Item1 == NPC.Actions.Move) {
                            npcs[i].SetAction(commandQueueNPC[i][0].Item2, ref npcsLoc[i]);
                            commandQueueNPC[i].RemoveAt(0);
                        }
                        else {
                            npcs[i].SetAction(commandQueueNPC[i][0].Item1);
                            commandQueueNPC[i].RemoveAt(0);
                        }
                        NPCSync[i] = false;
                    }
                }
            }
            if (npcs.All(s => s.CurrentAction == NPC.Actions.Idle) && player.CurrentAction == NPC.Actions.Idle) {
                NPCSync.SetAll(true);
                if (commandQueue.Count == 0 && commandQueueNPC.All(s => s.Count == 0)) {
                    awaitingCommands = true;
                }
            }
            // canvas area
            foreach (var block in blocks) {
                block.Update();
            }
            foreach (var npc in npcs) {
                npc.Update();
            }
            player.Update();
            for (int i = 0; i < sec_Objs.Length; i++) {
                if (!sec_Obj_Done[i]) {
                    sec_Objs[i].Update();
                }
            }
            for (int i = 0; i < sec_Obj_Markers.Length; i++) {
                if (!sec_Obj_Done[i]) {
                    sec_Obj_Markers[i].Update();
                }
            }
            final_Obj.Update();
            final_Obj_Marker.Update();
            foreach (var ruin in ruins) {
                ruin.Update();
            }
            // controls area
            firstcommandbutton.Update();
            controlPanel.Update();
            foreach (var button in buttons) {
                button.Update();
            }
            if (menu.BPressed == ButtonPressed.NaN && level.HelpText != "") {
                menu.Update();
            }
        }

        public void ResetTerrain()
        {
            // audio lejatszasa
            SoundEffectInstance sfx = audioManager.Big_Punch.CreateInstance();
            sfx.Volume = AppSettings.SFXVolume;
            sfx.Play();
            // a commandokat is resetelni kell
            validateCommands = false;
            awaitingCommands = false;
            Commands.Clear();
            CommandIndexer.Clear();
            commandQueue.Clear();
            foreach (var commandQNPC in commandQueueNPC) {
                commandQNPC.Clear();
            }
            controlPanel.UnLockControls();

            Point begining = new((Int32)Math.Floor(canvasSize.X / 2.0 - (Width / 2.0 * size.X)),
                                 (Int32)Math.Floor(canvasSize.Y / 2.0 - (Height / 2.0 * size.X) - margin.X));
            //initialize npc locations and sizes
            for (int i = 0; i < level.npcs.Length; i++) {
                npcsLoc[i] = level.npcs[i].Item1;
            }

            //set player location and size
            playerLoc = level.playerLoc;
            player.Location = new(playerLoc.X * size.X + begining.X + size.X / 2, playerLoc.Y * size.X + begining.Y + size.Y / 2);

            //set npc locations and sizes
            for (var i = 0; i < npcs.Length; i++) {
                npcs[i].Location = new(npcsLoc[i].X * size.X + begining.X + size.X / 2, npcsLoc[i].Y * size.X + begining.Y + size.Y / 2);
            }

            //reset objectives
            for (int i = 0; i < sec_Obj_Done.Length; i++) {
                sec_Obj_Done[i] = false;
            }
            final_Obj_Done = false;
            npcSteps = 2;
            npcStepsDirection = NPC.Directions.Right;
            buttons[1].Enabled = true;
        }

        /// <summary>
        /// Megnézi, hogy az elágazások vagy ciklusok teljesítik-e a futási feltételt.
        /// </summary>
        /// <returns></returns>
        private bool ValidateCondition()
        {
            if (CommandIndexer.Count > 1) {
                int a = CommandIndexer[^2];
                int b = CommandIndexer[^1];
                Tuple<ControlPanel.CommandType, short, ControlPanel?> ac = Commands[^2][a];
                switch (ac.Item1) {
                    case ControlPanel.CommandType.For:
                        if ((double)b / ac.Item3.GetCommandCount() < ac.Item2) {
                            return true;
                        }
                        return false;
                    case ControlPanel.CommandType.While:
                        switch ((ControlPanel.IfType)ac.Item2) {
                            case ControlPanel.IfType.Standonobj:
                                for (int i = 0; i < objsLoc.Length; i++) {
                                    if (objsLoc[i] == playerLoc && !sec_Obj_Done[i]) {
                                        return false;
                                    }
                                }
                                if (finalLoc == playerLoc) {
                                    return false;
                                }
                                return true;
                            case ControlPanel.IfType.Enemyinrange:
                                Point[] alterLocs = [new(playerLoc.X-1, playerLoc.Y), new(playerLoc.X, playerLoc.Y-1),
                                                     new(playerLoc.X+1, playerLoc.Y), new(playerLoc.X, playerLoc.Y+1)];
                                for (int i = 0; i < npcsLoc.Length; i++) {
                                    for (int j = 0; j < alterLocs.Length; j++) {
                                        if (npcsLoc[i] == alterLocs[j]) {
                                            return false;
                                        }
                                    }
                                }
                                return true;
                            case ControlPanel.IfType.Enemyinfront:
                                Point alterLoc = player.CurrentDirection switch {
                                    NPC.Directions.Up => new Point(playerLoc.X, playerLoc.Y - 1),
                                    NPC.Directions.Down => new Point(playerLoc.X, playerLoc.Y + 1),
                                    NPC.Directions.Left => new Point(playerLoc.X - 1, playerLoc.Y),
                                    NPC.Directions.Right => new Point(playerLoc.X + 1, playerLoc.Y),
                                    _ => throw new InvalidOperationException("Invalid direction")
                                };
                                for (int i = 0; i < npcsLoc.Length; i++) {
                                    if (npcsLoc[i] == alterLoc) {
                                        return false;
                                    }
                                }
                                return true;
                            default:
                                return false;
                        }
                    case ControlPanel.CommandType.If:
                        bool isValid = false;
                        switch ((ControlPanel.IfType)ac.Item2) {
                            case ControlPanel.IfType.Standonobj:
                                for (int i = 0; i < objsLoc.Length; i++) {
                                    if (objsLoc[i] == playerLoc && !sec_Obj_Done[i]) {
                                        isValid = true;
                                    }
                                }
                                if (finalLoc == playerLoc) {
                                    isValid = true;
                                }
                                break;
                            case ControlPanel.IfType.Enemyinrange:
                                Point[] alterLocs = [new(playerLoc.X-1, playerLoc.Y), new(playerLoc.X, playerLoc.Y-1),
                                                     new(playerLoc.X+1, playerLoc.Y), new(playerLoc.X, playerLoc.Y+1)];
                                for (int i = 0; i < npcsLoc.Length; i++) {
                                    for (int j = 0; j < alterLocs.Length; j++) {
                                        if (npcsLoc[i] == alterLocs[j]) {
                                            isValid = true;
                                        }
                                    }
                                }
                                break;
                            case ControlPanel.IfType.Enemyinfront:
                                Point alterLoc = player.CurrentDirection switch {
                                    NPC.Directions.Up => new(playerLoc.X, playerLoc.Y - 1),
                                    NPC.Directions.Down => new(playerLoc.X, playerLoc.Y + 1),
                                    NPC.Directions.Left => new(playerLoc.X - 1, playerLoc.Y),
                                    NPC.Directions.Right => new(playerLoc.X + 1, playerLoc.Y),
                                    _ => throw new InvalidOperationException("Invalid direction")
                                };
                                for (int i = 0; i < npcsLoc.Length; i++) {
                                    if (npcsLoc[i] == alterLoc) {
                                        isValid = true;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        if (isValid && (double)b / ac.Item3.GetCommandCount() < 1.0) {
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }
            }
            else {
                return true;
            }
        }

        public void Dispose()
        {
            windSFX.Stop();
            windSFX.Dispose();
            timer.Stop();
            timer.Dispose();
            foreach (var sfx in musicSFX) {
                sfx.Stop();
                sfx.Dispose();
            }
        }
    }
}
