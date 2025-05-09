using System;
using System.Collections;
using Microsoft.Xna.Framework;

namespace SnowWanderer {
    internal readonly struct Level {
        public readonly byte bgimage;
        public readonly Int32 Width;
        public readonly Int32 Height;
        public readonly BitArray terrain;
        public readonly Tuple<Point, byte>[] ruins;
        public readonly Tuple<Point, string>[] npcs;
        public readonly Point playerLoc;
        public readonly Point[] objectives;
        public readonly Point finalObjective;
        public readonly Int32 MaxScore;
        public readonly string HelpText;

        public Level(byte bgimage, Int32 width, Int32 height, BitArray terrain, Tuple<Point, byte>[] ruins, Tuple<Point, string>[] npcs, Point playerLoc, Point[] objectives, Point finalObjective, Int32 maxscore, string helptext = "")
        {
            this.bgimage = bgimage;
            this.Width = width;
            this.Height = height;
            this.terrain = terrain ?? throw new ArgumentNullException(nameof(terrain));
            this.ruins = ruins ?? throw new ArgumentNullException(nameof(ruins));
            this.npcs = npcs ?? throw new ArgumentNullException(nameof(npcs));
            this.playerLoc = playerLoc;
            this.objectives = objectives ?? throw new ArgumentNullException(nameof(objectives));
            this.finalObjective = finalObjective;
            this.MaxScore = maxscore;
            this.HelpText = helptext ?? throw new ArgumentNullException(nameof(helptext));
        }

    }
}
