using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Euterpe.Services
{
    public class PlaylistManager
    {
        public List<string> Tracks { get; } = new();
        private int index = 0;

        public PlaylistManager(string root)
        {
            var extensions = new[] { ".mp3", ".flac", ".wav", ".m4a" };

            Tracks = Directory
                .GetFiles(root, "*.*", SearchOption.AllDirectories)
                .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                .ToList();
        }

        public string? Current =>
            Tracks.Count > 0 ? Tracks[index] : null;

        public string? Next()
        {
            if (Tracks.Count == 0) return null;
            index = (index + 1) % Tracks.Count;
            return Current;
        }

        public string? Previous()
        {
            if (Tracks.Count == 0) return null;
            index = (index - 1 + Tracks.Count) % Tracks.Count;
            return Current;
        }
    }
}
