using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Euterpe.Audio;

namespace Euterpe
{
    public partial class AlbumTracksWindow : Window
    {
        private readonly AudioPlayer _player;
        private readonly List<string> _tracks;

        public AlbumTracksWindow(string albumPath, string albumName, AudioPlayer player)
        {
            InitializeComponent();

            _player = player;
            AlbumTitle.Text = albumName;

            _tracks = Directory.GetFiles(albumPath)
                .Where(f =>
                    f.EndsWith(".mp3") ||
                    f.EndsWith(".wav") ||
                    f.EndsWith(".flac") ||
                    f.EndsWith(".m4a"))
                .ToList();

            TracksList.ItemsSource =
                _tracks.Select(Path.GetFileNameWithoutExtension);
        }

        private void Track_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TracksList.SelectedIndex < 0)
                return;

            _player.LoadAlbum(Path.GetDirectoryName(_tracks[0])!);
            _player.Seek(0);

            // força tocar a faixa selecionada
            _player.PlaySpecific(TracksList.SelectedIndex);

            Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
