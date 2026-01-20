using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Euterpe.Audio;
using Euterpe.Models;
using Euterpe.Services;

namespace Euterpe
{
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer _player = new();
        private readonly DispatcherTimer _timer = new();
        private List<Album> _albums = new();
        private bool _dragging;

        public MainWindow()
        {
            InitializeComponent();

            var scanner = new AlbumScanner();
            _albums = scanner.Scan(@"D:\PLAYLIST");
            AlbumsGrid.ItemsSource = _albums;

            _player.TrackChanged += OnTrackChanged;

            _timer.Interval = TimeSpan.FromMilliseconds(300);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void OnTrackChanged()
        {
            Dispatcher.Invoke(() =>
            {
                TrackNameText.Text = _player.CurrentTrackName;
                ProgressSlider.Value = 0;
                ProgressSlider.Maximum = 1;
                TimeText.Text = "00:00 / 00:00";

                // Marca o álbum que está tocando
                foreach (var album in _albums)
                    album.IsPlaying = _player.CurrentTrackName.StartsWith(album.Name);
            });
        }

private void Album_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
{
    if (sender is FrameworkElement fe && fe.DataContext is Album album)
    {
        _player.LoadAlbum(album.FolderPath);
        TrackNameText.Text = _player.CurrentTrackName;

        if (!string.IsNullOrEmpty(album.CoverPath))
        {
            CurrentCover.Source =
                new System.Windows.Media.Imaging.BitmapImage(
                    new Uri(album.CoverPath));
        }

        var tracksWindow = new AlbumTracksWindow(album, _player);
        tracksWindow.Show();
    }
}


        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePlayPause();
        private void Next_Click(object sender, RoutedEventArgs e) => _player.Next();
        private void Prev_Click(object sender, RoutedEventArgs e) => _player.Previous();

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_player.Duration.TotalSeconds <= 0)
                return;

            if (ProgressSlider.Maximum != _player.Duration.TotalSeconds)
                ProgressSlider.Maximum = _player.Duration.TotalSeconds;

            if (_dragging)
                return;

            double position = _player.Position.TotalSeconds;
            double duration = _player.Duration.TotalSeconds;
            ProgressSlider.Value = Math.Min(position, duration);

            TimeText.Text =
                $"{TimeSpan.FromSeconds(position):mm\\:ss} / {TimeSpan.FromSeconds(duration):mm\\:ss}";
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }
        private void ProgressSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => _dragging = true;
        private void ProgressSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragging = false;
            _player.Seek(ProgressSlider.Value);
        }
    }
}