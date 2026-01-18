using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Euterpe.Audio;
using Euterpe.Services;

namespace Euterpe
{
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer player = new();
        private List<Album> albums;
        private List<string>? currentTracks;
        private int trackIndex;
        private bool isPlaying;
        private bool isDragging;

        public MainWindow()
        {
            InitializeComponent();

            albums = AlbumScanner.Scan(@"D:\PLAYLIST");
            AlbumGrid.ItemsSource = albums;

            player.ProgressChanged += (current, total) =>
            {
                if (isDragging) return;

                Dispatcher.Invoke(() =>
                {
                    ProgressSlider.Maximum = total;
                    ProgressSlider.Value = current;
                    TrackNameText.Text = player.CurrentFileName;
                });
            };
        }

        private void Album_Click(object sender, RoutedEventArgs e)
        {
            var album = (sender as Button)?.DataContext as Album;
            if (album == null) return;

            currentTracks = album.Tracks;
            trackIndex = 0;

            LoadTrack();
        }

        private void LoadTrack()
        {
            if (currentTracks == null || currentTracks.Count == 0) return;

            player.Load(currentTracks[trackIndex]);
            player.Play();
            isPlaying = true;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                player.Play();
                isPlaying = true;
            }
            else
            {
                player.Pause();
                isPlaying = false;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentTracks == null) return;

            trackIndex = (trackIndex + 1) % currentTracks.Count;
            LoadTrack();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (currentTracks == null) return;

            trackIndex = (trackIndex - 1 + currentTracks.Count) % currentTracks.Count;
            LoadTrack();
        }

        private void Slider_Down(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = true;
        }

        private void Slider_Up(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = false;
            player.Seek(ProgressSlider.Value);
        }
    }
}
