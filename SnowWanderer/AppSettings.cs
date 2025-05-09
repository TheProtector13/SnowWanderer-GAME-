using System.IO;
using System.IO.Compression;
using System.Text;

namespace SnowWanderer {
    internal static class AppSettings {
        public static float MusicVolume { get; set; } = 0.4f;
        public static float SFXVolume { get; set; } = 0.7f;
        public static bool Fullscreen { get; set; } = false;

        static AppSettings()
        {
            if (File.Exists("SETTINGS.bin")) {
                using FileStream fileStream = new("SETTINGS.bin", FileMode.Open, FileAccess.Read, FileShare.None);
                using BrotliStream brotli = new(fileStream, CompressionMode.Decompress);
                using BinaryReader reader = new(brotli, Encoding.ASCII);
                MusicVolume = (float)reader.ReadDouble();
                SFXVolume = (float)reader.ReadDouble();
                Fullscreen = reader.ReadBoolean();
            }
            else {
                Save();
            }
        }

        public static void Save()
        {
            using FileStream fileStream = new("SETTINGS.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            using BrotliStream brotli = new(fileStream, CompressionLevel.SmallestSize);
            using BinaryWriter writer = new(brotli, Encoding.ASCII);
            writer.Write((double)MusicVolume);
            writer.Write((double)SFXVolume);
            writer.Write(Fullscreen);
        }

        public static void Init() { }
    }
}
