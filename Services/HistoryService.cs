using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Euterpe.Models;

namespace Euterpe.Services
{
    public static class HistoryService
    {
        private const string FileName = "history.json";

        public static void Save(string artist, string album, string track)
        {
            var history = Load();

            history.Insert(0, new PlayHistoryItem
            {
                Artist = artist,
                Album = album,
                Track = track,
                PlayedAt = System.DateTime.Now
            });

            File.WriteAllText(FileName,
                JsonSerializer.Serialize(history, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
        }

        public static List<PlayHistoryItem> Load()
        {
            if (!File.Exists(FileName))
                return new List<PlayHistoryItem>();

            return JsonSerializer.Deserialize<List<PlayHistoryItem>>(
                File.ReadAllText(FileName));
        }
    }
}
