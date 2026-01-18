using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Euterpe.Services
{
    public class Album
    {
        public string Name { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Year { get; set; } = "";
        public string FolderPath { get; set; } = "";
        public List<string> Tracks { get; set; } = new();
        public string? CoverPath { get; set; }

        // usado para destacar o álbum tocando
        public bool IsPlaying { get; set; }
    }

    public static class AlbumScanner
    {
        private static readonly string[] AudioExtensions =
            { ".mp3", ".flac", ".wav", ".m4a" };

        private static readonly string[] CoverNames =
            { "cover.jpg", "folder.jpg", "front.jpg", "cover.png", "folder.png" };

        public static List<Album> Scan(string root)
        {
            var albums = new List<Album>();
            ScanRecursive(root, albums);
            return albums;
        }

        private static void ScanRecursive(string folder, List<Album> albums)
        {
            var audioFiles = Directory
                .GetFiles(folder)
                .Where(f => AudioExtensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();

            if (audioFiles.Any())
            {
                albums.Add(new Album
                {
                    Name = Path.GetFileName(folder),
                    Artist = GetArtist(folder),
                    Year = ExtractYear(Path.GetFileName(folder)),
                    FolderPath = folder,
                    Tracks = audioFiles,
                    CoverPath = FindCover(folder)
                });
                return;
            }

            foreach (var sub in Directory.GetDirectories(folder))
                ScanRecursive(sub, albums);
        }

        private static string GetArtist(string albumFolder)
        {
            var parent = Directory.GetParent(albumFolder);
            return parent != null ? parent.Name : "Unknown Artist";
        }

        private static string ExtractYear(string name)
        {
            var match = Regex.Match(name, @"(19|20)\d{2}");
            return match.Success ? match.Value : "";
        }

        private static string? FindCover(string folder)
        {
            foreach (var name in CoverNames)
            {
                var path = Path.Combine(folder, name);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }
    }
}
