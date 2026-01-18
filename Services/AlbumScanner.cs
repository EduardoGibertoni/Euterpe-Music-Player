using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Euterpe.Services
{
    public class Album
    {
        public string Name { get; set; } = "";
        public string FolderPath { get; set; } = "";
        public List<string> Tracks { get; set; } = new();
        public string? CoverPath { get; set; }
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

            ScanFolderRecursive(root, albums);

            return albums;
        }

        private static void ScanFolderRecursive(string folder, List<Album> albums)
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
                    FolderPath = folder,
                    Tracks = audioFiles,
                    CoverPath = FindCover(folder)
                });

                return; // não desce mais, esse nível já é o álbum
            }

            foreach (var sub in Directory.GetDirectories(folder))
            {
                ScanFolderRecursive(sub, albums);
            }
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
