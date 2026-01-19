using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Euterpe.Models
{
    public class Album : INotifyPropertyChanged
    {
        private bool _isPlaying;

        public string Name { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
        public string CoverPath { get; set; } = string.Empty;

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
