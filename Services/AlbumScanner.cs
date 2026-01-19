using Euterpe.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Euterpe.Services
{
    public class AlbumScanner
    {
        private static readonly string[] AudioExtensions =
        {
            ".mp3", ".flac", ".wav", ".m4a"
        };

        private static readonly string[] ImageExtensions =
        {
            ".jpg", ".jpeg", ".png"
        };

        public List<Album> Scan(string rootPath)
        {
            var albums = new List<Album>();

            if (!Directory.Exists(rootPath))
                return albums;

            ScanRecursive(rootPath, albums);

            return albums;
        }

        private void ScanRecursive(string folder, List<Album> albums)
        {
            // Verifica se a pasta contém músicas
            var audioFiles = Directory.GetFiles(folder)
                .Where(f => AudioExtensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();

            if (audioFiles.Any())
            {
                var cover = Directory.GetFiles(folder)
                    .FirstOrDefault(f =>
                        ImageExtensions.Contains(Path.GetExtension(f).ToLower()));

                albums.Add(new Album
                {
                    Name = Path.GetFileName(folder),
                    Artist = Directory.GetParent(folder)?.Name ?? "Desconhecido",
                    FolderPath = folder,
                    CoverPath = cover ?? string.Empty
                });

                return; // não precisa descer mais
            }

            // Continua descendo na árvore
            foreach (var sub in Directory.GetDirectories(folder))
            {
                ScanRecursive(sub, albums);
            }
        }
    }
}
