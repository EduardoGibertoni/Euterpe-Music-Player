using System;
using System.IO;
using NAudio.Wave;
using System.Windows.Threading;

namespace Euterpe.Audio
{
    public class AudioPlayer
    {
        private WaveOutEvent? output;
        private AudioFileReader? reader;
        private DispatcherTimer timer;

        public event Action<double, double>? ProgressChanged;

        public string CurrentFileName =>
            reader != null ? Path.GetFileName(reader.FileName) : string.Empty;

        public AudioPlayer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (reader == null || reader.TotalTime.TotalSeconds <= 0)
                return;

            ProgressChanged?.Invoke(
                reader.CurrentTime.TotalSeconds,
                reader.TotalTime.TotalSeconds
            );
        }

        public void Load(string path)
        {
            Stop();

            reader = new AudioFileReader(path);
            output = new WaveOutEvent();
            output.Init(reader);
        }

        public void Play()
        {
            if (output == null) return;

            output.Play();
            timer.Start();
        }

        public void Pause()
        {
            output?.Pause();
            timer.Stop();
        }

        public void Stop()
        {
            timer.Stop();

            output?.Stop();
            reader?.Dispose();
            output?.Dispose();

            reader = null;
            output = null;
        }

        public void Seek(double seconds)
        {
            if (reader == null) return;

            reader.CurrentTime = TimeSpan.FromSeconds(seconds);
        }
    }
}
