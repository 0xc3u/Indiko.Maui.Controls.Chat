using AVFoundation;
using Foundation;
using UIKit;

namespace Indiko.Maui.Controls.Chat.Platforms.iOS;

/// <summary>
/// Drives voice-note playback for an audio cell: wires the play/pause button, a tap-to-seek
/// waveform, a progress timer and the elapsed/total duration label. One instance per cell,
/// created once and reconfigured per bound message.
/// </summary>
internal sealed class VoiceNotePlayer : NSObject
{
    private readonly UIButton _button;
    private readonly WaveformView _waveform;
    private readonly UILabel _durationLabel;

    private AVAudioPlayer _player;
    private NSTimer _timer;
    private TimeSpan _total;
    private UIColor _iconColor = UIColor.White;
    private static bool _sessionConfigured;

    public VoiceNotePlayer(UIButton button, WaveformView waveform, UILabel durationLabel)
    {
        _button = button;
        _waveform = waveform;
        _durationLabel = durationLabel;

        _button.TouchUpInside += OnButtonTapped;

        _waveform.UserInteractionEnabled = true;
        _waveform.AddGestureRecognizer(new UITapGestureRecognizer(OnSeekTap));
    }

    public void Configure(string filePath, TimeSpan? duration, float[] samples,
        UIColor barColor, UIColor progressColor, UIColor iconColor, UIColor textColor)
    {
        Stop();
        EnsureAudioSession();

        _iconColor = iconColor;
        _waveform.BarColor = barColor;
        _waveform.ProgressColor = progressColor;
        _waveform.Samples = samples;
        _waveform.Progress = 0;
        _durationLabel.TextColor = textColor;

        _player?.Dispose();
        _player = null;
        _total = duration ?? TimeSpan.Zero;

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            _player = AVAudioPlayer.FromUrl(NSUrl.FromFilename(filePath), out var error);
            if (error != null)
                Console.WriteLine($"[VoiceNotePlayer] Failed to load audio: {error.LocalizedDescription}");
            if (_player != null)
            {
                _player.PrepareToPlay();
                if (_total <= TimeSpan.Zero)
                    _total = TimeSpan.FromSeconds(_player.Duration);
            }
        }

        SetIcon(playing: false);
        _durationLabel.Text = AudioWaveformHelper.Format(_total);
    }

    public void Stop()
    {
        _timer?.Invalidate();
        _timer = null;
        if (_player != null)
        {
            if (_player.Playing) _player.Stop();
            _player.CurrentTime = 0;
        }
        _waveform.Progress = 0;
        SetIcon(playing: false);
        if (_total > TimeSpan.Zero)
            _durationLabel.Text = AudioWaveformHelper.Format(_total);
    }

    private void OnButtonTapped(object sender, EventArgs e)
    {
        if (_player == null) return;

        if (_player.Playing)
        {
            _player.Pause();
            _timer?.Invalidate();
            _timer = null;
            SetIcon(playing: false);
        }
        else
        {
            _player.Play();
            SetIcon(playing: true);
            _timer?.Invalidate();
            _timer = NSTimer.CreateRepeatingScheduledTimer(0.05, _ => Tick());
        }
    }

    private void Tick()
    {
        if (_player == null) return;

        if (!_player.Playing)
        {
            // Reached the end (or was interrupted) — reset to the start.
            Stop();
            return;
        }

        var elapsed = TimeSpan.FromSeconds(_player.CurrentTime);
        _waveform.Progress = _total.TotalSeconds > 0 ? (nfloat)(elapsed.TotalSeconds / _total.TotalSeconds) : 0;
        _durationLabel.Text = AudioWaveformHelper.Format(elapsed);
    }

    private void OnSeekTap(UITapGestureRecognizer recognizer)
    {
        if (_player == null || _waveform.Bounds.Width <= 0) return;

        var x = recognizer.LocationInView(_waveform).X;
        var progress = Math.Clamp((double)(x / _waveform.Bounds.Width), 0, 1);
        _player.CurrentTime = progress * _total.TotalSeconds;
        _waveform.Progress = (nfloat)progress;
        _durationLabel.Text = AudioWaveformHelper.Format(TimeSpan.FromSeconds(_player.CurrentTime));
    }

    private void SetIcon(bool playing)
    {
        var image = UIImage.GetSystemImage(playing ? "pause.fill" : "play.fill")?.ApplyTintColor(_iconColor);
        _button.SetImage(image, UIControlState.Normal);
        _button.TintColor = _iconColor;
    }

    private static void EnsureAudioSession()
    {
        if (_sessionConfigured) return;
        try
        {
            var session = AVAudioSession.SharedInstance();
            session.SetCategory(AVAudioSessionCategory.Playback);
            session.SetActive(true);
            _sessionConfigured = true;
        }
        catch
        {
            // Non-fatal: playback still works in most cases without an explicit session.
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer?.Invalidate();
            _timer = null;
            _button.TouchUpInside -= OnButtonTapped;
            _player?.Dispose();
            _player = null;
        }
        base.Dispose(disposing);
    }
}
