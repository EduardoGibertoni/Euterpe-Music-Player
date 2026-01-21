using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using Euterpe.Models;

namespace Euterpe.Audio
{
    public class AudioPlayer
    {
        private readonly MediaPlayer _player = new MediaPlayer();
        private string[] _playlist = Array.Empty<string>();
        private int _currentIndex;

        public bool IsPlaying { get; private set; }
        public Album? CurrentAlbum { get; private set; }

        public event Action? TrackChanged;

        public int CurrentIndex => _currentIndex;
        public TimeSpan Duration => _player.NaturalDuration.HasTimeSpan ? _player.NaturalDuration.TimeSpan : TimeSpan.Zero;
        public TimeSpan Position => _player.Position;
        public string CurrentTrackName => _playlist.Length > 0 ? Path.GetFileNameWithoutExtension(_playlist[_currentIndex]) : string.Empty;

        public AudioPlayer()
        {
            _player.MediaEnded += (s, e) => Next();
        }

        public void LoadAlbum(Album album)
        {
            CurrentAlbum = album; // ✅ Atribuição correta

            _playlist = Directory.GetFiles(album.FolderPath)
                .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".flac", StringComparison.OrdinalIgnoreCase)
                         || f.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            _currentIndex = 0;
        }

        public void Play()
        {
            _player.Play();
            IsPlaying = true;
        }

        public void Pause()
        {
            _player.Pause();
            IsPlaying = false;
        }

        public void TogglePlayPause()
        {
            if (IsPlaying) Pause();
            else Play();
        }

        public void Next()
        {
            if (_playlist.Length == 0) return;
            _currentIndex = (_currentIndex + 1) % _playlist.Length;
            PlayCurrent();
        }

        public void Previous()
        {
            if (_playlist.Length == 0) return;
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _playlist.Length - 1;
            PlayCurrent();
        }

        public void Seek(double seconds)
        {
            _player.Position = TimeSpan.FromSeconds(seconds);
        }

        public void PlayAt(int index)
        {
            if (index < 0 || index >= _playlist.Length) return;
            _currentIndex = index;
            PlayCurrent();
        }

        private void PlayCurrent()
        {
            if (_playlist.Length == 0) return;
            _player.Open(new Uri(_playlist[_currentIndex]));
            Play();
            TrackChanged?.Invoke();
        }
    }
}
