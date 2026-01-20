using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Euterpe.Audio;
using Euterpe.Models;

namespace Euterpe
{
    public partial class AlbumTracksWindow : Window
    {
        private readonly AudioPlayer _player;
        private readonly DispatcherTimer _timer;
        private bool _isDragging;
        private readonly Album _album;

        public AlbumTracksWindow(Album album, AudioPlayer player)
        {
            InitializeComponent();

            _album = album;
            _player = player;

            // capa do álbum
            if (!string.IsNullOrEmpty(album.CoverPath))
            {
                AlbumCover.Source =
                    new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(album.CoverPath));
            }

            // lista de faixas
            var tracks = Directory.GetFiles(album.FolderPath)
                .Where(f =>
                    f.EndsWith(".mp3") ||
                    f.EndsWith(".wav") ||
                    f.EndsWith(".flac") ||
                    f.EndsWith(".m4a"))
                .ToList();

            TracksList.ItemsSource = tracks
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            _timer.Tick += (s, e) =>
            {
                if (_player.Duration.TotalSeconds <= 0)
                    return;

                if (!_isDragging)
                {
                    ProgressSlider.Maximum = _player.Duration.TotalSeconds;
                    ProgressSlider.Value = _player.Position.TotalSeconds;
                }

                CurrentTimeText.Text = FormatTime(_player.Position);
                TotalTimeText.Text = FormatTime(_player.Duration);

                TrackNameText.Text = _player.CurrentTrackName;
                ArtistNameText.Text = _album.Artist; // ✅ NOME CORRETO
            };

            _timer.Start();
        }

        private void TracksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TracksList.SelectedIndex < 0)
                return;

            _player.PlayAt(TracksList.SelectedIndex);
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            _player.TogglePlayPause();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _player.Next();
            TracksList.SelectedIndex = _player.CurrentIndex;
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            _player.Previous();
            TracksList.SelectedIndex = _player.CurrentIndex;
        }

        private void ProgressSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
        }

        private void ProgressSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _player.Seek(ProgressSlider.Value);
            _isDragging = false;
        }

        private string FormatTime(TimeSpan time)
        {
            return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        }
    }
}
