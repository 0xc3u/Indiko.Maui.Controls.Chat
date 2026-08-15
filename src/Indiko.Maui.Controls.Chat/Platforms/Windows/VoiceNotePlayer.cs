using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using WindowsColor = Windows.UI.Color;

namespace Indiko.Maui.Controls.Chat.Platforms.Windows;

/// <summary>
/// Drives voice-note playback for a chat row: wires the play/pause button, a tap-to-seek
/// waveform, a progress timer and the elapsed/total duration label. One instance per row,
/// reconfigured per bound message. Mirrors the Android/iOS voice note players.
/// </summary>
internal sealed class VoiceNotePlayer
{
    private readonly Button _button;
    private readonly WaveformView _waveform;
    private readonly TextBlock _durationLabel;
    private readonly DispatcherTimer _timer;
    private readonly FontIcon _icon;

    private MediaPlayer _player;
    private TimeSpan _total;
    private WindowsColor _iconColor = WindowsColor.FromArgb(255, 255, 255, 255);

    public VoiceNotePlayer(Button button, WaveformView waveform, TextBlock durationLabel)
    {
        _button = button;
        _waveform = waveform;
        _durationLabel = durationLabel;

        _icon = new FontIcon
        {
            Glyph = "\uE768",
            FontSize = 14,
            Foreground = new SolidColorBrush(_iconColor),
        };
        if (_button != null)
            _button.Content = _icon;

        if (_button != null)
            _button.Click += OnButtonClick;
        if (_waveform != null)
            _waveform.Tapped += OnWaveformTapped;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _timer.Tick += (s, e) => Tick();
    }

    public void Configure(string filePath, TimeSpan? duration, WindowsColor iconColor)
    {
        Stop();
        Release();

        _iconColor = iconColor;
        _icon.Foreground = new SolidColorBrush(iconColor);
        _total = duration ?? TimeSpan.Zero;

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                _player = new MediaPlayer
                {
                    Source = MediaSource.CreateFromUri(new Uri(filePath, UriKind.Absolute)),
                    AutoPlay = false,
                };
                _player.MediaEnded += OnMediaEnded;
                _player.MediaOpened += OnMediaOpened;
            }
            catch
            {
                _player?.Dispose();
                _player = null;
            }
        }

        SetIcon(playing: false);
        _waveform?.SetProgress(0);
        SetLabel(_total);
    }

    public void Stop()
    {
        _timer.Stop();
        if (_player != null)
        {
            try
            {
                if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                    _player.Pause();
                _player.PlaybackSession.Position = TimeSpan.Zero;
            }
            catch { /* player may not be fully opened yet */ }
        }
        _waveform?.SetProgress(0);
        SetIcon(playing: false);
        if (_total > TimeSpan.Zero)
            SetLabel(_total);
    }

    public void Release()
    {
        _timer.Stop();
        if (_player != null)
        {
            try { _player.MediaEnded -= OnMediaEnded; } catch { /* ignore */ }
            try { _player.MediaOpened -= OnMediaOpened; } catch { /* ignore */ }
            _player.Dispose();
            _player = null;
        }
    }

    private void OnMediaOpened(MediaPlayer sender, object args)
    {
        if (_total <= TimeSpan.Zero && _player != null)
            _total = _player.PlaybackSession.NaturalDuration;
        SetLabel(_total);
    }

    private void OnMediaEnded(MediaPlayer sender, object args) => Stop();

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (_player == null)
            return;

        try
        {
            if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                _player.Pause();
                _timer.Stop();
                SetIcon(playing: false);
            }
            else
            {
                _player.Play();
                SetIcon(playing: true);
                _timer.Start();
            }
        }
        catch { /* not opened yet */ }
    }

    private void OnWaveformTapped(object sender, TappedRoutedEventArgs e)
    {
        if (_player == null || _waveform == null || _waveform.ActualWidth <= 0 || _total <= TimeSpan.Zero)
            return;

        var x = e.GetPosition(_waveform).X;
        var progress = Math.Clamp(x / _waveform.ActualWidth, 0, 1);
        try
        {
            _player.PlaybackSession.Position = TimeSpan.FromSeconds(progress * _total.TotalSeconds);
            _waveform.SetProgress((float)progress);
            SetLabel(_player.PlaybackSession.Position);
        }
        catch { /* not seekable yet */ }
    }

    private void Tick()
    {
        if (_player == null || _player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            return;

        var position = _player.PlaybackSession.Position;
        var totalMs = _total.TotalMilliseconds > 0 ? _total.TotalMilliseconds : position.TotalMilliseconds;
        _waveform?.SetProgress(totalMs > 0 ? (float)(position.TotalMilliseconds / totalMs) : 0);
        SetLabel(position);
    }

    private void SetLabel(TimeSpan value) => _durationLabel.Text = AudioWaveformHelper.Format(value);

    private void SetIcon(bool playing)
    {
        _icon.Glyph = playing ? "\uE769" : "\uE768";
    }
}
