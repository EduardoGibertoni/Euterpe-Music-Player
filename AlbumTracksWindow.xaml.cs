using System;
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
        private Album _album;

        public AlbumTracksWindow(Album album, AudioPlayer player)
        {
            InitializeComponent();

            _player = player;
            _album = album;

            LoadAlbumUI();

            // Timer para atualizar apenas a barra de progresso e tempo
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        // Atualiza a janela para outro álbum
        public void UpdateAlbum(Album album)
        {
            _album = album;
            LoadAlbumUI();
        }

        // Carrega as informações visuais do álbum na janela
        private void LoadAlbumUI()
        {
            if (!string.IsNullOrEmpty(_album.CoverPath))
            {
                AlbumCover.Source =
                    new System.Windows.Media.Imaging.BitmapImage(
                        new Uri(_album.CoverPath));
            }

            // NÃO mostra o nome da música até o usuário clicar
            TrackNameText.Text = "";
            ArtistNameText.Text = _album.Artist;

            // Lista de faixas
            var tracks = System.IO.Directory.GetFiles(_album.FolderPath)
                    .Where(f => f.EndsWith(".mp3") || f.EndsWith(".wav") || f.EndsWith(".flac") || f.EndsWith(".m4a"))
                    .Select(System.IO.Path.GetFileNameWithoutExtension)
                    .ToArray();

            TracksList.ItemsSource = tracks;
        }

        // Atualiza barra de progresso e informações da música
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_player.Duration.TotalSeconds <= 0) return;

            if (!_isDragging)
            {
                ProgressSlider.Maximum = _player.Duration.TotalSeconds;
                ProgressSlider.Value = _player.Position.TotalSeconds;
            }

            CurrentTimeText.Text = FormatTime(_player.Position);
            TotalTimeText.Text = FormatTime(_player.Duration);

            // MOSTRA o nome da faixa apenas se o player estiver tocando uma música desse álbum
            if (_player.CurrentAlbum == _album)
                TrackNameText.Text = _player.CurrentTrackName;
        }

        // Novo método: clique em uma faixa
        private void TracksList_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TracksList.SelectedIndex < 0) return;

            var clickedTrackName = TracksList.SelectedItem.ToString() ?? "";

            // Se o player estiver tocando essa mesma faixa, reinicia
            if (_player.CurrentAlbum == _album &&
                _player.CurrentTrackName == clickedTrackName)
            {
                _player.Seek(0);
                _player.Play();
            }
            else
            {
                // Se o álbum atual do player for diferente, carrega o álbum
                if (_player.CurrentAlbum != _album)
                    _player.LoadAlbum(_album);

                // Toca a faixa clicada
                _player.PlayAt(TracksList.SelectedIndex);
            }

            // Atualiza o nome da música na janela do álbum
            TrackNameText.Text = _player.CurrentTrackName;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e) => _player.TogglePlayPause();
        private void Next_Click(object sender, RoutedEventArgs e) => _player.Next();
        private void Prev_Click(object sender, RoutedEventArgs e) => _player.Previous();

        private void ProgressSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => _isDragging = true;
        private void ProgressSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _player.Seek(ProgressSlider.Value);
            _isDragging = false;
        }

        private string FormatTime(TimeSpan time) => $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
    }
}
