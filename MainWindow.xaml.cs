using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Euterpe.Audio;
using Euterpe.Models;
using Euterpe.Services;

namespace Euterpe
{
    public partial class MainWindow : Window
    {
        private readonly AudioPlayer _player = new();
        private List<Album> _allAlbums;
        private Image _currentCover;

        public MainWindow()
        {
            InitializeComponent();

            _allAlbums = LibraryLoader.LoadAlbums(@"D:\PLAYLIST");
            AlbumsGrid.ItemsSource = _allAlbums;
        }

        // BUSCA INSTANTÂNEA
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = SearchBox.Text.ToLower();

            AlbumsGrid.ItemsSource = _allAlbums
                .Where(a =>
                    a.Name.ToLower().Contains(text) ||
                    a.Artist.ToLower().Contains(text))
                .ToList();
        }

        // CLIQUE NO ÁLBUM
        private void Album_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var panel = sender as StackPanel;
            var album = panel?.DataContext as Album;
            if (album == null) return;

            var track = Directory.GetFiles(album.FolderPath)
                .FirstOrDefault(f =>
                    f.EndsWith(".flac") ||
                    f.EndsWith(".mp3") ||
                    f.EndsWith(".m4a"));

            if (track == null) return;

            _player.Play(track);

            StartCoverAnimation(panel);

            HistoryService.Save(
                album.Artist,
                album.Name,
                Path.GetFileName(track));
        }

        // ANIMAÇÃO DA CAPA
        private void StartCoverAnimation(StackPanel panel)
        {
            if (_currentCover != null)
            {
                _currentCover.RenderTransform.BeginAnimation(
                    System.Windows.Media.RotateTransform.AngleProperty, null);
            }

            _currentCover = panel.Children[0] as Image;

            var storyboard = (Storyboard)FindResource("RotateCover");
            storyboard.Begin(_currentCover, true);
        }
    }
}
