using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SnowWanderer {
    internal class AudioManager : IDisposable {
        public Song[] BGMusic { get; init; }
        public SoundEffect[] BGMusicAsSFX { get; init; }
        public SoundEffect Wind { get; init; }
        public SoundEffect[] RockSFX { get; init; }
        public SoundEffect Big_Punch { get; init; }
        public SoundEffect[] NPC { get; init; }
        public SoundEffect[] Steps { get; init; }
        public SoundEffect[] Collect { get; init; }
        public SoundEffect[] Place { get; init; }

        public AudioManager(ContentManager Content)
        {
            this.BGMusic = [Content.Load<Song>("SOUND/BGMUSIC/GoOn"),
                            Content.Load<Song>("SOUND/BGMUSIC/HB")];
            this.BGMusicAsSFX = [Content.Load<SoundEffect>("SOUND/WIND/GoOn"),
                                 Content.Load<SoundEffect>("SOUND/WIND/HB")];
            this.Wind = Content.Load<SoundEffect>("SOUND/WIND/WIND");
            this.RockSFX = [Content.Load<SoundEffect>("SOUND/ROCK/rock_breaking"),
                            Content.Load<SoundEffect>("SOUND/ROCK/rock_falling")];
            this.Big_Punch = Content.Load<SoundEffect>("SOUND/SFX/big_punch");
            this.NPC = [Content.Load<SoundEffect>("SOUND/SFX/blub_hurt"),
                        Content.Load<SoundEffect>("SOUND/SFX/blub_hurt2")];
            this.Steps = [Content.Load<SoundEffect>("SOUND/STEPS/snow_step_dry-01"),
                          Content.Load<SoundEffect>("SOUND/STEPS/snow_step_dry-02"),
                          Content.Load<SoundEffect>("SOUND/STEPS/snow_step_dry-03"),
                          Content.Load<SoundEffect>("SOUND/STEPS/snow_step_dry-04")];
            this.Collect = [Content.Load<SoundEffect>("SOUND/WOOD/wood_breaking_01"),
                            Content.Load<SoundEffect>("SOUND/WOOD/wood_breaking_02")];
            this.Place = [Content.Load<SoundEffect>("SOUND/WOOD/wood_falling_01"),
                          Content.Load<SoundEffect>("SOUND/WOOD/wood_falling_02")];
        }

        public void Dispose()
        {
            foreach (var song in BGMusic) {
                song.Dispose();
            }
            Wind.Dispose();
            foreach (var sfx in RockSFX) {
                sfx.Dispose();
            }
            Big_Punch.Dispose();
            foreach (var sfx in NPC) {
                sfx.Dispose();
            }
            foreach (var sfx in Steps) {
                sfx.Dispose();
            }
            foreach (var sfx in Collect) {
                sfx.Dispose();
            }
            foreach (var sfx in Place) {
                sfx.Dispose();
            }
        }
    }
}
