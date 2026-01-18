using System;

namespace Euterpe.Models
{
    public class PlayHistoryItem
    {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Track { get; set; }
        public DateTime PlayedAt { get; set; }
    }
}
