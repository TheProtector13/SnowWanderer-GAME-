using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SnowWanderer {
    internal class NPC : IGraphicObject {
        private readonly Texture2D[][] moveTexture; //Up, Down, Left, Right
        private readonly Texture2D[] idleTexture;
        private readonly Dictionary<Actions, Texture2D[]> eventTexture = [];
        private readonly Dictionary<Actions, SoundEffectInstance?[]> eventSounds = [];
        private readonly AudioManager audioManager;
        private Int16 callCount = 0, moveCount = 0, reachTargetIn = 60;
        private Point moveTarget = Point.Zero;
        private Point realLocation = Point.Zero;
        private readonly bool oneSideWalk = false;
        private Texture2D[] currentTexture;
        public Actions CurrentAction { get; private set; } = Actions.Idle;
        public Directions CurrentDirection { get; private set; } = Directions.Right;
        /// <summary>
        /// Must be the middle bottom of the object.
        /// </summary>
        public Point Location { get; set; } = Point.Zero;
        public Point Size { get; set; } = new(64, 64);
        public Point Margin { get; set; } = Point.Zero;
        public static Point DefSize { get; } = new(64, 64);
        /// <summary>
        /// X = Top, Y = Bottom
        /// </summary>
        public static Point DefMargin { get; } = Point.Zero;
        /// <summary>
        /// Frame changing interval. Default is 2.
        /// </summary>
        public Int16 FrameRate { get; set; } = 5;
        public Int16 CurrentFrame { get; private set; } = 0;
        public Int16 ID { get; init; } = 0;

        public enum Actions {
            Idle,
            Move,
            Die,
            Wake,
            Wait,
            Pickup
        }

        public enum Directions {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Creates a new NPC object with the given textures.
        /// </summary>
        public NPC(AudioManager audioManager, Texture2D[] moveup, Texture2D[] movedown, Texture2D[] moveleft, Texture2D[] moveright, Texture2D[] idle, Texture2D[][] events, Actions[] eventNames, bool movesound = false)
        {
            this.audioManager = audioManager;
            this.moveTexture = [moveup, movedown, moveleft, moveright];
            if (moveup.Length == 0) {
                oneSideWalk = true;
            }
            if (eventNames.Length != events.Length) {
                throw new ArgumentException("The eventNames and events arrays must have the same length, and the eventNames cannot have repeated values!");
            }
            for (int i = 0; i < eventNames.Length; i++) {
                eventTexture.Add(eventNames[i], events[i]);
            }
            if (idle.Length == 0) {
                this.idleTexture = [moveright[0]];
            }
            else {
                this.idleTexture = idle;
            }
            eventTexture.Add(Actions.Wait, this.idleTexture);
            eventSounds.Add(Actions.Wait, [null]);
            if (movesound) {
                eventSounds.Add(Actions.Move, [audioManager.Steps[0].CreateInstance(),
                                               audioManager.Steps[1].CreateInstance(),
                                               audioManager.Steps[2].CreateInstance(),
                                               audioManager.Steps[3].CreateInstance()]);
            }
            else {
                eventSounds.Add(Actions.Move, [null]);
            }
            eventSounds.Add(Actions.Pickup, [null]);
            eventSounds.Add(Actions.Die, [audioManager.NPC[0].CreateInstance(), audioManager.NPC[1].CreateInstance()]);
            eventSounds.Add(Actions.Wake, [audioManager.RockSFX[0].CreateInstance(), audioManager.RockSFX[1].CreateInstance()]);
            eventSounds.Add(Actions.Idle, [null]);
            currentTexture = idleTexture;
        }

        public NPC(AudioManager audioManager, Texture2D[] moveup, Texture2D[] movedown, Texture2D[] moveleft, Texture2D[] moveright, Texture2D[] idle, Texture2D[][] events, Actions[] eventNames, Int16 id, bool movesound = false)
        {
            this.audioManager = audioManager;
            this.moveTexture = [moveup, movedown, moveleft, moveright];
            if (moveup.Length == 0) {
                oneSideWalk = true;
            }
            if (eventNames.Length != events.Length) {
                throw new ArgumentException("The eventNames and events arrays must have the same length, and the eventNames cannot have repeated values!");
            }
            for (int i = 0; i < eventNames.Length; i++) {
                if (!eventTexture.ContainsKey(eventNames[i])) {
                    throw new ArgumentException("The eventNames and events arrays must have the same length, and the eventNames cannot have repeated values!");
                }
                eventTexture.Add(eventNames[i], events[i]);
            }
            if (idle.Length == 0) {
                this.idleTexture = [moveright[0]];
            }
            else {
                this.idleTexture = idle;
            }
            eventTexture.Add(Actions.Wait, this.idleTexture);
            eventSounds.Add(Actions.Wait, [null]);
            if (movesound) {
                eventSounds.Add(Actions.Move, [audioManager.Steps[0].CreateInstance(),
                                               audioManager.Steps[1].CreateInstance(),
                                               audioManager.Steps[2].CreateInstance(),
                                               audioManager.Steps[3].CreateInstance()]);
            }
            else {
                eventSounds.Add(Actions.Move, [null]);
            }
            eventSounds.Add(Actions.Pickup, [null]);
            eventSounds.Add(Actions.Die, [audioManager.NPC[0].CreateInstance(), audioManager.NPC[1].CreateInstance()]);
            eventSounds.Add(Actions.Wake, [audioManager.RockSFX[0].CreateInstance(), audioManager.RockSFX[1].CreateInstance()]);
            eventSounds.Add(Actions.Idle, [null]);
            currentTexture = idleTexture;
            this.ID = id;
        }

        /// <summary>
        /// Returns the current frame of the animation,
        /// and increments the frame counter. (IF the input is true! By default false.)
        /// </summary>
        public Texture2D GetTexture(bool increment = false)
        {
            if (increment) {
                callCount++;
                if (callCount > FrameRate) {
                    callCount = 0;
                    CurrentFrame++;
                    if (CurrentFrame >= currentTexture.Length) {
                        CurrentFrame = 0;
                    }
                }
            }
            return currentTexture[CurrentFrame];
        }

        public void SetAction(Actions action)
        {
            if (action == Actions.Move) {
                throw new ArgumentException("Use SetAction(Point target) for moving the NPC!");
            }
            CurrentAction = action;
            CurrentFrame = 0;
            currentTexture = eventTexture[action];
        }

        public void SetAction(Point target, ref Point position)
        {
            moveTarget = target;
            CurrentAction = Actions.Move;
            CurrentFrame = 0;
            if (target.X < Location.X) {
                if (oneSideWalk) {
                    currentTexture = moveTexture[3];
                }
                else {
                    currentTexture = moveTexture[2];
                }
                CurrentDirection = Directions.Left;
                position.X--;
            }
            else if (target.X > Location.X) {
                currentTexture = moveTexture[3];
                CurrentDirection = Directions.Right;
                position.X++;
            }
            else if (target.Y < Location.Y) {
                if (oneSideWalk) {
                    currentTexture = moveTexture[3];
                }
                else {
                    currentTexture = moveTexture[0];
                }
                CurrentDirection = Directions.Up;
                position.Y--;
            }
            else {
                if (oneSideWalk) {
                    currentTexture = moveTexture[3];
                }
                else {
                    currentTexture = moveTexture[1];
                }
                CurrentDirection = Directions.Down;
                position.Y++;
            }
        }

        /// <summary>
        /// Draws the block, with the correct frame of the animation.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (oneSideWalk && CurrentAction == Actions.Move && CurrentDirection == Directions.Left) {
                spriteBatch.Draw(currentTexture[CurrentFrame], new Rectangle(realLocation, Size), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
            else {
                spriteBatch.Draw(currentTexture[CurrentFrame], new Rectangle(realLocation, Size), Color.White);
            }
        }

        public void Update()
        {
            if (CurrentAction == Actions.Move) {
                Point begining = new(Location.X - Size.X / 2 + Margin.X, Location.Y - Size.Y + Margin.Y);
                Point target = new(moveTarget.X - Size.X / 2 + Margin.X, moveTarget.Y - Size.Y + Margin.Y);
                realLocation = new((Int32)(begining.X + (target.X - (double)begining.X) / reachTargetIn * moveCount),
                                   (Int32)(begining.Y + (target.Y - (double)begining.Y) / reachTargetIn * moveCount));
                if (moveCount >= reachTargetIn) {
                    CurrentAction = Actions.Idle;
                    currentTexture = idleTexture;
                    moveCount = 0;
                    CurrentFrame = 0;
                    Location = moveTarget;
                }
                else { moveCount++; }
            }
            else {
                realLocation = new(Location.X - Size.X / 2 + Margin.X, Location.Y - Size.Y + Margin.Y);
            }
            callCount++;
            if (callCount > FrameRate) {
                callCount = 0;
                CurrentFrame++;
                if (CurrentAction == Actions.Move && eventSounds[Actions.Move][0] != null && CurrentFrame % 4 == 0) {
                    int num = Random.Shared.Next(0, 4);
                    eventSounds[Actions.Move][num].Volume = AppSettings.SFXVolume;
                    eventSounds[Actions.Move][num].Play();
                }
                if (CurrentFrame >= currentTexture.Length) {
                    if (CurrentAction != Actions.Move) {
                        if (eventSounds[CurrentAction][0] != null) {
                            int num = Random.Shared.Next(0, eventSounds[CurrentAction].Length);
                            eventSounds[CurrentAction][num].Volume = AppSettings.SFXVolume;
                            eventSounds[CurrentAction][num].Play();
                        }
                        CurrentAction = Actions.Idle;
                        currentTexture = idleTexture;
                    }
                    CurrentFrame = 0;
                }
            }
        }
    }
}
