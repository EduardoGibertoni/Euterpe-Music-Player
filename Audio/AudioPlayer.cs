using System;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Euterpe.Audio
{
    public class AudioPlayer
    {
        private readonly MediaPlayer _player = new();
        private string[] _playlist = Array.Empty<string>();
        private int _currentIndex;

        public bool IsPlaying { get; private set; }

        public event Action? TrackChanged;

        public TimeSpan Duration =>
            _player.NaturalDuration.HasTimeSpan
                ? _player.NaturalDuration.TimeSpan
                : TimeSpan.Zero;

        public TimeSpan Position => _player.Position;

        public string CurrentTrackName { get; private set; } = string.Empty;

        public AudioPlayer()
        {
            _player.MediaEnded += (s, e) => Next();

            _player.MediaOpened += (s, e) =>
            {
                if (_playlist.Length == 0) return;

                CurrentTrackName = Path.GetFileNameWithoutExtension(_playlist[_currentIndex]);
                TrackChanged?.Invoke();
            };
        }

        public void LoadAlbum(string folderPath)
        {
            _playlist = Directory.GetFiles(folderPath)
                .Where(f =>
                    f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".flac", StringComparison.OrdinalIgnoreCase) ||
                    f.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f)
                .ToArray();

            _currentIndex = 0;

            if (_playlist.Length > 0)
                PlayCurrent();
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
            if (IsPlaying)
                Pause();
            else
                Play();
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
            if (_currentIndex < 0)
                _currentIndex = _playlist.Length - 1;

            PlayCurrent();
        }

        public void Seek(double seconds)
        {
            _player.Position = TimeSpan.FromSeconds(seconds);
        }

        private void PlayCurrent()
        {
            _player.Stop();
            _player.Open(new Uri(_playlist[_currentIndex], UriKind.Absolute));
            Play();
        }
    }
}
