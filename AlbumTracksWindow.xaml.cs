using System.IO;
using System.Linq;
using System.Windows;
using Euterpe.Audio;
using Euterpe.Models;

namespace Euterpe
{
    public partial class AlbumTracksWindow : Window
    {
        private readonly AudioPlayer _player;

        public AlbumTracksWindow(Album album, AudioPlayer player)
        {
            InitializeComponent();

            _player = player;

            var tracks = Directory.GetFiles(album.FolderPath)
                .Where(f =>
                    f.EndsWith(".mp3") ||
                    f.EndsWith(".wav") ||
                    f.EndsWith(".flac") ||
                    f.EndsWith(".m4a"))
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            TracksList.ItemsSource = tracks;
        }

        private void TracksList_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TracksList.SelectedIndex < 0)
                return;

            _player.PlayAt(TracksList.SelectedIndex);

            // 🚫 NÃO fecha a janela
            // Close(); ← REMOVIDO DE PROPÓSITO
        }
    }
}
