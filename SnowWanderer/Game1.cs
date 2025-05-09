using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SnowWanderer;

public class Game1 : Game {
    private readonly GraphicsDeviceManager _graphics;
    private TextureManager textureManager;
    private AudioManager audioManager;
    private Terrain terrain;
    private SpriteFont font;
    private SpriteBatch _spriteBatch;
    private Point canvasSize;
    private Menu menu;
    private Int16 selLevel = 0;
    private Int32 bgimageindex = 0;
    private GameState currentstate = GameState.MainMenu;

    enum GameState {
        MainMenu,
        LevelSelect,
        Options,
        Achievement,
        warning,
        playing
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
        AppSettings.Init();
        LevelData.Init();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        textureManager = new TextureManager(Content);
        bgimageindex = Random.Shared.Next(0, textureManager.Backgrounds.Length);
        audioManager = new AudioManager(Content);
        MediaPlayer.Volume = AppSettings.MusicVolume;
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(audioManager.BGMusic[1]);
        font = Content.Load<SpriteFont>("FONTS/Ariel");
        canvasSize = new(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height);
        menu = new(textureManager, Menu.MenuType.MainMenu, font);
        menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
        // TODO: use this.Content to load your game content here
        //saving levels ++debug++
        //// level1
        //int w = 12, h = 8;
        //BitArray terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 6 + i] = true;
        //}

        //Tuple<Point, byte>[] ruins = [new(new(8, 2), 7), new(Point.Zero, 1)];
        //Tuple<Point, string>[] npcs = [new(new(3, 4), "ICE")];
        //Point player = new(0, 6);
        //Point[] objs = [new(4, 6)];
        //Point final = new(11, 6);
        //int maxscore = 6;
        //string helptext = "Üdv! Rossz hírem van, egyből a mélyvízbe\n" +
        //                  "dobunk. Szedd össze a rézpénzzel jelölt\n" +
        //                  "tárgyakat a pályán. Fontos megjegyezni,\n" +
        //                  "hogy a tábortüzet, azaz az utolsó\n" +
        //                  "tárgyat csak akkor tudod meggyújtani,\n" +
        //                  "ha már minden mást összeszedtél.\n" +
        //                  "A jobb felső sarokban található + jellel\n" +
        //                  "hozhatsz létre utasításokat, és a zöld\n" +
        //                  "pipával futtathatod! Sok sikert!";

        //Level level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/1.dat");
        ////
        //// level2
        //w = 12; h = 8;
        //terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 5 + i] = true;
        //}
        //terrain[w * 6 + 5] = true;
        //terrain[w * 7 + 5] = true;
        //terrain[w * 4 + 9] = true;
        //terrain[w * 4 + 11] = true;

        //ruins = [new(new(7, 3), 3), new(Point.Zero, 5)];
        //npcs = [new(new(5, 6), "ICE")];
        //player = new(0, 5);
        //objs = [new(5, 7), new(9, 4)];
        //final = new(11, 4);
        //maxscore = 10;
        //helptext = "Figyelj az szörnyekre is!";

        //level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/2.dat");
        ////
        //// level3
        //w = 12; h = 8;
        //terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 5 + i] = true;
        //    terrain[w * 4 + i] = true;
        //}
        //terrain[w * 3 + 1] = true;
        //terrain[w * 6 + 6] = true;
        //terrain[w * 7 + 6] = true;
        //terrain[w * 6 + 8] = true;
        //terrain[w * 2 + 7] = true;

        //ruins = [new(new(7, 6), 2), new(Point.Zero, 4), new(new(9, 1), 6), new(new(2, 1), 0)];
        //npcs = [new(new(6, 4), "ICE"), new(new(8, 5), "STONE")];
        //player = new(0, 4);
        //objs = [new(1, 3), new(6, 7), new(8, 6), new(7, 2)];
        //final = new(10, 5);
        //maxscore = 15;
        //helptext = "";

        //level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/3.dat");
        ////
        //// level4
        //w = 12; h = 8;
        //terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 7 + i] = true;
        //}
        //terrain[w * 6 + 7] = true;
        //terrain[w * 5 + 7] = true;
        //terrain[w * 4 + 7] = true;
        //terrain[w * 3 + 7] = true;
        //terrain[w * 3 + 8] = true;
        //terrain[w * 6 + 3] = true;
        //terrain[w * 5 + 3] = true;
        //terrain[w * 5 + 1] = true;

        //ruins = [new(Point.Zero, 5), new(new(9, 0), 7), new(new(6, 4), 9)];
        //npcs = [new(new(7, 6), "ICE"), new(new(7, 3), "STONE"), new(new(2, 5), "ICE")];
        //player = new(0, 7);
        //objs = [new(10, 2), new(3, 5), new(8, 3), new(11, 7)];
        //final = new(1, 5);
        //maxscore = 20;
        //helptext = "";

        //level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/4.dat");
        ////
        //// level5
        //w = 12; h = 8;
        //terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 0 + i] = true;
        //}
        //terrain[w * 1 + 0] = true;
        //terrain[w * 2 + 0] = true;
        //terrain[w * 2 + 1] = true;
        //terrain[w * 2 + 2] = true;
        //terrain[w * 2 + 4] = true;
        //terrain[w * 1 + 9] = true;
        //terrain[w * 3 + 9] = true;
        //terrain[w * 4 + 9] = true;
        //terrain[w * 2 + 6] = true;
        //terrain[w * 3 + 6] = true;
        //terrain[w * 5 + 6] = true;
        //terrain[w * 4 + 7] = true;

        //ruins = [new(new(0, 4), 1), new(new(5, 6), 2), new(new(9, 6), 4)];
        //npcs = [new(new(5, 0), "ICE"), new(new(9, 3), "ICE")];
        //player = new(0, 0);
        //objs = [new(2, 2), new(9, 4), new(9, 1), new(6, 5), new(7, 4)];
        //final = new(4, 2);
        //maxscore = 25;
        //helptext = "";

        //level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/5.dat");
        ////
        //// level6
        //w = 12; h = 8;
        //terrain = new(w * h);
        //for (var i = 0; i < w; i++) {
        //    terrain[w * 4 + i] = true;
        //}
        //terrain[w * 5 + 5] = true;
        //terrain[w * 6 + 5] = true;
        //terrain[w * 7 + 6] = true;
        //terrain[w * 1 + 0] = true;
        //terrain[w * 2 + 0] = true;
        //terrain[w * 2 + 1] = true;
        //terrain[w * 2 + 2] = true;
        //terrain[w * 2 + 4] = true;
        //terrain[w * 1 + 9] = true;
        //terrain[w * 3 + 9] = true;
        //terrain[w * 4 + 9] = true;
        //terrain[w * 2 + 6] = true;
        //terrain[w * 3 + 6] = true;
        //terrain[w * 5 + 6] = true;
        //terrain[w * 4 + 7] = true;

        //ruins = [new(Point.Zero, 5), new(new(10, 0), 7), new(new(6, 4), 9)];
        //npcs = [new(new(9, 4), "ICE"), new(new(6, 6), "STONE"), new(new(6, 4), "ICE"), new(new(7, 3), "ICE")];
        //player = new(0, 4);
        //objs = [new(5, 6), new(6, 7), new(8, 3), new(11, 7), new(9, 1), new(6, 5), new(7, 4)];
        //final = new(11, 4);
        //maxscore = 30;
        //helptext = "";

        //level = new(2, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
        //LevelData.SaveLevel(level, "LEVELS/6.dat");
        ////
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        // Az NOK gomb a vissza / feltételezzük, hogy alap esetben már az elvárt menu van behúzva a menusbe
        switch (currentstate) {
            case GameState.MainMenu:
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    menu.Dispose();
                    switch (menu.BPressed) {
                        case Menu.ButtonPressed.Play:
                            if (LevelData.IsModified) {
                                menu = new(textureManager, Menu.MenuType.Warning, font);
                                menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                                menu.WarnText = "A programfájlokat módosították!\nMódosított fájlok:\n";
                                foreach (var item in LevelData.Modified) {
                                    menu.WarnText += item + "\n";
                                }
                                currentstate = GameState.warning;
                            }
                            else {
                                menu = new(textureManager, Menu.MenuType.LevelSelect, font);
                                menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                                currentstate = GameState.LevelSelect;
                            }
                            break;
                        case Menu.ButtonPressed.Options:
                            menu = new(textureManager, Menu.MenuType.Options, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.Options;
                            break;
                        default:
                            break;
                    }
                }
                menu.Update();
                break;
            case GameState.LevelSelect:
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    menu.Dispose();
                    switch (menu.BPressed) {
                        case Menu.ButtonPressed.OK:
                            MediaPlayer.Pause();
                            selLevel = menu.SelectedLevel;
                            currentstate = GameState.playing;
                            terrain = new(textureManager, audioManager, font, canvasSize, LevelData.Levels[selLevel]);
                            break;
                        case Menu.ButtonPressed.NOK:
                            menu = new(textureManager, Menu.MenuType.MainMenu, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.MainMenu;
                            menu.Update();
                            break;
                        default:
                            break;
                    }
                }
                else {
                    menu.Update();
                }
                break;
            case GameState.Options:
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    menu.Dispose();
                    switch (menu.BPressed) {
                        case Menu.ButtonPressed.OK:
                            menu.SaveSettings();
                            menu = new(textureManager, Menu.MenuType.MainMenu, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.MainMenu;
                            MediaPlayer.Volume = AppSettings.MusicVolume;
                            break;
                        case Menu.ButtonPressed.NOK:
                            menu = new(textureManager, Menu.MenuType.MainMenu, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.MainMenu;
                            break;
                        default:
                            break;
                    }
                }
                menu.Update();
                break;
            case GameState.Achievement:
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    menu.Dispose();
                    switch (menu.BPressed) {
                        case Menu.ButtonPressed.OK:
                            if (selLevel + 1 < LevelData.Levels.Length) {
                                MediaPlayer.Pause();
                                selLevel++;
                                currentstate = GameState.playing;
                                terrain = new(textureManager, audioManager, font, canvasSize, LevelData.Levels[selLevel]);
                            }
                            else {
                                menu = new(textureManager, Menu.MenuType.LevelSelect, font);
                                menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                                currentstate = GameState.LevelSelect;
                            }
                            break;
                        case Menu.ButtonPressed.NOK:
                            menu = new(textureManager, Menu.MenuType.LevelSelect, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.LevelSelect;
                            break;
                        default:
                            break;
                    }
                }
                else {
                    menu.Update();
                }
                break;
            case GameState.warning:
                if (menu.BPressed != Menu.ButtonPressed.NaN) {
                    menu.Dispose();
                    menu = new(textureManager, Menu.MenuType.LevelSelect, font);
                    menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                    currentstate = GameState.LevelSelect;
                }
                menu.Update();
                break;
            case GameState.playing:
                if (terrain.BPressed != Menu.ButtonPressed.NaN) {
                    terrain.Dispose();
                    switch (terrain.BPressed) {
                        case Menu.ButtonPressed.NOK:
                            MediaPlayer.Resume();
                            menu = new(textureManager, Menu.MenuType.LevelSelect, font);
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.LevelSelect;
                            break;
                        case Menu.ButtonPressed.OK:
                            MediaPlayer.Resume();
                            menu = new(textureManager, Menu.MenuType.Achievement, font) { Score = terrain.Score };
                            menu.Location = new(canvasSize.X / 2 - menu.Size.X / 2, canvasSize.Y / 2 - menu.Size.Y / 2);
                            currentstate = GameState.Achievement;
                            break;
                        default:
                            break;
                    }
                }
                terrain.Update();
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(textureManager.Backgrounds[bgimageindex], new Rectangle(0, 0, canvasSize.X, canvasSize.Y), Color.White);
        if (currentstate == GameState.playing) {
            terrain.Draw(_spriteBatch);
        }
        else {
            menu.Draw(_spriteBatch);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    //hangeffektek (commandokra pakolható) jobb lenne NPC-n belül kezelni
}
