using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Hashing;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace SnowWanderer {
    static class LevelData {
        private static readonly ManualResetEventSlim loaded = new(false);
        private static readonly string[] Hashes = ["7F3CA93E13322318", "A12C51EE77880ED6", "5DA5A8A88B2EC074",
            "70F6D2CEB31EC4FD", "80F4FA53C0744C9E", "5513C6376EE9E9B6"]; //hash codes to compare
        public static readonly Level[] Levels;
        public static bool IsModified { get; private set; } = false;
        public static List<string> Modified { get; private set; } = [];

        static LevelData()
        {
            if (!Directory.Exists("LEVELS")) {
                Directory.CreateDirectory("LEVELS");
            }
            string[] files = Directory.GetFiles("LEVELS");
            Levels = new Level[files.Length];
            if (Hashes.Length != files.Length) {
                IsModified = true;
            }
            for (int i = 0; i < files.Length; i++) {
                using (FileStream filestream = new(files[i], FileMode.Open, FileAccess.Read, FileShare.None)) {
                    using BrotliStream brotli = new(filestream, CompressionMode.Decompress);
                    using BinaryReader reader = new(brotli, Encoding.UTF8);
                    byte bgimage = reader.ReadByte();
                    Int32 w = reader.Read7BitEncodedInt();
                    Int32 h = reader.Read7BitEncodedInt();
                    Int32 ruincount = reader.Read7BitEncodedInt();
                    Tuple<Point, byte>[] ruins = new Tuple<Point, byte>[ruincount];
                    for (int j = 0; j < ruincount; j++) {
                        ruins[j] = new(new(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt()), reader.ReadByte());
                    }
                    Int32 npccount = reader.Read7BitEncodedInt();
                    Tuple<Point, string>[] npcs = new Tuple<Point, string>[npccount];
                    for (int j = 0; j < npccount; j++) {
                        npcs[j] = new(new(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt()), reader.ReadString());
                    }
                    Point player = new(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt());
                    Int32 objcount = reader.Read7BitEncodedInt();
                    Point[] objs = new Point[objcount];
                    for (int j = 0; j < objcount; j++) {
                        objs[j] = new(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt());
                    }
                    Point final = new(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt());
                    //terrain feltöltése
                    byte[] byteArray = reader.ReadBytes((Int32)Math.Ceiling((w * h) / 8.0));
                    BitArray terrain = new(byteArray);
                    Int32 maxscore = reader.Read7BitEncodedInt();
                    string helptext = reader.ReadString() ?? "";
                    Levels[i] = new(bgimage, w, h, terrain, ruins, npcs, player, objs, final, maxscore, helptext);
                }
                if (!IsModified) {
                    using FileStream filestream = new(files[i], FileMode.Open, FileAccess.Read, FileShare.None);
                    XxHash3 HashAlgo = new();
                    HashAlgo.Append(filestream);
                    string hash = BitConverter.ToString(HashAlgo.GetHashAndReset()).Replace("-", "");
                    if (!hash.Equals(Hashes[i])) {
                        IsModified = true;
                        lock (Modified) {
                            Modified.Add("LEVELS\\" + Path.GetFileName(files[i]));
                        }
                    }
                }
            }
            loaded.Set();
        }

        public static void SaveLevel(Level level, string path)
        {
            loaded.Wait();
            using (FileStream filestream = new(path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using BrotliStream brotli = new(filestream, CompressionLevel.SmallestSize);
                using BinaryWriter writer = new(brotli, Encoding.UTF8);
                writer.Write(level.bgimage);
                writer.Write7BitEncodedInt(level.Width);
                writer.Write7BitEncodedInt(level.Height);
                writer.Write7BitEncodedInt(level.ruins.Length);
                foreach (Tuple<Point, byte> element in level.ruins) {
                    writer.Write7BitEncodedInt(element.Item1.X);
                    writer.Write7BitEncodedInt(element.Item1.Y);
                    writer.Write(element.Item2);
                }
                writer.Write7BitEncodedInt(level.npcs.Length);
                foreach (Tuple<Point, string> element in level.npcs) {
                    writer.Write7BitEncodedInt(element.Item1.X);
                    writer.Write7BitEncodedInt(element.Item1.Y);
                    writer.Write(element.Item2);
                }
                writer.Write7BitEncodedInt(level.playerLoc.X);
                writer.Write7BitEncodedInt(level.playerLoc.Y);
                writer.Write7BitEncodedInt(level.objectives.Length);
                foreach (Point element in level.objectives) {
                    writer.Write7BitEncodedInt(element.X);
                    writer.Write7BitEncodedInt(element.Y);
                }
                writer.Write7BitEncodedInt(level.finalObjective.X);
                writer.Write7BitEncodedInt(level.finalObjective.Y);
                byte[] byteArray = new byte[(Int32)Math.Ceiling((level.Width * level.Height) / 8.0)];
                level.terrain.CopyTo(byteArray, 0);
                writer.Write(byteArray);
                writer.Write7BitEncodedInt(level.MaxScore);
                writer.Write(level.HelpText);
                writer.Flush();
                brotli.Flush();
            }
            using (FileStream filestream = new(path, FileMode.Open, FileAccess.Read, FileShare.None)) {
                XxHash3 HashAlgo = new();
                HashAlgo.Append(filestream);
                byte[] bytehash = HashAlgo.GetCurrentHash();
                string hash = BitConverter.ToString(bytehash).Replace("-", "");
                Debug.WriteLine(hash);
            }
        }

        public static void Init() { }
    }
}
