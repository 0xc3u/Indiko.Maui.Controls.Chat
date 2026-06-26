using Android.Media;
using Android.OS;
using Android.Views;
using AView = Android.Views.View;
using Color = Android.Graphics.Color;
using ImageButton = Android.Widget.ImageButton;
using TextView = Android.Widget.TextView;

namespace Indiko.Maui.Controls.Chat.Platforms.Android;

/// <summary>
/// Drives voice-note playback for a chat row: wires the play/pause button, a tap-to-seek
/// waveform, a progress poller and the elapsed/total duration label. One instance per
/// view holder, reconfigured per bound message.
/// </summary>
public sealed class VoiceNotePlayer : Java.Lang.Object
{
    private readonly ImageButton _button;
    private readonly WaveformView _waveform;
    private readonly TextView _durationLabel;
    private readonly Handler _handler;

    private MediaPlayer _player;
    private TimeSpan _total;
    private Color _iconColor = Color.White;
    private Action _poll;

    public VoiceNotePlayer(ImageButton button, WaveformView waveform, TextView durationLabel)
    {
        _button = button;
        _waveform = waveform;
        _durationLabel = durationLabel;
        _handler = new Handler(Looper.MainLooper);

        _button.Click += OnButtonClick;
        _waveform.Touch += OnWaveformTouch;
    }

    public void Configure(string filePath, TimeSpan? duration, Color iconColor)
    {
        Stop();
        Release();

        _iconColor = iconColor;
        _total = duration ?? TimeSpan.Zero;

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try
            {
                _player = new MediaPlayer();
                _player.SetDataSource(filePath);
                _player.Prepared += (s, e) =>
                {
                    if (_total <= TimeSpan.Zero && _player != null)
                    {
                        _total = TimeSpan.FromMilliseconds(_player.Duration);
                        _durationLabel.Text = AudioWaveformHelper.Format(_total);
                    }
                };
                _player.Completion += (s, e) => Stop();
                _player.PrepareAsync();
            }
            catch
            {
                _player = null;
            }
        }

        SetIcon(playing: false);
        _waveform.SetProgress(0);
        _durationLabel.Text = AudioWaveformHelper.Format(_total);
    }

    public void Stop()
    {
        _handler.RemoveCallbacksAndMessages(null);
        if (_player != null)
        {
            try
            {
                if (_player.IsPlaying) _player.Pause();
                _player.SeekTo(0);
            }
            catch { /* player may not be prepared yet */ }
        }
        _waveform.SetProgress(0);
        SetIcon(playing: false);
        if (_total > TimeSpan.Zero)
            _durationLabel.Text = AudioWaveformHelper.Format(_total);
    }

    public void Release()
    {
        _handler.RemoveCallbacksAndMessages(null);
        if (_player != null)
        {
            try { _player.Release(); } catch { }
            _player.Dispose();
            _player = null;
        }
    }

    private void OnButtonClick(object sender, EventArgs e)
    {
        if (_player == null) return;

        try
        {
            if (_player.IsPlaying)
            {
                _player.Pause();
                _handler.RemoveCallbacksAndMessages(null);
                SetIcon(playing: false);
            }
            else
            {
                _player.Start();
                SetIcon(playing: true);
                StartPolling();
            }
        }
        catch { /* not prepared */ }
    }

    private void OnWaveformTouch(object sender, AView.TouchEventArgs e)
    {
        if (e.Event.Action != MotionEventActions.Up)
        {
            e.Handled = false;
            return;
        }

        if (_player != null && _waveform.Width > 0 && _total > TimeSpan.Zero)
        {
            var progress = Math.Clamp(e.Event.GetX() / _waveform.Width, 0f, 1f);
            try
            {
                _player.SeekTo((int)(progress * _total.TotalMilliseconds));
                _waveform.SetProgress(progress);
                _durationLabel.Text = AudioWaveformHelper.Format(TimeSpan.FromMilliseconds(progress * _total.TotalMilliseconds));
            }
            catch { }
        }
        e.Handled = true;
    }

    private void StartPolling()
    {
        _handler.RemoveCallbacksAndMessages(null);
        _poll = () =>
        {
            if (_player == null || !_player.IsPlaying) return;

            var position = _player.CurrentPosition; // ms
            var totalMs = _total.TotalMilliseconds > 0 ? _total.TotalMilliseconds : _player.Duration;
            _waveform.SetProgress(totalMs > 0 ? (float)(position / totalMs) : 0);
            _durationLabel.Text = AudioWaveformHelper.Format(TimeSpan.FromMilliseconds(position));
            _handler.PostDelayed(_poll, 50);
        };
        _handler.PostDelayed(_poll, 50);
    }

    private void SetIcon(bool playing)
    {
        _button.SetImageResource(playing
            ? global::Android.Resource.Drawable.IcMediaPause
            : global::Android.Resource.Drawable.IcMediaPlay);
        _button.SetColorFilter(_iconColor);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _button.Click -= OnButtonClick;
            _waveform.Touch -= OnWaveformTouch;
            Release();
        }
        base.Dispose(disposing);
    }
}
