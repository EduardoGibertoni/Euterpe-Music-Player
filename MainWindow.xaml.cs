using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Forms;
using Euterpe.Audio;
using Euterpe.Models;
using Euterpe.Services;

namespace Euterpe
{
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer _player = new AudioPlayer();
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private List<Album> _albums = new List<Album>();
        private bool _dragging;

        private readonly string settingsFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastfolder.txt");

        public MainWindow()
        {
            InitializeComponent();

            string? folder = LoadFolder();

            if (string.IsNullOrEmpty(folder) || !System.IO.Directory.Exists(folder))
            {
                folder = AskForFolder();
            }

            if (!string.IsNullOrEmpty(folder))
                LoadMusicFolder(folder);

            _player.TrackChanged += OnTrackChanged;

            _timer.Interval = TimeSpan.FromMilliseconds(300);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void LoadMusicFolder(string folderPath)
        {
            var scanner = new AlbumScanner();
            _albums = scanner.Scan(folderPath);
            AlbumsGrid.ItemsSource = _albums;

            // Limpa informações do player
            TrackNameText.Text = "";
            CurrentCover.Source = null;

            SaveFolder(folderPath);
        }

        private string? AskForFolder()
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "Selecione a pasta de músicas";
            var result = dialog.ShowDialog();
            return result == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
        }

        private void ChangeFolder_Click(object sender, RoutedEventArgs e)
        {
            string? folder = AskForFolder();
            if (!string.IsNullOrEmpty(folder))
                LoadMusicFolder(folder);
        }

        private void SaveFolder(string folder)
        {
            try { System.IO.File.WriteAllText(settingsFile, folder); } catch { }
        }

        private string? LoadFolder()
        {
            try
            {
                if (System.IO.File.Exists(settingsFile))
                    return System.IO.File.ReadAllText(settingsFile);
            }
            catch { }
            return null;
        }

        private void OnTrackChanged()
        {
            Dispatcher.Invoke(() =>
            {
                TrackNameText.Text = _player.CurrentTrackName;

                if (_player.CurrentAlbum != null && !string.IsNullOrEmpty(_player.CurrentAlbum.CoverPath))
                    CurrentCover.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_player.CurrentAlbum.CoverPath));

                foreach (var album in _albums)
                    album.IsPlaying = _player.CurrentAlbum == album;
            });
        }

        private void Album_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Album album)
            {
                var window = System.Windows.Application.Current.Windows
                                .OfType<AlbumTracksWindow>()
                                .FirstOrDefault();

                if (window != null)
                    window.UpdateAlbum(album);
                else
                {
                    var tracksWindow = new AlbumTracksWindow(album, _player);
                    tracksWindow.Show();
                }
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePlayPause();
        private void Next_Click(object sender, RoutedEventArgs e) => _player.Next();
        private void Prev_Click(object sender, RoutedEventArgs e) => _player.Previous();

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_player.Duration.TotalSeconds <= 0 || _dragging) return;

            ProgressSlider.Maximum = _player.Duration.TotalSeconds;
            ProgressSlider.Value = Math.Min(_player.Position.TotalSeconds, _player.Duration.TotalSeconds);
            TimeText.Text = $"{TimeSpan.FromSeconds(_player.Position.TotalSeconds):mm\\:ss} / {TimeSpan.FromSeconds(_player.Duration.TotalSeconds):mm\\:ss}";
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) { }
        private void ProgressSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _dragging = true;
        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;
            _player.Seek(ProgressSlider.Value);
        }
    }
}
